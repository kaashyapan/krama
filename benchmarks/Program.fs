open JsonBenchmarks

open System
open BenchmarkDotNet.Running

[<EntryPoint>]
let main argv =

    BenchmarkRunner.Run typeof<JsonDeserialize> |> ignore

    BenchmarkRunner.Run typeof<JsonSerialize> |> ignore

    (*
    (JsonSerialize().FsharpLuDeserialization())
    |> printfn "%A"

    Sample.sampleString
    |> JsonBenchmarks.Deserializers.jay 
    |> printfn "%A" 

    0
    *)

    0
