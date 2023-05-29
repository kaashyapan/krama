namespace Krama.Json

[<AutoOpen>]
module internal Convert =

    open System
    open System.Globalization
    open Krama.Json

    let inline floatInRange min max (f: float) =
        let _min = float min
        let _max = float max
        f >= _min && f <= _max

    let convertOrFail (typeName: string) (ctor: CultureInfo -> JsonNode -> 'a option) (jnode: JsonNode) : 'a =
        let cul = CultureInfo.InvariantCulture

        match ctor cul jnode with
        | Some s -> s
        | None ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a '{typeName}' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let convertOrNone (ctor: CultureInfo -> JsonNode -> 'a option) (jsonType: JsonNode) : 'a option =
        let cul = CultureInfo.InvariantCulture
        ctor cul jsonType

    let asString (cul: CultureInfo) (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JBool b when b -> Some(if b then "true" else "false")
        | JString s -> Some s
        | JNumber n -> Some(n.ToString(cul))
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'string' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let asInt16 (cul: CultureInfo) (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JNumber n -> Some <| int16 n
        | JString s -> StringParser.parseInt16 cul s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'int16' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let asInt32 (cul: CultureInfo) (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JNumber n -> Some <| int32 n
        | JString s -> StringParser.parseInt32 cul s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'int32' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let asInt64 (cul: CultureInfo) (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JNumber n -> Some <| int64 n
        | JString s -> StringParser.parseInt64 cul s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'int64' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let asInt (cul: CultureInfo) (jnode: JsonNode) = asInt32 cul jnode

    let asBool (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JBool b -> Some b
        | JString s -> StringParser.parseBoolean s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'boolean' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let asFloat (cul: CultureInfo) (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JNumber n -> Some(float n)
        | JString s -> StringParser.parseFloat cul s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'float' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let asDecimal (cul: CultureInfo) (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JNumber n -> Some(decimal n)
        | JString s -> StringParser.parseDecimal cul s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'decimal' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

    let asDateTime (cul: CultureInfo) (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JNumber n ->
            match n with
            | n when floatInRange Int64.MinValue Int64.MaxValue n -> Some(epoch.AddMilliseconds(float n))
            | _ ->
                raise (
                    InvalidPropertyTypeException(
                        $"Expected a 'DateTime' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                        Some jnode
                    )
                )
        | JString s -> StringParser.parseDateTime cul s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'DateTime' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let asDateTimeOffset (cul: CultureInfo) (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JNumber n ->
            match n with
            | n when floatInRange Int64.MinValue Int64.MaxValue n ->
                Some(DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(n)))

            | _ ->
                raise (
                    InvalidPropertyTypeException(
                        $"Expected a 'DateTimeOffset' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                        Some jnode
                    )
                )
        | JString s -> StringParser.parseDateTimeOffset cul s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'DateTimeOffset' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let asTimeSpan (cul: CultureInfo) (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JString s -> StringParser.parseTimeSpan cul s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'TimeSpan' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    let asGuid (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JString s -> StringParser.parseGuid s
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'Guid' in JSON at position {jnode.Position.Value}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )
