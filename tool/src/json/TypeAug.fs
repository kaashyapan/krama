module Krama.Json.Ast.TypeAug

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Types
open Fabulous.AST
open Fantomas.Core.SyntaxOak
open type Fabulous.AST.Ast
open Fantomas.FCS.Text
open FSharpx
open CaseExtensions

let serializeStmt (typeAccessPath: string) =
  let objName = typeAccessPath.Split(".") |> Array.rev |> Array.head
  let objNameLower = objName.ToCamelCase()

  let parameters =
    [ PatternWithCurriedParameters([ (objNameLower, Some(CommonType.mkType (typeAccessPath))) ]) ]

  Expr.CompExprBody(
    ExprCompExprBodyNode(
      [
        ComputationExpressionStatement.LetOrUseStatement(
          ExprLetOrUseNode(
            BindingNode(
              None,
              None,
              MultipleTextsNode([ SingleTextNode("let", Range.Zero) ], Range.Zero),
              false,
              None,
              None,
              Choice1Of2(
                IdentListNode(
                  [ IdentifierOrDot.Ident(SingleTextNode("encoder", Range.Zero)) ],
                  Range.Zero
                )
              ),
              None,
              parameters,
              None,
              SingleTextNode("=", Range.Zero),
              Expr.Constant(
                Constant.FromText(SingleTextNode($"Encoders.encode {objNameLower}", Range.Zero))
              ),
              Range.Zero
            ),
            None,
            Range.Zero
          )
        )
        ComputationExpressionStatement.OtherStatement(
          Expr.Constant(
            Constant.FromText(SingleTextNode($"Json.serialize encoder {objNameLower}", Range.Zero))
          )
        )
      ],
      Range.Zero
    )
  )

let parseStmt (objName: string) =
  Expr.Constant(
    Constant.FromText(SingleTextNode($"Json.parse Decoders.As{objName} jsonstr", Range.Zero))
  )

let typeMethods
  (jsonOuts: Json.Output)
  (config: Json.Config)
  (typs: T List)
  (typesToPrint: T list)
  =

  jsonOuts.Includes
  |> List.map (fun incl ->
    incl
    |> Seq.map (fun kv ->
      let typeAccessPath = kv.Key
      let objName = typeAccessPath.Split(".") |> Array.rev |> Array.head
      let objNameLower = objName.ToCamelCase()

      let toJsonPattern =
        PatternWithCurriedParameters([ (objNameLower, Some(CommonType.mkType (typeAccessPath))) ])

      let fromJsonPattern = PatternWithCurriedParameters([ ("jsonstr", Some(CommonType.String)) ])

      let clas =
        Augment(typeAccessPath, []) {
          (StaticMethodMember("ToJson", [ toJsonPattern ]) {
            EscapeHatch(serializeStmt typeAccessPath)
          })
            .returnType (CommonType.String)

          (StaticMethodMember("FromJson", [ fromJsonPattern ]) { EscapeHatch(parseStmt objName) })
            .returnType (CommonType.mkType (typeAccessPath))
        }

      clas
    )
    |> Seq.toList
  )
  |> List.concat
