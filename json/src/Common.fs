namespace Krama.Json

open System
open System.Globalization
open System.Collections

[<AutoOpen>]
type JsonType =
    | JNull
    | JBool of bool
    | JString of string
    | JNumber of float
    | JArray of elements: JsonNode[]
    | JObject of properties: (string * JsonNode)[]

and JsonNode =
    {
        Position: int option //position in the Json string
        Value: JsonType
    }

    member this.typ = this.Value

    static member encode() = { Position = None; Value = JNull }

    static member encode(value: string) = { Position = None; Value = JString(value.ToString()) }

    static member encode(value: int8) = { Position = None; Value = JNumber(float value) }

    static member encode(value: int16) = { Position = None; Value = JNumber(float value) }

    static member encode(value: int32) = { Position = None; Value = JNumber(float value) }

    static member encode(value: int64) = { Position = None; Value = JNumber(float value) }

    static member encode(value: float32) = { Position = None; Value = JNumber(float value) }

    static member encode(value: double) = { Position = None; Value = JNumber(float value) }

    static member encode(value: decimal) = { Position = None; Value = JNumber(float value) }

    static member encode(value: Guid) = { Position = None; Value = JString(value.ToString()) }

    static member encode(value: DateTime) =
        { Position = None; Value = JString(value.ToString("o", CultureInfo.InvariantCulture)) }

    static member encode(value: DateOnly) =
        { Position = None; Value = JString(value.ToString("o", CultureInfo.InvariantCulture)) }

    static member encode(value: DateTimeOffset) =
        { Position = None; Value = JString(value.ToString("o", CultureInfo.InvariantCulture)) }

    static member encode(value: TimeOnly) = { Position = None; Value = JString(value.ToString()) }

    static member encode(value: TimeSpan) = { Position = None; Value = JString(value.ToString()) }

    static member encode(value: bool) = { Position = None; Value = JBool value }

    static member encode(value: string option) : JsonNode =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: int8 option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: int16 option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: int32 option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: int64 option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: float32 option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: double option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: decimal option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: Guid option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: DateTime option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: DateOnly option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: DateTimeOffset option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: TimeOnly option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: TimeSpan option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: bool option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: JsonNode option) = Option.defaultValue { Position = None; Value = JNull } value
    (* 
    static member encode(value: JsonNode voption) =
        ValueOption.defaultValue { Position = None; Value = JNull } value
 *)
    static member encode(value: JsonNode seq) = { Position = None; Value = value |> Seq.toArray |> JArray }

    static member encode(value: (string * JsonNode) seq) = { Position = None; Value = value |> Seq.toArray |> JObject }

    static member encode(value: Map<string, JsonNode>) = { Position = None; Value = value |> Map.toArray |> JObject }

    static member encode(value: JsonNode seq option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: (string * JsonNode) seq option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

    static member encode(value: Map<string, JsonNode> option) =
        value |> Option.map JsonNode.encode |> Option.defaultValue (JsonNode.encode ())

module internal StringParser =
    let parseWith (tryParseFunc: string -> bool * _) =
        tryParseFunc
        >> function
            | true, v -> Some v
            | false, _ -> None

    let parseInt16 cul = parseWith (fun str -> Int16.TryParse(str, NumberStyles.Currency, cul))

    let parseInt32 cul = parseWith (fun str -> Int32.TryParse(str, NumberStyles.Currency, cul))

    let parseInt64 cul = parseWith (fun str -> Int64.TryParse(str, NumberStyles.Currency, cul))

    let parseInt cul = parseInt32 cul

    let parseFloat cul = parseWith (fun str -> Double.TryParse(str, NumberStyles.Currency, cul))

    let parseDecimal cul = parseWith (fun str -> Decimal.TryParse(str, NumberStyles.Currency, cul))

    let parseDateTime cul =
        parseWith (fun str ->
            DateTime.TryParse(str, cul, DateTimeStyles.AllowWhiteSpaces ||| DateTimeStyles.RoundtripKind)
        )

    let parseDateTimeOffset cul =
        parseWith (fun str ->
            DateTimeOffset.TryParse(str, cul, DateTimeStyles.AllowWhiteSpaces ||| DateTimeStyles.RoundtripKind)
        )

    let parseTimeSpan cul = parseWith (fun str -> TimeSpan.TryParse(str, cul))

    let parseBoolean = parseWith Boolean.TryParse
    let parseGuid = parseWith Guid.TryParse

[<AutoOpen>]
module internal SpanParser =
    let inline toOption (parsedResult: bool * _) =
        match parsedResult with
        | true, v -> Some v
        | false, _ -> None

    let parseInt16 (cul: CultureInfo) (spn: ReadOnlySpan<char>) =
        Int16.TryParse(spn, NumberStyles.Currency, CultureInfo.InvariantCulture)
        |> toOption

    let parseInt32 (cul: CultureInfo) (spn: ReadOnlySpan<char>) =
        Int32.TryParse(spn, NumberStyles.Currency, CultureInfo.InvariantCulture)
        |> toOption

    let parseUInt32 (cul: CultureInfo) (spn: ReadOnlySpan<char>) =
        UInt32.Parse(spn, NumberStyles.Currency, CultureInfo.InvariantCulture)

    let parseInt64 (cul: CultureInfo) (spn: ReadOnlySpan<char>) =
        Int64.TryParse(spn, NumberStyles.Currency, CultureInfo.InvariantCulture)
        |> toOption

    let parseInt cul spn = parseInt32 cul spn

    let parseFloat (cul: CultureInfo) (spn: ReadOnlySpan<char>) =
        Double.TryParse(spn, NumberStyles.Currency, CultureInfo.InvariantCulture)
        |> toOption

    let parseDecimal (cul: CultureInfo) (spn: ReadOnlySpan<char>) =
        Decimal.TryParse(spn, NumberStyles.Currency, CultureInfo.InvariantCulture)
        |> toOption

    let parseDateTime (cul: CultureInfo) (spn: ReadOnlySpan<char>) =
        DateTime.TryParse(
            spn,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AllowWhiteSpaces ||| DateTimeStyles.RoundtripKind
        )
        |> toOption

    let parseDateTimeOffset (cul: CultureInfo) (spn: ReadOnlySpan<char>) =
        DateTime.TryParse(
            spn,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AllowWhiteSpaces ||| DateTimeStyles.RoundtripKind
        )
        |> toOption

    let parseTimeSpan (cul: CultureInfo) (spn: ReadOnlySpan<char>) =
        TimeSpan.TryParse(spn, CultureInfo.InvariantCulture) |> toOption

    let parseBoolean (spn: ReadOnlySpan<char>) = Boolean.TryParse(spn) |> toOption
    let parseGuid (spn: ReadOnlySpan<char>) = Guid.TryParse(spn) |> toOption
