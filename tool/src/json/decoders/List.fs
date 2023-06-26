module Krama.Json.Decoders.List

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Types
open Krama.Common
open Fabulous.AST
open Fantomas.Core.SyntaxOak
open type Fabulous.AST.Ast
open Fantomas.FCS.Text
open CaseExtensions

let mkDecode (name: string) (t: T) =
    Match(
        Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("jnode.Value", Range.Zero))))
      ) {
            MatchClause(
              "JArray v",
              Ast.EscapeHatch(
                Expr.Constant(
                  Constant.FromText(
                    SingleTextNode.Create("v |> Seq.map (fun v -> v.AsString()) |> Seq.toList")
                  )
                )
              )
            )
            MatchClause(
              "_",
              Ast.EscapeHatch(
                Expr.Constant(
                  Constant.FromText(
                    SingleTextNode.Create("failwith \"Unexpected value. Expecting a list\"")
                  )
                )
              )
            )

      }

let mkDecodeMaybe (name: string) (t: T) =
      let objName = name.Replace(".", "")  
      Match(
        Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("jnode.Value", Range.Zero))))
      ) {
        MatchClause(
          "JString _",
          Ast.EscapeHatch(
            Expr.Constant(
              Constant.FromText(SingleTextNode($"jnode.As{objName}() |> Some", Range.Zero))
            )
          )
        )

        MatchClause(
          "JNull",
          Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("None", Range.Zero))))
        )

        MatchClause(
          "_",
          Ast.EscapeHatch(
            Expr.Constant(
              Constant.FromText(SingleTextNode("""failwith "Unexpected value" """, Range.Zero))
            )
          )
        )
      }

let decode (name: string) (t: T) =
  (mkDecode name t, mkDecodeMaybe name t) 
