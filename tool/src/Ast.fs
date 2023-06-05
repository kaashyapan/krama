module Krama.Ast

open Fantomas.Core
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open Fantomas.Core.SyntaxOak
open FSharpx

type Ctx = string list
let mutable ctx: Ctx = List.empty

let popCtx () =
  match ctx with
  | [ x ] -> ctx <- List.empty
  | _ :: tail -> ctx <- tail
  | _ -> ctx <- List.empty

let addCtx (moduleOrNs: string) = ctx <- moduleOrNs :: ctx

let rec findText (n: Node) =
  match n with
  | :? SingleTextNode as s -> s.Text
  | x -> x.Children |> Array.map findText |> String.concat "."

let genTypes (t: TypeDefn) =
  printfn "Typedefn %A" <| t.GetType()

  match t with
  | TypeDefn.Enum n -> printfn "%A" <| n.EnumCases.ToString()
  | TypeDefn.Union n -> printfn "%A" <| n.UnionCases.ToString()

  | TypeDefn.Record n ->
    n.Fields
    |> Seq.map (fun f ->
      f.Children
      |> Seq.map (fun x -> printfn "%A" <| x.ToString())
      |> Seq.toList
      |> ignore
    )
    |> ignore

  | TypeDefn.Abbrev n -> printfn "%A" <| n.Type.ToString()

  | TypeDefn.Regular n -> printfn "%A" <| n.GetType()
  | _ -> ignore 0

let rec genNestedModules (m: NestedModuleNode) =
  m.Identifier |> findText |> addCtx

  m.Declarations
  |> List.map (fun m ->
    match m with
    | ModuleDecl.NestedModule t -> genNestedModules t
    | ModuleDecl.TypeDefn t -> genTypes t
    | _ -> ignore 0
  )
  |> ignore

  popCtx ()

let genModules (oak: Oak) =
  oak.ModulesOrNamespaces
  |> List.collect (fun m ->
    m.Header.Value.Name.Value |> findText |> addCtx
    m.Declarations
  )
  |> List.map (fun m ->
    match m with
    | ModuleDecl.NestedModule t -> genNestedModules t
    | ModuleDecl.TypeDefn t -> genTypes t
    | _ -> ignore 0

    popCtx ()
  )
  |> ignore
