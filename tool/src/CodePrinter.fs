module Krama.CodePrinter

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Types
open FSharpx

let getTypeDependencies (op: Json.Includes list) (typs: T List) =
  op
  |> List.map (fun incls ->
    incls
    |> Map.keys
    |> Seq.toList
    |> List.map (fun typeName -> typeName |> getTypeHeirarchy typs |> List.distinct)
    |> List.concat
  )
  |> List.concat
  |> List.distinct

let mkjsonTemplate (config: Json.Config) (typs: T List) (typesToPrint: T list) =
  typs
  |> List.fold (fun acc t -> if List.contains t typesToPrint then t.ToString() :: acc else acc) []
  |> String.concat "\n"

let writeFile (opFileName: string) (byts) = File.WriteAllText(opFileName, byts)

let writeJson (config: Json.Config) (typs: T List) =
  log (Log.Info "Loading JSON serializer.. ")
  config.Outputs
  |> List.map (fun op ->

    log (Log.Info $"Looking up types that need printing to {op.File} ..")
    let typesToPrint = getTypeDependencies op.Includes typs
    let stringToPrint = mkjsonTemplate config typs typesToPrint
    log (Log.Info $"Printing {op.File} ..")
    writeFile op.File stringToPrint
  )
  |> ignore

let writeBare (config: Bare.Config) (typs: T List) =
  printfn "%A" config
  printfn "%A" typs
