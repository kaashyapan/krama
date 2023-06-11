namespace KitchensinkJson

open System
open System.Runtime.CompilerServices
open Krama.Json

type Encoders () =
    static member encode (value: Kitchensink.Address) = 23
    static member encode (value: Kitchensink.Address option) = 23
    static member encode (value: Kitchensink.Age) = 23
    static member encode (value: Kitchensink.Age option) = 23
    static member encode (value: Kitchensink.Pet) = 23
    static member encode (value: Kitchensink.Pet option) = 23
    static member encode (value: Kitchensink.Sex) = 23
    static member encode (value: Kitchensink.Sex option) = 23
    static member encode (value: Kitchensink.Cht) = 23
    static member encode (value: Kitchensink.Cht option) = 23
    static member encode (value: Kitchensink.Payload) = 23
    static member encode (value: Kitchensink.Payload option) = 23

type Decoders () =
    [<Extension>]
    static member asAddress (jnode: JsonNode) = 23

    [<Extension>]
    static member asAddressOrNone (jnode: JsonNode) =
        match jnode.typ with
        | JObject [||] -> None
        | JObject _ -> jnode.AsAddress() |> Some
        | JNull -> None
        | _ -> failwith "Unexpected value"

    [<Extension>]
    static member asAddressOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsAddressOrNone())

    [<Extension>]
    static member asAge (jnode: JsonNode) = 23

    [<Extension>]
    static member asAgeOrNone (jnode: JsonNode) = 23

    [<Extension>]
    static member asAgeOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsAgeOrNone())

    [<Extension>]
    static member asPet (jnode: JsonNode) = 23

    [<Extension>]
    static member asPetOrNone (jnode: JsonNode) = 23

    [<Extension>]
    static member asPetOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsPetOrNone())

    [<Extension>]
    static member asSex (jnode: JsonNode) = 23

    [<Extension>]
    static member asSexOrNone (jnode: JsonNode) = 23

    [<Extension>]
    static member asSexOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsSexOrNone())

    [<Extension>]
    static member asCht (jnode: JsonNode) = 23

    [<Extension>]
    static member asChtOrNone (jnode: JsonNode) = 23

    [<Extension>]
    static member asChtOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsChtOrNone())

    [<Extension>]
    static member asPayload (jnode: JsonNode) = 23

    [<Extension>]
    static member asPayloadOrNone (jnode: JsonNode) =
        match jnode.typ with
        | JObject [||] -> None
        | JObject _ -> jnode.AsPayload() |> Some
        | JNull -> None
        | _ -> failwith "Unexpected value"

    [<Extension>]
    static member asPayloadOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsPayloadOrNone())

type Kitchensink.Payload with

    static member toJson (payload: Kitchensink.Payload) =
        let encoder = Encoders.encode payload
        Json.serialize encoder payload

    static member fromJson (jsonstr: string) = Json.parse Decoders.AsPayload jsonstr
