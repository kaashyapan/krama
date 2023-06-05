module Krama.CodePrinter

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Types

let writeJson (config: Json.Config) (typs: T List) =
  config.Outputs
  |> List.map (fun op ->
    op.Includes
    |> List.map (fun incls ->
      incls
      |> Map.keys
      |> Seq.toList
      |> List.map (fun typeName -> typeName |> getTypeHeirarchy typs)
      |> List.concat
    )
    |> List.concat
    |> List.distinct
    |> List.where (fun t -> t <> T.TypeNotSupported)
    |> (fun x -> printfn "%A" x ; x)// write to file
  )
  |> ignore

let writeBare (config: Bare.Config) (typs: T List) =
  printfn "%A" config
  printfn "%A" typs
