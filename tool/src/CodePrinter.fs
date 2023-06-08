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
    |> List.map (fun typeName -> typeName |> getTypeHeirarchy typs)
    |> List.concat
  )
  |> List.concat
  |> List.where (fun t -> t <> T.TypeNotSupported)
  |> List.distinct
  |> List.map (fun t ->
    match t with
    | T.Userdef str -> str
    | _ -> ""
  )
  |> List.where (not << String.IsNullOrWhiteSpace)

let mkjsonTemplate (config: Json.Config) (typs: T List) (typesToPrint: string list) =
  typs
  |> List.fold
    (fun acc t ->
      match t with
      | T.Userdef str ->
        if (List.contains str typesToPrint) then List.append acc [ (t.ToString()) ] else acc
      | _ -> acc
    )
    []
  |> String.concat "\n"

let writeFile (opFileName: string) (byts) = File.WriteAllText(opFileName, byts)

let writeJson (projectRoot: DirectoryInfo) (config: Json.Config) (typs: T List) =
  config.Outputs
  |> List.map (fun op ->
    let typesToPrint = getTypeDependencies op.Includes typs
    let stringToPrint = mkjsonTemplate config typs typesToPrint
    let fileToPrint = sprintf "%s/%s" projectRoot.FullName op.File
    writeFile fileToPrint stringToPrint
  )
  |> ignore

let writeBare (projectRoot: DirectoryInfo) (config: Bare.Config) (typs: T List) =
  printfn "%A" config
  printfn "%A" typs
