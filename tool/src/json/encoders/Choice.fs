module Krama.Json.Encoders.Choice

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
  let tlistIndx = tlist |> List.mapi (fun i t -> (i, t))
  let tlistLength = List.length tlist

  Match(Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("value", Range.Zero))))) {
    for (idx, choice) in tlistIndx do
      let idx = idx + 1

      match choice with
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
        MatchClause(
          $"Choice{idx}Of{tlistLength} of v",
          Ast.EscapeHatch(
            Expr.Constant(Constant.FromText(SingleTextNode("JsonNode.encode v", Range.Zero)))
          )
        )

      | _ ->
        MatchClause(
          $"Choice{idx}Of{tlistLength} of v",
          Ast.EscapeHatch(
            Expr.Constant(Constant.FromText(SingleTextNode("Encoders.encode v", Range.Zero)))
          )
        )
  }
