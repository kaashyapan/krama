module Krama.Json.Encoders.Union

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
  Match(Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("value", Range.Zero))))) {
    for unionMember in tlist do
      match unionMember with
      | T.UnionMember(name, tlist) ->
        printfn "Member %A" name
        printfn "Member typ %A" tlist

        let members =
          tlist
          |> List.map (fun typ ->
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
              | T.SByte -> $"JsonNode.encode v"
              | T.List t ->
                let aliasName = $"List_{getHash (t.ToString())}"
                T.Alias(aliasName, typ) |> GeneratedAlias.add
                "Encoders.encode v"
              | T.Array t ->
                let aliasName = $"Array_{getHash (t.ToString())}"
                T.Alias(aliasName, typ) |> GeneratedAlias.add
                "Encoders.encode v"
              | T.Seq t ->
                let aliasName = $"Seq_{getHash (t.ToString())}"
                T.Alias(aliasName, typ) |> GeneratedAlias.add
                "Encoders.encode v"
              | T.Set t ->
                let aliasName = $"Set_{getHash (t.ToString())}"
                T.Alias(aliasName, typ) |> GeneratedAlias.add
                "Encoders.encode v"
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
                "Encoders.encode v"
              | T.Tuple tlist ->
                let mutable parms =
                  tlist |> List.map (fun t -> (t.ToString())) |> FSharpx.Text.Strings.joinWords

                parms <- parms.Replace(" ", "")
                let aliasName = $"Tuple_{parms}"
                T.Alias(aliasName, typ) |> GeneratedAlias.add
                $"Encoders.encode v"
              | _ -> $"Encoders.encode v"

            (name, rhs)
          )
          |> List.map (fun (name, rhs) ->
            ComputationExpressionStatement.OtherStatement(
              Expr.Paren(
                ExprParenNode(
                  SingleTextNode.leftParenthesis,
                  Expr.Tuple(
                    ExprTupleNode(
                      [
                        Choice2Of2(SingleTextNode($"\"{name}\"", Range.Zero))
                        Choice2Of2(SingleTextNode.comma)
                        Choice2Of2(SingleTextNode(rhs, Range.Zero))
                      ],
                      Range.Zero
                    )
                  ),
                  SingleTextNode.rightParenthesis,
                  Range.Zero
                )
              )
            )
          )

        let lhs =
          match members with
          | [] ->
            let members =
              [
                ComputationExpressionStatement.OtherStatement(
                  Expr.Paren(
                    ExprParenNode(
                      SingleTextNode.leftParenthesis,
                      Expr.Tuple(
                        ExprTupleNode(
                          [
                            Choice2Of2(SingleTextNode($"\"{name}\"", Range.Zero))
                            Choice2Of2(SingleTextNode.comma)
                            Choice2Of2(SingleTextNode("JsonNode.encode ()", Range.Zero))
                          ],
                          Range.Zero
                        )
                      ),
                      SingleTextNode.rightParenthesis,
                      Range.Zero
                    )
                  )
                )
              ]

            ExprCompExprBodyNode(members, Range.Zero)

          | _ -> ExprCompExprBodyNode(members, Range.Zero)

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

        match members with
        | [] -> MatchClause($"{name}", EscapeHatch(Expr.InfixApp expr))
        | _ -> MatchClause($"{name} v", EscapeHatch(Expr.InfixApp expr))

      | t -> failwith (sprintf "Invalid union member %A" (t.ToString()))

  }
