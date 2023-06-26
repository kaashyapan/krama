module Krama.Json.Encoders.AnonRecord

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Common
open Krama.Types
open Fabulous.AST
open Fantomas.Core.SyntaxOak
open type Fabulous.AST.Ast
open Fantomas.FCS.Text
open CaseExtensions
open Krama.Json

let encode (tlist: T list) =
  let members =
    tlist
    |> List.map (fun t ->
      match t with
      | T.RecordMember(name, typ) ->
        let rhs =
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
          | T.SByte -> $"JsonNode.encode value.{name}"
          | T.List t ->
            let aliasName = $"List_{getHash (t.ToString())}"
            T.Alias(aliasName, typ) |> GeneratedAlias.add
            $"Encoders.encode value.{name}"
          | T.Array t ->
            let aliasName = $"Array_{getHash (t.ToString())}"
            T.Alias(aliasName, typ) |> GeneratedAlias.add
            $"Encoders.encode value.{name}"
          | T.Seq t ->
            let aliasName = $"Seq_{getHash (t.ToString())}"
            T.Alias(aliasName, typ) |> GeneratedAlias.add
            $"Encoders.encode value.{name}"
          | T.Set t ->
            let aliasName = $"Set_{getHash (t.ToString())}"
            T.Alias(aliasName, typ) |> GeneratedAlias.add
            $"Encoders.encode value.{name}"
          | T.AnonRecord tlist ->
            let mutable parms =
              tlist
              |> List.map (fun t ->
                match t with
                | T.RecordMember(name, t') -> name + t'.ToString()
                | _ -> ""
              )
              |> FSharpx.Text.Strings.joinWords

            parms <- parms.Replace(" ", "")
            let aliasName = $"AnonRecord_{getHash (parms)}"
            T.Alias(aliasName, typ) |> GeneratedAlias.add
            $"Encoders.encode value.{name}"
          | T.Tuple tlist ->
            let mutable parms =
              tlist |> List.map (fun t -> (t.ToString())) |> FSharpx.Text.Strings.joinWords

            parms <- parms.Replace(" ", "")
            let aliasName = $"Tuple_{getHash (parms)}"
            T.Alias(aliasName, typ) |> GeneratedAlias.add
            $"Encoders.encode value.{name}"
          | _ -> $"Encoders.encode value.{name}"

        [
          Choice2Of2(SingleTextNode("\"" + name + "\"", Range.Zero))
          Choice2Of2(SingleTextNode.comma)
          Choice2Of2(SingleTextNode(rhs, Range.Zero))
        ]
        |> Some
      | _ -> None
    )
    |> List.where Option.isSome
    |> List.map (fun x ->

      ComputationExpressionStatement.OtherStatement(
        Expr.Paren(
          ExprParenNode(
            SingleTextNode.leftParenthesis,
            Expr.Tuple(ExprTupleNode(x.Value, Range.Zero)),
            SingleTextNode.rightParenthesis,
            Range.Zero
          )
        )
      )
    )

  let lhs = ExprCompExprBodyNode(members, Range.Zero)

  let expr =
    ExprInfixAppNode(
      Expr.NamedComputation(
        ExprNamedComputationNode(
          Expr.Constant(Constant.FromText(SingleTextNode("seq", Range.Zero))),
          SingleTextNode.leftCurlyBrace,
          Expr.CompExprBody lhs,
          SingleTextNode.rightCurlyBrace,
          Range.Zero
        )
      ),
      SingleTextNode.pipeRight,
      Expr.Constant(Constant.FromText(SingleTextNode("JsonNode.encode", Range.Zero))),
      Range.Zero
    )

  EscapeHatch(Expr.InfixApp expr)
