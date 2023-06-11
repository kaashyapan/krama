module Krama.CodePrinter.Json

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Types
open Fabulous.AST
open Fantomas.Core.SyntaxOak
open type Fabulous.AST.Ast
open Fantomas.FCS.Text

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

let mkjsonTemplate
  (jsonOuts: Json.Output)
  (config: Json.Config)
  (typs: T List)
  (typesToPrint: T list)
  =

  match jsonOuts.Namespace with
  | Some ns ->
    match jsonOuts.Module with
    | Some module_ ->

      Namespace(ns) {
        Open "System"
        Open "System.Runtime.CompilerServices"
        Open "Krama.Json"

        NestedModule(module_) {
          Krama.Json.Encoders.encoders jsonOuts config typs typesToPrint
          Krama.Json.Decoders.decoders jsonOuts config typs typesToPrint

          for widget in (Krama.Json.Ast.TypeAug.typeMethods jsonOuts config typs typesToPrint) do
            widget
        }
      }
      |> Tree.compile
      |> Fantomas.Core.CodeFormatter.FormatOakAsync
      |> Async.RunSynchronously
      |> Ok

    | None ->
      Namespace(ns) {
        Open "System"
        Open "System.Runtime.CompilerServices"
        Open "Krama.Json"
        Krama.Json.Encoders.encoders jsonOuts config typs typesToPrint
        Krama.Json.Decoders.decoders jsonOuts config typs typesToPrint

        for widget in (Krama.Json.Ast.TypeAug.typeMethods jsonOuts config typs typesToPrint) do
          widget
      }
      |> Tree.compile
      |> Fantomas.Core.CodeFormatter.FormatOakAsync
      |> Async.RunSynchronously
      |> Ok

  | None ->
    match jsonOuts.Module with
    | Some module_ ->
      Module(module_) {
        Open "System"
        Open "System.Runtime.CompilerServices"
        Open "Krama.Json"
        Krama.Json.Encoders.encoders jsonOuts config typs typesToPrint
        Krama.Json.Decoders.decoders jsonOuts config typs typesToPrint

        for widget in (Krama.Json.Ast.TypeAug.typeMethods jsonOuts config typs typesToPrint) do
          widget
      }
      |> Tree.compile
      |> Fantomas.Core.CodeFormatter.FormatOakAsync
      |> Async.RunSynchronously
      |> Ok

    | None ->
      log (Log.Err "Unspecified Namespace/Module")
      Error "Unspecified Namespace/Module"

//        log(Log.Err "Unspecified namespace/module")

let writeFile (opFileName: string) (byts) = File.WriteAllText(opFileName, byts)

let writeJson (config: Json.Config) (typs: T List) =
  log (Log.Info "Loading JSON serializer.. ")

  config.Outputs
  |> List.map (fun op ->

    log (Log.Info $"Looking up types that need printing to {op.File} ..")
    let typesToPrint = getTypeDependencies op.Includes typs

    match (mkjsonTemplate op config typs typesToPrint) with
    | Ok str ->
      log (Log.Info $"Printing {op.File} ..")
      writeFile op.File str
    | Error err -> ignore err
  )
  |> ignore
