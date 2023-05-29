namespace JsonBenchmarks

open System
open System.Runtime.CompilerServices
open Krama.Json

module Jtest =
    type Payload =
        {
            foo: string
            bar: string
        }

        static member AsPayload(j: JsonNode) : Payload =
            { foo = (j </> "foo").AsString(); bar = (j </> "bar").AsString() }

        static member toJson(j: Payload) =
            [| ("foo", JsonNode.encode j.foo); ("bar", JsonNode.encode j.bar) |]
            |> JsonNode.encode

    let serializer () = { foo = "hello"; bar = "world" } |> Json.serialize Payload.toJson

    let deserializer () =
        let str = """{"foo":"hello","bar":"world"}"""
        Json.parse Payload.AsPayload str

    let serde (s: string) : string =
        let payload = Json.parse Payload.AsPayload s
        let response = Json.serialize Payload.toJson payload
        response
