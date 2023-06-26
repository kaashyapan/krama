namespace KitchensinkJson

open System
open System.Runtime.CompilerServices
open Krama.Json
type AnonRecord_3651533599 = {| age: int32 ; name: string |}
type List_646123815 = string list
type Tuple_2777257471 = string * string * int32

type Encoders =
    static member encode (value: AnonRecord_3651533599) : JsonNode =
        seq {
            ("age" , JsonNode.encode value.age )
            ("name" , JsonNode.encode value.name )
        }
        |> JsonNode.encode

    static member encode (value: AnonRecord_3651533599 option) : JsonNode =
        match value with
        | Some anonRecord3651533599 -> Encoders.encode anonRecord3651533599
        | None -> JsonNode.encode ()

    static member encode (value: List_646123815) : JsonNode =
        value |> Seq.map JsonNode.encode |> JsonNode.encode

    static member encode (value: List_646123815 option) : JsonNode =
        match value with
        | Some list646123815 -> Encoders.encode list646123815
        | None -> JsonNode.encode ()

    static member encode (value: Tuple_2777257471) : JsonNode =
        let v0,v1,v2 = value

        [ JsonNode.encode v0; JsonNode.encode v1; JsonNode.encode v2 ]
        |> JsonNode.encode

    static member encode (value: Tuple_2777257471 option) : JsonNode =
        match value with
        | Some tuple2777257471 -> Encoders.encode tuple2777257471
        | None -> JsonNode.encode ()

    static member encode (value: Kitchensink.Address) : JsonNode =
        seq {
            ("Street" , JsonNode.encode value.Street )
            ("State'" , JsonNode.encode value.State' )
        }
        |> JsonNode.encode

    static member encode (value: Kitchensink.Address option) : JsonNode =
        match value with
        | Some address -> Encoders.encode address
        | None -> JsonNode.encode ()

    static member encode (value: Kitchensink.Age) : JsonNode = JsonNode.encode value

    static member encode (value: Kitchensink.Age option) : JsonNode =
        match value with
        | Some age -> Encoders.encode age
        | None -> JsonNode.encode ()

    static member encode (value: Kitchensink.Pet) : JsonNode =
        match value with
        | Cat -> JsonNode.encode value
        | Dog -> JsonNode.encode value

    static member encode (value: Kitchensink.Pet option) : JsonNode =
        match value with
        | Some pet -> Encoders.encode pet
        | None -> JsonNode.encode ()

    static member encode (value: Kitchensink.Sex) : JsonNode =
        match value with
        | Male v -> seq { ("Male" , Encoders.encode v ) } |> JsonNode.encode
        | Female -> seq { ("Female" , JsonNode.encode () ) } |> JsonNode.encode

    static member encode (value: Kitchensink.Sex option) : JsonNode =
        match value with
        | Some sex -> Encoders.encode sex
        | None -> JsonNode.encode ()

    static member encode (value: Kitchensink.Cht) : JsonNode =
        match value with
        | Choice1Of2 of v -> JsonNode.encode v
        | Choice2Of2 of v -> JsonNode.encode v

    static member encode (value: Kitchensink.Cht option) : JsonNode =
        match value with
        | Some cht -> Encoders.encode cht
        | None -> JsonNode.encode ()

    static member encode (value: Kitchensink.Payload) : JsonNode =
        seq {
            ("Name" , JsonNode.encode value.Name )
            ("Age" , Encoders.encode value.Age )
            ("Address" , Encoders.encode value.Address )
            ("Sex" , Encoders.encode value.Sex )
            ("Pet" , Encoders.encode value.Pet )
            ("Cht" , Encoders.encode value.Cht )
            ("Books" , Encoders.encode value.Books )
            ("Bills" , Encoders.encode value.Bills )
        }
        |> JsonNode.encode

    static member encode (value: Kitchensink.Payload option) : JsonNode =
        match value with
        | Some payload -> Encoders.encode payload
        | None -> JsonNode.encode ()

type Decoders =
    [<Extension>]
    static member AsKitchensinkAddress (jnode: JsonNode) =
        { Kitchensink.Address.Street = 23
          Kitchensink.Address.State' = 23 }

    [<Extension>]
    static member AsKitchensinkAddressOrNone (jnode: JsonNode) =
        match jnode.Value with
        | JObject [||] -> None
        | JObject _ -> jnode.AsAddress() |> Some
        | JNull -> None
        | _ -> failwith "Unexpected value"

    [<Extension>]
    static member AsKitchensinkAddressOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsKitchensinkAddressOrNone())

    [<Extension>]
    static member AsKitchensinkAge (jnode: JsonNode) = jnode.AsInt32()

    [<Extension>]
    static member AsKitchensinkAgeOrNone (jnode: JsonNode) =
        match jnode.Value with
        | JNull -> None
        | JNumber _ -> jnode.AsKitchensinkAge() |> Some
        | _ -> failwith "Unexpected value"

    [<Extension>]
    static member AsKitchensinkAgeOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsKitchensinkAgeOrNone())

    [<Extension>]
    static member AsKitchensinkPet (jnode: JsonNode) =
        match jnode.Value with
        | "Cat" -> Cat
        | "Dog" -> Dog

    [<Extension>]
    static member AsKitchensinkPetOrNone (jnode: JsonNode) =
        match jnode.Value with
        | JString _ -> jnode.AsPet() |> Some
        | JNull -> None
        | _ -> failwith "Unexpected value"

    [<Extension>]
    static member AsKitchensinkPetOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsKitchensinkPetOrNone())

    [<Extension>]
    static member AsKitchensinkSex (jnode: JsonNode) = 23

    [<Extension>]
    static member AsKitchensinkSexOrNone (jnode: JsonNode) = 23

    [<Extension>]
    static member AsKitchensinkSexOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsKitchensinkSexOrNone())

    [<Extension>]
    static member AsKitchensinkCht (jnode: JsonNode) = 23

    [<Extension>]
    static member AsKitchensinkChtOrNone (jnode: JsonNode) = 23

    [<Extension>]
    static member AsKitchensinkChtOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsKitchensinkChtOrNone())

    [<Extension>]
    static member AsKitchensinkPayload (jnode: JsonNode) =
        { Kitchensink.Payload.Name = 23
          Kitchensink.Payload.Age = 23
          Kitchensink.Payload.Address = 23
          Kitchensink.Payload.Sex = 23
          Kitchensink.Payload.Pet = 23
          Kitchensink.Payload.Cht = 23
          Kitchensink.Payload.Books = 23
          Kitchensink.Payload.Bills = 23 }

    [<Extension>]
    static member AsKitchensinkPayloadOrNone (jnode: JsonNode) =
        match jnode.Value with
        | JObject [||] -> None
        | JObject _ -> jnode.AsPayload() |> Some
        | JNull -> None
        | _ -> failwith "Unexpected value"

    [<Extension>]
    static member AsKitchensinkPayloadOrNone (jnode: JsonNode option) =
        jnode |> Option.bind (fun node -> node.AsKitchensinkPayloadOrNone())

type Kitchensink.Payload with

    static member ToJson (payload: Kitchensink.Payload) : string =
        let encoder (payload: Kitchensink.Payload) = Encoders.encode payload
        Json.serialize encoder payload

    static member FromJson (jsonstr: string) : Kitchensink.Payload = Json.parse Decoders.AsPayload jsonstr
