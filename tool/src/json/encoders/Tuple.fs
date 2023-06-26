module Krama.Json.Encoders.Tuple

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

let encode (tlist: T list) =
  let mkItem idx t =
    match t with
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
      Expr.Constant(Constant.FromText(SingleTextNode($"JsonNode.encode v{idx}", Range.Zero)))
    | _ -> Expr.Constant(Constant.FromText(SingleTextNode($"Encoders.encode v{idx}", Range.Zero)))

  let nodes = tlist |> List.mapi mkItem

  let items =
    tlist
    |> List.mapi (fun idx _ -> IdentifierOrDot.Ident(SingleTextNode($"v{idx}", Range.Zero)))
    |> List.intersperse (SingleTextNode.comma |> IdentifierOrDot.Ident)

  let lhs =
    Expr.ArrayOrList(
      ExprArrayOrListNode(
        SingleTextNode.leftBracket,
        nodes,
        SingleTextNode.rightBracket,
        Range.Zero
      )
    )

  Expr.CompExprBody(
    ExprCompExprBodyNode(
      [
        ComputationExpressionStatement.LetOrUseStatement(
          ExprLetOrUseNode(
            BindingNode(
              None,
              None,
              MultipleTextsNode([ SingleTextNode.Create("let") ], Range.Zero),
              false,
              None,
              None,
              Choice1Of2(IdentListNode(items, Range.Zero)),
              None,
              [],
              None,
              SingleTextNode.equals,
              Expr.Constant(Constant.FromText(SingleTextNode.Create("value"))),
              Range.Zero
            ),
            None,
            Range.Zero
          )
        )
        ComputationExpressionStatement.OtherStatement(
          Expr.InfixApp(
            ExprInfixAppNode(
              lhs,
              SingleTextNode.pipeRight,
              Expr.Constant(Constant.FromText(SingleTextNode.Create("JsonNode.encode"))),
              Range.Zero
            )
          )
        )
      ],
      Range.Zero
    )
  )
  |> EscapeHatch
