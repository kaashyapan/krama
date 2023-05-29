namespace Krama.Json

#if FABLE_COMPILER
// Fable compiler uses StringBuilder to build the JSON string

[<AutoOpen>]
module Json =
    open System
    open System.IO
    open System.Text

    let parse (decoder: JsonNode -> 'a) (str: string) = Parser(str).Parse(decoder)

    let internal serializeObj (json: JsonNode) =
        let propSep = "\":"

        // Encode characters that are not valid in JS string. The implementation is based
        // on https://github.com/mono/mono/blob/master/mcs/class/System.Web/System.Web/HttpUtility.cs
        let jsonEncode (w: StringBuilder) (value: string) =
            if not (String.IsNullOrEmpty value) then
                for i = 0 to value.Length - 1 do
                    let c = value.[i]
                    let ci = int c

                    if ci >= 0 && ci <= 7 || ci = 11 || ci >= 14 && ci <= 31 then
                        w.Append("\\u{0:x4}")
                    // w.Append(ci)
                    else
                        match c with
                        | '\b' -> w.Append "\\b"
                        | '\t' -> w.Append "\\t"
                        | '\n' -> w.Append "\\n"
                        | '\f' -> w.Append "\\f"
                        | '\r' -> w.Append "\\r"
                        | '"' -> w.Append "\\\""
                        | '\\' -> w.Append "\\\\"
                        | _ -> w.Append c
                    |> ignore

        let rec serializeNode (w: StringBuilder) (json: JsonNode) =
            match json.Value with
            | JNull -> w.Append "null"
            | JBool b -> w.Append(if b then "true" else "false")
            | JNumber number -> w.Append number
            | JString s ->
                w.Append("\"") |> ignore
                jsonEncode w s
                w.Append("\"")

            | JObject properties ->
                w.Append("{") |> ignore

                for i = 0 to properties.Length - 1 do
                    let k, v = properties.[i]

                    if i > 0 then w.Append(",") |> ignore

                    w.Append("\"") |> ignore

                    jsonEncode w k
                    w.Append(propSep) |> ignore

                    (serializeNode w v) |> ignore

                w.Append("}")

            | JArray elements ->
                w.Append("[") |> ignore

                for i = 0 to elements.Length - 1 do
                    if i > 0 then w.Append(",") |> ignore

                    elements.[i] |> serializeNode w |> ignore

                w.Append("]")

        let w = new System.Text.StringBuilder()
        json |> serializeNode w |> ignore
        w.ToString()

    let serialize (encoder: 'a -> JsonNode) (fobj: 'a) = fobj |> encoder |> serializeObj

#else

[<AutoOpen>]
module Json =

    open System
    open System.IO

    let parse (decoder: JsonNode -> 'a) (str: string) = Parser(str).Parse(decoder)

    let parseStream (decoder: JsonNode -> 'a) (str: Stream) =
        use reader = new StreamReader(str)
        let text = reader.ReadToEnd()
        Parser(text).Parse(decoder)

    let internal serializeObj (jsonObj: JsonNode) =
        let propSep = "\":"

        // Encode characters that are not valid in JS string. The implementation is based
        // on https://github.com/mono/mono/blob/master/mcs/class/System.Web/System.Web/HttpUtility.cs
        let encode (w: TextWriter) (value: string) =
            if not (String.IsNullOrEmpty value) then
                for i = 0 to value.Length - 1 do
                    let c = value.[i]
                    let ci = int c

                    if ci >= 0 && ci <= 7 || ci = 11 || ci >= 14 && ci <= 31 then
                        w.Write("\\u{0:x4}", ci) |> ignore
                    else
                        match c with
                        | '\b' -> w.Write "\\b"
                        | '\t' -> w.Write "\\t"
                        | '\n' -> w.Write "\\n"
                        | '\f' -> w.Write "\\f"
                        | '\r' -> w.Write "\\r"
                        | '"' -> w.Write "\\\""
                        | '\\' -> w.Write "\\\\"
                        | _ -> w.Write c

        let rec serializeNode (w: TextWriter) (jsonObj: JsonNode) =
            match jsonObj.Value with
            | JNull -> w.Write "null"
            | JBool b -> w.Write(if b then "true" else "false")
            | JNumber number -> w.Write number
            | JString s ->
                w.Write "\""
                encode w s
                w.Write "\""
            | JObject properties ->
                w.Write "{"

                for i = 0 to properties.Length - 1 do
                    let k, v = properties.[i]

                    if i > 0 then w.Write ","

                    w.Write "\""
                    encode w k
                    w.Write propSep
                    serializeNode w v

                w.Write "}"
            | JArray elements ->
                w.Write "["

                for i = 0 to elements.Length - 1 do
                    if i > 0 then w.Write ","

                    serializeNode w elements.[i]

                w.Write "]"

        let w = new StringWriter()
        serializeNode w jsonObj
        w.GetStringBuilder().ToString()

    let serialize (encoder: 'a -> JsonNode) (fobj: 'a) = fobj |> encoder |> serializeObj
#endif
