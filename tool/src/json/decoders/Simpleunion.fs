module Krama.Json.Decoders.SimpleUnion

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

let mkDecode (name: string) (tlist: T list) =
    Match(
        Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("jnode.Value", Range.Zero))))
      ) {
        for unionMember in tlist do
          match unionMember with
          | T.UnionMember(name, _) ->
            MatchClause(
              "\"" + name + "\"",
              Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode(name, Range.Zero))))
            )
          | _ -> failwith "Invalid union member"
      }

let mkDecodeMaybe (name: string) (tlist: T list) =
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


let decode (name: string) (tlist: T list) =
  (mkDecode name tlist, mkDecodeMaybe name tlist) 
