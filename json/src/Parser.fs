namespace Krama.Json

open System
open System.Collections
open System.Collections.Generic

#if FABLE_COMPILER
[<AutoOpen>]
module Parser =

    open System
    open System.Globalization
    open System.Text

    module internal UnicodeHelper =
        // used http://en.wikipedia.org/wiki/UTF-16#Code_points_U.2B010000_to_U.2B10FFFF as a guide below
        let getUnicodeSurrogatePair num =
            // only code points U+010000 to U+10FFFF supported
            // for coversion to UTF16 surrogate pair
            let codePoint = num - 0x010000u
            let highTenBitMask = 0xFFC00u // 1111|1111|1100|0000|0000
            let lowTenBitMask = 0x003FFu // 0000|0000|0011|1111|1111
            let leadSurrogate = (codePoint &&& highTenBitMask >>> 10) + 0xD800u
            let trailSurrogate = (codePoint &&& lowTenBitMask) + 0xDC00u

            char leadSurrogate, char trailSurrogate

    type internal Parser(jsonText: string) =
        let mutable i = 0
        let s = jsonText

        let buf = StringBuilder() // pre-allocate buffers for strings

        // Helper functions
        let skipWhitespace () =
            while i < s.Length && Char.IsWhiteSpace s.[i] do
                i <- i + 1

        let isNumChar c = Char.IsDigit c || c = '.' || c = 'e' || c = 'E' || c = '+' || c = '-'

        let throw () =
            let msg =
                sprintf
                    "Invalid JSON at position %d; Context - %s"
                    i
                    (jsonText.[(max 0 (i - 10)) .. (min (jsonText.Length - 1) (i + 10))])

            failwith msg

        let ensure cond = if not cond then throw ()

        // Recursive descent parser for JSON that uses global mutable index
        let rec parseValue () =
            skipWhitespace ()
            ensure (i < s.Length)
            let start = i

            match s.[i] with
            | '"' -> { Position = Some start; Value = JString(parseString ()) }
            | '-' -> parseNum ()
            | c when Char.IsDigit(c) -> parseNum ()
            | '{' -> parseObject ()
            | '[' -> parseArray ()
            | 't' -> parseLiteral ("true", JBool true)
            | 'f' -> parseLiteral ("false", JBool false)
            | 'n' -> parseLiteral ("null", JNull)
            | _ -> throw ()

        and parseString () =
            ensure (i < s.Length && s.[i] = '"')
            i <- i + 1

            while i < s.Length && s.[i] <> '"' do
                if s.[i] = '\\' then
                    ensure (i + 1 < s.Length)

                    match s.[i + 1] with
                    | 'b' -> buf.Append('\b') |> ignore
                    | 'f' -> buf.Append('\f') |> ignore
                    | 'n' -> buf.Append('\n') |> ignore
                    | 't' -> buf.Append('\t') |> ignore
                    | 'r' -> buf.Append('\r') |> ignore
                    | '\\' -> buf.Append('\\') |> ignore
                    | '/' -> buf.Append('/') |> ignore
                    | '"' -> buf.Append('"') |> ignore
                    | 'u' ->
                        ensure (i + 5 < s.Length)

                        let hexdigit d =
                            if d >= '0' && d <= '9' then int32 d - int32 '0'
                            elif d >= 'a' && d <= 'f' then int32 d - int32 'a' + 10
                            elif d >= 'A' && d <= 'F' then int32 d - int32 'A' + 10
                            else failwith "hexdigit"

                        let unicodeChar (s: string) =
                            if s.Length <> 4 then failwith "unicodeChar"

                            char (
                                hexdigit s.[0] * 4096
                                + hexdigit s.[1] * 256
                                + hexdigit s.[2] * 16
                                + hexdigit s.[3]
                            )

                        let ch = unicodeChar (s.Substring(i + 2, 4))
                        buf.Append(ch) |> ignore
                        i <- i + 4 // the \ and u will also be skipped past further below
                    | 'U' ->
                        ensure (i + 9 < s.Length)

                        let unicodeChar (s: string) =
                            if s.Length <> 8 then failwith "unicodeChar"

                            if s.[0..1] <> "00" then failwith "unicodeChar"

                            UnicodeHelper.getUnicodeSurrogatePair <| UInt32.Parse(s, NumberStyles.HexNumber)

                        let lead, trail = unicodeChar (s.Substring(i + 2, 8))
                        buf.Append(lead) |> ignore
                        buf.Append(trail) |> ignore
                        i <- i + 8 // the \ and u will also be skipped past further below
                    | _ -> throw ()

                    i <- i + 2 // skip past \ and next char
                else
                    buf.Append(s.[i]) |> ignore
                    i <- i + 1

            ensure (i < s.Length && s.[i] = '"')
            i <- i + 1
            let str = buf.ToString()
            buf.Clear() |> ignore
            str

        and parseNum () =
            let start = i

            while i < s.Length && (isNumChar s.[i]) do
                i <- i + 1

            let len = i - start
            let sub = s.Substring(start, len)

            match StringParser.parseFloat CultureInfo.InvariantCulture sub with
            | Some x -> { Position = Some start; Value = JNumber(float (sub.ToString())) }
            | _ -> throw ()

        and parsePair () =
            let key = parseString ()
            skipWhitespace ()
            ensure (i < s.Length && s.[i] = ':')
            i <- i + 1
            skipWhitespace ()
            key, parseValue ()

        and parseObject () =
            ensure (i < s.Length && s.[i] = '{')
            i <- i + 1
            skipWhitespace ()
            let start = i
            let pairs = ResizeArray<_>()

            if i < s.Length && s.[i] = '"' then
                pairs.Add(parsePair ())
                skipWhitespace ()

                while i < s.Length && s.[i] = ',' do
                    i <- i + 1
                    skipWhitespace ()
                    pairs.Add(parsePair ())
                    skipWhitespace ()

            ensure (i < s.Length && s.[i] = '}')
            i <- i + 1

            { Position = Some start; Value = JObject(pairs.ToArray()) }

        and parseArray () =
            ensure (i < s.Length && s.[i] = '[')
            i <- i + 1
            skipWhitespace ()
            let start = i

            let vals = ResizeArray<_>()

            if i < s.Length && s.[i] <> ']' then
                vals.Add(parseValue ())
                skipWhitespace ()

                while i < s.Length && s.[i] = ',' do
                    i <- i + 1
                    skipWhitespace ()
                    vals.Add(parseValue ())
                    skipWhitespace ()

            ensure (i < s.Length && s.[i] = ']')
            i <- i + 1

            { Position = Some start; Value = JArray(vals.ToArray()) }

        and parseLiteral (expected, r) =
            ensure (i + expected.Length <= s.Length)

            for j in 0 .. expected.Length - 1 do
                ensure (s.[i + j] = expected.[j])

            i <- i + expected.Length
            { Position = Some i; Value = r }

        // Start by parsing the top-level value
        member _.Parse(decoder: JsonNode -> 'a) =
            let value = parseValue ()
            skipWhitespace ()

            if i <> s.Length then throw ()

            decoder value
#else
[<AutoOpen>]
module Parser =

    open System
    open System.Globalization
    open System.Text

    module internal UnicodeHelper =
        // used http://en.wikipedia.org/wiki/UTF-16#Code_points_U.2B010000_to_U.2B10FFFF as a guide below
        let getUnicodeSurrogatePair num =
            // only code points U+010000 to U+10FFFF supported
            // for coversion to UTF16 surrogate pair
            let codePoint = num - 0x010000u
            let highTenBitMask = 0xFFC00u // 1111|1111|1100|0000|0000
            let lowTenBitMask = 0x003FFu // 0000|0000|0011|1111|1111

            let leadSurrogate = (codePoint &&& highTenBitMask >>> 10) + 0xD800u

            let trailSurrogate = (codePoint &&& lowTenBitMask) + 0xDC00u

            char leadSurrogate, char trailSurrogate

        let unicodeChar8 (s: ReadOnlySpan<char>) =
            if s.Length <> 8 then raise (InvalidCharacterException $"Unicode char {s.ToString()}")

            if not (s.[0] = '0' && s.[1] = '0') then
                raise (InvalidCharacterException $"Unicode char {s.ToString()}")

            getUnicodeSurrogatePair <| UInt32.Parse(s, NumberStyles.HexNumber)

        let hexdigit d =
            if d >= '0' && d <= '9' then int32 d - int32 '0'
            elif d >= 'a' && d <= 'f' then int32 d - int32 'a' + 10
            elif d >= 'A' && d <= 'F' then int32 d - int32 'A' + 10
            else failwith "hexdigit"

        let unicodeChar4 (s: ReadOnlySpan<char>) =
            if s.Length <> 4 then
                raise (InvalidCharacterException $"Invalid unicode char in JSON - {s.ToString()}")

            try
                char (
                    hexdigit s.[0] * 4096
                    + hexdigit s.[1] * 256
                    + hexdigit s.[2] * 16
                    + hexdigit s.[3]
                )
            with _ ->
                raise (InvalidCharacterException $"Invalid unicode char in JSON - {s.ToString()}")

    type internal Parser(jsonText: string) =
        let mutable i = 0

        let buf = StringBuilder() // pre-allocate buffers for strings

        // Helper functions
        let skipWhitespace (s: ReadOnlySpan<char>) =
            while i < s.Length && Char.IsWhiteSpace s.[i] do
                i <- i + 1

        let isNumChar c = Char.IsDigit c || c = '.' || c = 'e' || c = 'E' || c = '+' || c = '-'

        let throw () =
            let msg =
                sprintf
                    "Invalid JSON at position %d; {...%s...}"
                    i
                    (jsonText.[(max 0 (i - 10)) .. (min (jsonText.Length - 1) (i + 10))])

            raise (JsonParserException msg)

        let ensure cond = if not cond then throw ()

        // Recursive descent parser for JSON that uses global mutable index
        let rec parseValue (s) : JsonNode =
            skipWhitespace (s)
            ensure (i < s.Length)
            let start = i

            match s.[i] with
            | '"' -> { Position = Some start; Value = JString(parseString (s)) }
            | '-' -> parseNum (s)
            | c when Char.IsDigit(c) -> parseNum (s)
            | '{' -> parseObject (s)
            | '[' -> parseArray (s)
            | 't' -> parseLiteral (s, "true", JBool true)
            | 'f' -> parseLiteral (s, "false", JBool false)
            | 'n' -> parseLiteral (s, "null", JNull)
            | _ -> throw ()

        and parseString (s: ReadOnlySpan<char>) =
            ensure (i < s.Length && s.[i] = '"')
            i <- i + 1
            let start = i

            while i < s.Length && s.[i] <> '"' do
                if s.[i] = '\\' then
                    ensure (i + 1 < s.Length)

                    match s.[i + 1] with
                    | 'b' -> buf.Append('\b') |> ignore
                    | 'f' -> buf.Append('\f') |> ignore
                    | 'n' -> buf.Append('\n') |> ignore
                    | 't' -> buf.Append('\t') |> ignore
                    | 'r' -> buf.Append('\r') |> ignore
                    | '\\' -> buf.Append('\\') |> ignore
                    | '/' -> buf.Append('/') |> ignore
                    | '"' -> buf.Append('"') |> ignore
                    | 'u' ->
                        ensure (i + 5 < s.Length)

                        let ch = UnicodeHelper.unicodeChar4 (s.Slice(i + 2, 4))

                        buf.Append(ch) |> ignore
                        i <- i + 4 // the \ and u will also be skipped past further below
                    | 'U' ->
                        ensure (i + 9 < s.Length)

                        let lead, trail = UnicodeHelper.unicodeChar8 (s.Slice(i + 2, 8))

                        buf.Append(lead) |> ignore
                        buf.Append(trail) |> ignore
                        i <- i + 8 // the \ and u will also be skipped past further below
                    | _ -> throw ()

                    i <- i + 2 // skip past \ and next char
                else
                    buf.Append(s.[i]) |> ignore
                    i <- i + 1

            ensure (i < s.Length && s.[i] = '"')
            i <- i + 1

            let str = buf.ToString()
            buf.Clear() |> ignore
            str

        and parseNum (s: ReadOnlySpan<char>) =
            let start = i

            while i < s.Length && (isNumChar s.[i]) do
                i <- i + 1

            let len = i - start
            let sub = s.Slice(start, len)

            { Position = Some start; Value = JNumber(float (sub.ToString())) }

        and parsePair (s: ReadOnlySpan<char>) : (string * JsonNode) =
            let key = parseString (s)
            skipWhitespace (s)
            ensure (i < s.Length && s.[i] = ':')
            i <- i + 1
            skipWhitespace (s)
            key, parseValue (s)

        and parseObject (s: ReadOnlySpan<char>) : JsonNode =
            ensure (i < s.Length && s.[i] = '{')
            i <- i + 1
            skipWhitespace (s)
            let start = i

            let pairs = ResizeArray<_>()

            if i < s.Length && s.[i] = '"' then
                pairs.Add(parsePair (s))
                skipWhitespace (s)

                while i < s.Length && s.[i] = ',' do
                    i <- i + 1
                    skipWhitespace (s)
                    pairs.Add(parsePair (s))
                    skipWhitespace (s)

            ensure (i < s.Length && s.[i] = '}')
            i <- i + 1

            { Position = Some start; Value = (pairs.ToArray()) |> JObject }

        and parseArray (s: ReadOnlySpan<char>) : JsonNode =
            ensure (i < s.Length && s.[i] = '[')
            i <- i + 1
            skipWhitespace (s)
            let start = i

            let vals = ResizeArray<_>()

            if i < s.Length && s.[i] <> ']' then
                vals.Add(parseValue (s))
                skipWhitespace (s)

                while i < s.Length && s.[i] = ',' do
                    i <- i + 1
                    skipWhitespace (s)
                    vals.Add(parseValue (s))
                    skipWhitespace (s)

            ensure (i < s.Length && s.[i] = ']')
            i <- i + 1

            { Position = Some start; Value = JArray(vals.ToArray()) }

        and parseLiteral (s, expected, r) =
            ensure (i + expected.Length <= s.Length)
            let start = i

            for j in 0 .. expected.Length - 1 do
                ensure (s.[i + j] = expected.[j])

            i <- i + expected.Length
            { Position = Some i; Value = r }

        // Start by parsing the top-level value
        member _.Parse(decoder: JsonNode -> 'a) =
            let getContext (ex: Exception) =
                let pos =
                    ex.Data
                    |> Seq.cast<KeyValuePair<string, int>>
                    |> Seq.map (fun ent -> ent.Key |> unbox |> string, ent.Value |> unbox |> int)
                    |> Seq.find (fun (k, v) -> k = "Position")
                    |> function
                        | (k, v) -> v

                let ctx = (jsonText.[(max 0 (pos - 10)) .. (min (jsonText.Length - 1) (pos + 10))])
                (pos, ctx)

            try
                let s = jsonText.AsSpan()
                let value = parseValue (s)
                skipWhitespace (s)

                if i <> s.Length then throw ()

                decoder value
            with
            | :? JsonParserException as ex -> raise ex
            | :? PropertyNotFoundException as ex ->
                let pos, ctx = getContext ex

                raise (
                    PropertyNotFoundException(
                        ex.Message + $"; Object starting at {pos}; " + "{..." + ctx + "...}",
                        None
                    )
                )

                raise ex
            | :? InvalidPropertyTypeException as ex ->
                let pos, ctx = getContext ex
                raise (InvalidPropertyTypeException(ex.Message + "; {..." + ctx + "...}", None))
            | :? InvalidCharacterException as ex -> raise ex

#endif
