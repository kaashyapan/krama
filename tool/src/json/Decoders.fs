module Krama.Json.Decoder

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

let exprObjOption typName =
  Expr.Constant(
    Constant.FromText(
      SingleTextNode($"jnode |> Option.bind (fun node -> node.As{typName}OrNone())", Range.Zero)
    )
  )

let mkMember (t: T) =
  match t with
  | T.Alias(name, typ) ->
    let objName = name.Replace(".", "")  

    let parameters =
      PatternWithCurriedParameters([ ("jnode", Some(CommonType.mkType ("JsonNode"))) ])

    let parametersOrNone =
      PatternWithCurriedParameters([ ("jnode", Some(CommonType.mkType ("JsonNode option"))) ])

    let exprObj, exprObjMaybe = 
      match typ with
      | T.Char
      | T.String
      | T.Int8
      | T.Int16
      | T.Int32 
      | T.Int64
      | T.Int128
      | T.UInt8
      | T.UInt16
      | T.UInt32
      | T.UInt64
      | T.UInt128
      | T.Half
      | T.Single
      | T.Double
      | T.Guid
      | T.Bool
      | T.Decimal
      | T.Byte
      | T.SByte ->
          EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode.Create("jnode.AsInt32()")))),
          Match(
            EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode.Create("jnode.Value"))))
          ) {
            MatchClause(
              "JNull",
              EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode.Create("None"))))
            )
            MatchClause(
              "JNumber _",
              EscapeHatch(
                Expr.Constant(
                  Constant.FromText(SingleTextNode.Create($"jnode.As{objName}() |> Some"))
                )
              )
            )
            MatchClause(
              "_",
              EscapeHatch(
                Expr.Constant(
                  Constant.FromText(SingleTextNode("""failwith "Unexpected value" """, Range.Zero))
                )
              )
            )
          }


      | T.UnionSimple tlist -> Krama.Json.Decoders.SimpleUnion.decode name tlist
      | T.Record tlist -> Krama.Json.Decoders.Record.decode name tlist
      | T.List t -> Krama.Json.Decoders.List.decode name t
      | _ -> EscapeHatch(expr), EscapeHatch(expr)


    [
      (StaticMethodMember($"As{objName}", [ parameters ]) { exprObj })
        .attributes([ "Extension" ])
      (StaticMethodMember($"As{objName}OrNone", [ parameters ]) { exprObjMaybe })
        .attributes([ "Extension" ])
      (StaticMethodMember($"As{objName}OrNone", [ parametersOrNone ]) {
        EscapeHatch(exprObjOption objName)
      })
        .attributes([ "Extension" ])
    ]
  | _ -> [ (StaticMethodMember("Name", []) { EscapeHatch(expr) }) ]

let decoders (jsonOuts: Json.Output) (config: Json.Config) (typs: T List) (typesToPrint: T list) =

  let members =
    typs
    |> List.where (fun t -> List.contains t typesToPrint)
    |> List.map mkMember
    |> List.concat

  Class("Decoders") {
    for m in members do
      m
  }
