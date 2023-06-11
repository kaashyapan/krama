module Krama.Json.Decoders

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Types
open Fabulous.AST
open Fantomas.Core.SyntaxOak
open type Fabulous.AST.Ast
open Fantomas.FCS.Text
open CaseExtensions

let expr = Expr.Constant(Constant.FromText(SingleTextNode("23", Range.Zero)))

let exprObj (typ: T) =
  match typ with
  | T.Alias(name, typ) ->
    let objName = name.Split(".") |> Array.rev |> Array.head
    // {
    //     Name = j.Get("Name").AsString()
    //     Age = (j </> "Age").AsInt()
    //     Address = (j?Address).AsAddressOrNone()
    //     Sex = (j </> "Sex").AsSex()
    //     Pet = (j </> "Pet").AsPetOrNone()
    //     Cht = (j </> "Cht").AsCht()
    // }
    //
    EscapeHatch(expr)

  | _ -> EscapeHatch(expr)

let exprObjMaybe (typ: T) =
  printfn "%A" typ
  match typ with
  | T.Alias(name, typ) ->
    let objName = name.Split(".") |> Array.rev |> Array.head

    match typ with
    | T.Record tlist ->
      Match(
        Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("jnode.typ", Range.Zero))))
      ) {
        MatchClause(
          "JObject [||]",
          Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("None", Range.Zero))))
        )
        MatchClause(
          "JObject _",
          Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode($"jnode.As{objName}() |> Some", Range.Zero))))
        )
        MatchClause(
          "JNull",
          Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("None", Range.Zero))))
        )
        MatchClause(
          "_",
          Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("""failwith "Unexpected value" """, Range.Zero))))
        )
      }

    | _ -> EscapeHatch(expr)
  | _ -> EscapeHatch(expr)

let exprObjOption typName =
  Expr.Constant(
    Constant.FromText(
      SingleTextNode($"jnode |> Option.bind (fun node -> node.As{typName}OrNone())", Range.Zero)
    )
  )

let mkMember (t: T) =
  match t with
  | T.Alias(name, typ) ->
    let objName = name.Split(".") |> Array.rev |> Array.head

    [
      (Method($"as{objName}", [| $"(jnode: JsonNode)" |]) { exprObj t })
        .isStatic()
        .attributes ([ "Extension" ])

      (Method($"as{objName}OrNone", [| $"(jnode: JsonNode)" |]) { exprObjMaybe t })
        .isStatic()
        .attributes ([ "Extension" ])

      (Method($"as{objName}OrNone", [| $"(jnode: JsonNode option)" |]) {
        EscapeHatch(exprObjOption objName)
      })
        .isStatic()
        .attributes ([ "Extension" ])
    ]
  | _ -> [ (Method("Name", [| "()" |]) { EscapeHatch(expr) }).isStatic () ]

let decoders (jsonOuts: Json.Output) (config: Json.Config) (typs: T List) (typesToPrint: T list) =

  let members =
    typs
    |> List.where (fun t -> List.contains t typesToPrint)
    |> List.map mkMember
    |> List.concat

  Class("Decoders", []) {
    for m in members do
      m
  }
