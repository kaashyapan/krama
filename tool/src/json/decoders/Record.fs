module Krama.Json.Decoders.Record

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
      let members (t: T) =
          match t with
          | T.RecordMember(n, t) ->
              RecordFieldNode(
                IdentListNode([IdentifierOrDot.Ident(SingleTextNode.Create(name + "." + n))], Range.Zero), 
                SingleTextNode.equals,
                expr,
                Range.Zero
              )
          | _ -> failwith "Invalid record member"
 
      let expr =
        Expr.Record(
          ExprRecordNode(
            SingleTextNode.leftCurlyBrace,
            None,
            tlist |> List.map members,
            SingleTextNode.rightCurlyBrace,
            Range.Zero
          )
        )
      EscapeHatch(expr)

let mkDecodeMaybe (name: string) (tlist: T list) =
      let objName = name.Replace(".", "")  

      Match(
        Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("jnode.Value", Range.Zero))))
      ) {
        MatchClause(
          "JObject [||]",
          Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("None", Range.Zero))))
        )

        MatchClause(
          "JObject _",
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
