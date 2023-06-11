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

let serializeStmt (objName: string) =
  let objNameLower = objName.ToCamelCase()

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
              List.Empty,
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

      let clas =
        Augment(typeAccessPath, []) {
          (Method("toJson", [| $"({objNameLower}: {typeAccessPath})" |]) {
            EscapeHatch(serializeStmt objName)
          })
            .isStatic ()

          (Method("fromJson", [| "(jsonstr: string)" |]) { EscapeHatch(parseStmt objName) })
            .isStatic ()
        }

      clas
    )
    |> Seq.toList
  )
  |> List.concat
