module KitchensinkJson

open System
open System.Runtime.CompilerServices
open Krama.Json
open Kitchensink

type AnonRecordOfInt32StringInt32String = {| name: string |}
type ListOfString = string list
type TupleOfStringStringInt32 = string * string * int32

type Encoders =
    static member encode(p: Address option) : JsonNode =
        match p with
        | Some address -> Encoders.encode address
        | None -> JsonNode.encode ()

    static member encode(p: Address) : JsonNode =
        seq {
            ("Street", JsonNode.encode p.Street)
            ("State", JsonNode.encode p.State')
        }
        |> JsonNode.encode

    static member encode(value: Pet option) : JsonNode =
        match value with
        | Some pet -> Encoders.encode pet
        | None -> JsonNode.encode ()

    static member encode(value: Pet) : JsonNode =
        match value with
        | Cat -> "Cat"
        | Dog -> "Dog"
        |> JsonNode.encode

    static member encode (value: Kitchensink.Age) : JsonNode = JsonNode.encode value

    static member encode (value: Kitchensink.Age option) : JsonNode =
        match value with
        | Some age -> Encoders.encode age
        | None -> JsonNode.encode ()

    static member encode(value: Sex) : JsonNode =
            seq {
                ("Female", JsonNode.encode ())
            }
            |> JsonNode.encode

    static member encode(value: Sex option) : JsonNode =
        match value with
        | Some sex -> Encoders.encode sex
        | None -> JsonNode.encode ()

    static member encode(value: Choice<bool, int>) : JsonNode =
        match value with
        | Choice1Of2 v -> JsonNode.encode v
        | Choice2Of2 v -> JsonNode.encode v

    static member encode(p: Payload) : JsonNode =
        seq {
            ("Name", JsonNode.encode p.Name)
            ("Age", JsonNode.encode p.Age)
            ("Address", Encoders.encode p.Address)
            ("Sex", Encoders.encode p.Sex)
            ("Pet", Encoders.encode p.Pet)
            ("Cht", Encoders.encode p.Cht)
            ("Books", p.Books |> Seq.map JsonNode.encode |> JsonNode.encode)
        }
        |> JsonNode.encode

[<Extension>]
type Decoders =
    [<Extension>]
    static member AsAddressOrNone(jnode: JsonNode) = jnode.AsAddress() |> Some

    [<Extension>]
    static member AsAddressOrNone(jnode: JsonNode option) =
        match jnode with
        | Some prop -> prop.AsAddressOrNone()
        | None -> None

    [<Extension>]
    static member AsAddress(j: JsonNode) = { Street = j.Get("Street").AsString(); State' = j.Get("State").AsString() }

    [<Extension>]
    static member AsSex(j: JsonNode) =
        match j.Value with
        | JObject [| record |] -> 
            match record with
            | ("Female", _) -> Female
            | err -> failwith "Invalid value as Sex. Received {err}"
        | err -> failwith "Invalid value as Sex. Received {err}"

    [<Extension>]
    static member AsSexOrNone(j: JsonNode) = j.AsSex() |> Some

    [<Extension>]
    static member AsSexOrNone(j: JsonNode option) =
        match j with
        | Some node -> node.AsSexOrNone()
        | None -> None

    [<Extension>]
    static member AsPetOrNone(j: JsonNode) = j.AsPet() |> Some

    [<Extension>]
    static member AsPetOrNone(j: JsonNode option) =
        match j with
        | Some node -> node.AsPetOrNone()
        | None -> None

    [<Extension>]
    static member AsPet(j: JsonNode) =
        match (j.AsString()) with
        | "Cat" -> Cat
        | "Dog" -> Dog
        | err -> failwith "Invalid value as Pet. Received {err}"

    [<Extension>]
    static member AsCht(j: JsonNode) =
        match j.typ with
        | JBool v -> Choice1Of2 v
        | JNumber v -> Choice2Of2(int32 v)
        | _ -> failwith "Unexpected value"

    [<Extension>]
    static member AsListOfString(j: JsonNode) =
        match j.typ with
        | JArray v -> v |> Seq.map (fun v -> v.AsString()) |> Seq.toList
        | _ -> failwith "Unexpected value"

    [<Extension>]
    static member AsPayload(j: JsonNode) : Payload =
        {
            Kitchensink.Payload.Name = j.Get("Name").AsString()
            Kitchensink.Payload.Age = (j </> "Age").AsInt32()
            Kitchensink.Payload.Address = (j?Address).AsAddressOrNone()
            Kitchensink.Payload.Books = (j </> "Books").AsListOfString()
            Kitchensink.Payload.Bills = ("", "", 0)
            Kitchensink.Payload.Sex = (j </> "Sex").AsSex()
            Kitchensink.Payload.Pet = (j </> "Pet").AsPetOrNone()
            Kitchensink.Payload.Cht = (j </> "Cht").AsCht()
        }

    [<Extension>]
    static member AsPayloadOrNone(j: JsonNode) : Payload option =
        printfn "%A" j

        match j.typ with
        | JObject [||] -> None
        | JObject _ -> j.AsPayload() |> Some
        | JNull -> None
        | _ -> failwith "Unexpected value"

    [<Extension>]
    static member AsPayloadOrNone(jnode: JsonNode option) : Payload option =
        jnode |> Option.bind (fun node -> node.AsPayloadOrNone())

type Payload with

    static member toJson(p: Payload) : string =
        let encoder (p: Payload) = Encoders.encode p
        Json.serialize encoder p

    static member ofJson(jstr) : Payload = Json.parse Decoders.AsPayload jstr
