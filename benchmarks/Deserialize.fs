namespace JsonBenchmarks

open System
open BenchmarkDotNet.Attributes
open System.Text.Json
open System.Text.Json.Serialization
open Microsoft.FSharpLu.Json
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Krama.Json

module Deserializers =
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

    let systemText (jsonString: string) =
        System.Text.Json.JsonSerializer.Deserialize<Payload>(jsonString, systemTextOptions)

    let fsharplu (jsonString: string) = FsharpLuSerializer.deserialize<Payload> (jsonString)

    let kramaJson (jsonString: string) = jsonString |> Payload.fromJson

    let newtonsoft (jsonString: string) = JsonConvert.DeserializeObject<Payload>(jsonString, new OptionConverter())

    let thoth (jsonString: string) =
        (Thoth.Json.Net.Decode.fromString ThothDecoders.payloadDecoder jsonString)
        |> function
            | Ok p -> p
            | Error e -> failwith e

[<MemoryDiagnoser>]
type JsonDeserialize() =
    let payload = Deserializers.kramaJson Sample.sampleString

    let bareEncodedBytes =
        payload
        |> BareHelpers.payloadToBare
        |> Bare.Msg.Encoding.ofBPayload
        |> Result.toOption
        |> Option.get

    [<Benchmark>]
    member self.NewtonsoftDeserialization() = Deserializers.newtonsoft Sample.sampleString

    [<Benchmark>]
    member self.FsharpLuDeserialization() = Deserializers.fsharplu Sample.sampleString

    [<Benchmark>]
    member self.SystemTextDeserialization() = Deserializers.systemText Sample.sampleString

    [<Benchmark>]
    member self.ThothNetDeserialization() = Deserializers.thoth Sample.sampleString

    [<Benchmark>]
    member self.KramaJsonDeserialization() = Deserializers.kramaJson Sample.sampleString

    [<Benchmark>]
    member self.BareDeserialization() =
        bareEncodedBytes
        |> Bare.Msg.Encoding.toBPayload
        |> Result.toOption
        |> Option.get
