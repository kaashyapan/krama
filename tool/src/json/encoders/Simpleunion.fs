module Krama.Json.Encoders.SimpleUnion

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
  Match(Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("value", Range.Zero))))) {
    for unionMember in tlist do
      match unionMember with
      | T.UnionMember(name, _) ->
        MatchClause(
          name,
          Ast.EscapeHatch(
            Expr.Constant(Constant.FromText(SingleTextNode("JsonNode.encode value", Range.Zero)))
          )
        )
      | _ -> failwith "Invalid union member"
  }
