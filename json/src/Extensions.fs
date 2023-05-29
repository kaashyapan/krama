namespace Krama.Json

open System
open System.Globalization
open System.Runtime.CompilerServices
open Krama.Json

[<AutoOpen>]
[<Extension>]
type Decode() =

    [<Extension>]
    static member TryGet(jnode: JsonNode, name: string) : JsonNode option =
        match jnode.Value with
        | JObject props -> Array.tryFind (fst >> (=) name) props |> Option.map snd
        | _ -> None

    [<Extension>]
    static member TryGet(jnode: JsonNode option, name: string) : JsonNode option =
        match jnode with
        | Some j ->
            match j.Value with
            | JObject props -> Array.tryFind (fst >> (=) name) props |> Option.map snd
            | _ -> None
        | _ -> None

    [<Extension>]
    static member Get(jnode: JsonNode, name: string) : JsonNode =
        match jnode.TryGet(name) with
        | Some prop -> prop
        | None -> raise (PropertyNotFoundException($"Property '{name}' does not exist in JSON", Some jnode))

    [<Extension>]
    static member Get(jnode: JsonNode option, name: string) : JsonNode =
        match jnode with
        | Some j ->
            match j.TryGet(name) with
            | Some prop -> prop
            | None -> raise (PropertyNotFoundException($"Property '{name}' does not exist in JSON", Some j))
        | None -> raise (PropertyNotFoundException($"Property '{name}' does not exist in JSON", None))

    [<Extension>]
    static member AsPropertyArray(jnode: JsonNode) =
        match jnode.Value with
        | JObject properties -> properties
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected an 'object/map' in JSON at position {jnode.Position}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    [<Extension>]
    static member AsPropertyArrayOrNone(jnode: JsonNode) =
        match jnode.Value with
        | JObject properties -> properties |> Some
        | JNull -> None
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected an 'object/map' in JSON at position {jnode.Position}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    [<Extension>]
    static member AsPropertyArrayOrNone(jnode: JsonNode option) =
        match jnode with
        | Some prop ->
            match prop.Value with
            | JObject properties -> properties |> Some
            | JNull -> None
            | _ ->
                raise (
                    InvalidPropertyTypeException(
                        $"Expected an 'object/map' in JSON at position {prop.Position}. Got - {Json.serializeObj (prop)}",
                        Some prop
                    )
                )

        | None -> None

    [<Extension>]
    static member AsArray(jnode: JsonNode) : JsonNode[] =
        match jnode.Value with
        | JArray a -> a
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected an 'array' in JSON at position {jnode.Position}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    [<Extension>]
    static member AsArrayOrNone(jnode: JsonNode) : JsonNode[] option =
        match jnode.Value with
        | JArray a -> Some a
        | JNull -> None
        | _ ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected an 'array' in JSON at position {jnode.Position}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    [<Extension>]
    static member AsArrayOrNone(jnode: JsonNode option) : JsonNode[] option =
        match jnode with
        | Some prop ->
            match prop.Value with
            | JArray a -> Some a
            | JNull -> None
            | _ ->
                raise (
                    InvalidPropertyTypeException(
                        $"Expected an 'array' in JSON at position {prop.Position}. Got - {Json.serializeObj (prop)}",
                        Some prop
                    )
                )

        | None -> None

    [<Extension>]
    static member inline Item(jnode: JsonNode, i: int) : JsonNode = jnode.AsArray().[i]

    [<Extension>]
    static member TryStringFormat(jnode: JsonNode, cul: CultureInfo) : string option = asString cul jnode

    [<Extension>]
    static member AsStringFormat(jnode: JsonNode, cul: CultureInfo) : string =
        match jnode.TryStringFormat cul with
        | Some s -> s
        | None ->
            raise (
                InvalidPropertyTypeException(
                    $"Expected a 'string' in JSON at position {jnode.Position}. Got - {Json.serializeObj (jnode)}",
                    Some jnode
                )
            )

    [<Extension>]
    static member AsString(jnode: JsonNode) : string = jnode.AsStringFormat CultureInfo.InvariantCulture

    [<Extension>]
    static member AsStringOrNone(jnode: JsonNode) : string option = jnode.TryStringFormat CultureInfo.InvariantCulture

    [<Extension>]
    static member AsStringOrNone(jnode: JsonNode option) : string option =
        match jnode with
        | Some prop -> prop.AsStringOrNone()
        | None -> None

    [<Extension>]
    static member AsInt16(jnode: JsonNode) : int16 = convertOrFail "Int16" asInt16 jnode

    [<Extension>]
    static member AsInt16OrNone(jnode: JsonNode) : int16 option = convertOrNone asInt16 jnode

    [<Extension>]
    static member AsInt16OrNone(jnode: JsonNode option) : int16 option =
        match jnode with
        | Some prop -> convertOrNone asInt16 prop
        | None -> None

    [<Extension>]
    static member AsInt32(jnode: JsonNode) : int32 = convertOrFail "Int32" asInt32 jnode

    [<Extension>]
    static member AsInt32OrNone(jnode: JsonNode) : int32 option = convertOrNone asInt32 jnode

    [<Extension>]
    static member AsInt32OrNone(jnode: JsonNode option) : int32 option =
        match jnode with
        | Some prop -> convertOrNone asInt32 prop
        | None -> None

    [<Extension>]
    static member AsInt64(jnode: JsonNode) : int64 = convertOrFail "Int64" asInt64 jnode

    [<Extension>]
    static member AsInt64OrNone(jnode: JsonNode) : int64 option = convertOrNone asInt64 jnode

    [<Extension>]
    static member AsInt64OrNone(jnode: JsonNode option) : int64 option =
        match jnode with
        | Some prop -> convertOrNone asInt64 prop
        | None -> None

    [<Extension>]
    static member AsInt(jnode: JsonNode) : int32 = jnode.AsInt32()

    [<Extension>]
    static member AsIntOrNone(jnode: JsonNode) : int32 option = jnode.AsInt32OrNone()

    [<Extension>]
    static member AsIntOrNone(jnode: JsonNode option) : int32 option = jnode.AsInt32OrNone()

    [<Extension>]
    static member AsBool(jnode: JsonNode) : bool = convertOrFail "Bool" (fun _ json -> asBool json) jnode

    [<Extension>]
    static member AsBoolOrNone(jnode: JsonNode) : bool option = convertOrNone (fun _ json -> asBool json) jnode

    [<Extension>]
    static member AsBoolOrNone(jnode: JsonNode option) : bool option =
        match jnode with
        | Some prop -> convertOrNone (fun _ json -> asBool json) prop
        | None -> None

    [<Extension>]
    static member AsFloat(jnode: JsonNode) : float = convertOrFail "Float" asFloat jnode

    [<Extension>]
    static member AsFloatOrNone(jnode: JsonNode) : float option = convertOrNone asFloat jnode

    [<Extension>]
    static member AsFloatOrNone(jnode: JsonNode option) : float option =
        match jnode with
        | Some prop -> convertOrNone asFloat prop
        | None -> None

    [<Extension>]
    static member AsDecimal(jnode: JsonNode) : decimal = convertOrFail "Decimal" asDecimal jnode

    [<Extension>]
    static member AsDecimalOrNone(jnode: JsonNode) : decimal option = convertOrNone asDecimal jnode

    [<Extension>]
    static member AsDecimalOrNone(jnode: JsonNode option) : decimal option =
        match jnode with
        | Some prop -> convertOrNone asDecimal prop
        | None -> None

    [<Extension>]
    static member AsDateTime(jnode: JsonNode) : DateTime = convertOrFail "DateTime" asDateTime jnode

    [<Extension>]
    static member AsDateTimeOrNone(jnode: JsonNode) : DateTime option = convertOrNone asDateTime jnode

    [<Extension>]
    static member AsDateTimeOrNone(jnode: JsonNode option) : DateTime option =
        match jnode with
        | Some prop -> convertOrNone asDateTime prop
        | None -> None

    [<Extension>]
    static member AsDateTimeOffset(jnode: JsonNode) : DateTimeOffset =
        convertOrFail "DateTimeOffset" asDateTimeOffset jnode

    [<Extension>]
    static member AsDateTimeOffsetOrNone(jnode: JsonNode) : DateTimeOffset option = convertOrNone asDateTimeOffset jnode

    [<Extension>]
    static member AsDateTimeOffsetOrNone(jnode: JsonNode option) : DateTimeOffset option =
        match jnode with
        | Some prop -> convertOrNone asDateTimeOffset prop
        | None -> None

    [<Extension>]
    static member AsTimeSpan(jnode: JsonNode) : TimeSpan = convertOrFail "TimeSpan" asTimeSpan jnode

    [<Extension>]
    static member AsTimeSpanOrNone(jnode: JsonNode) : TimeSpan option = convertOrNone asTimeSpan jnode

    [<Extension>]
    static member AsTimeSpanOrNone(jnode: JsonNode option) : TimeSpan option =
        match jnode with
        | Some prop -> convertOrNone asTimeSpan prop
        | None -> None

    [<Extension>]
    static member AsGuid(jnode: JsonNode) : Guid = convertOrFail "Guid" (fun _ json -> asGuid json) jnode

    [<Extension>]
    static member AsGuidOrNone(jnode: JsonNode) : Guid option = convertOrNone (fun _ json -> asGuid json) jnode

    [<Extension>]
    static member AsGuidOrNone(jnode: JsonNode option) : Guid option =
        match jnode with
        | Some prop -> convertOrNone (fun _ json -> asGuid json) prop
        | None -> None
