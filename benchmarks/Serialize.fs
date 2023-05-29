namespace JsonBenchmarks

open System
open BenchmarkDotNet.Attributes
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.FSharpLu.Json
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Krama.Json

module Serializers =
    type FsharpLuSettings =
        static member settings =
            let s =
                JsonSerializerSettings(
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                )

            s.Converters.Add(CompactUnionJsonConverter())
            s

        static member formatting = Formatting.None

    type FsharpLuSerializer = With<FsharpLuSettings>

    let systemTextOptions =
        let options = JsonSerializerOptions()
        options.Converters.Add(JsonFSharpConverter())
        options

    let systemText (payload: Payload) = System.Text.Json.JsonSerializer.Serialize(payload, systemTextOptions)

    let fsharplu (payload: Payload) = FsharpLuSerializer.serialize (payload)

    let kramaJson (payload: Payload) = payload |> Payload.toJson

    let newtonsoft (payload: Payload) = JsonConvert.SerializeObject(payload, new OptionConverter())

    let thoth (payload: Payload) = payload |> ThothEncoders.toJsonPayload |> Thoth.Json.Net.Encode.toString 0

[<MemoryDiagnoser>]
type JsonSerialize() =
    let payload = Deserializers.kramaJson Sample.sampleString
    let bareDto = BareHelpers.payloadToBare payload

    [<Benchmark>]
    member self.NewtonsoftSerialization() = Serializers.newtonsoft payload

    [<Benchmark>]
    member self.FsharpLuSerialization() = Serializers.fsharplu payload

    [<Benchmark>]
    member self.SystemTextSerialization() = Serializers.systemText payload

    [<Benchmark>]
    member self.ThothNetSerialization() = Serializers.thoth payload

    [<Benchmark>]
    member self.KramaJsonSerialization() = Serializers.kramaJson payload

    [<Benchmark>]
    member self.BareSerialization() = bareDto |> Bare.Msg.Encoding.ofBPayload |> Result.toOption |> Option.get
