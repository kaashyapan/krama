module Krama.Json.Encoders.List

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

let encode (t: T) =
  (match t with
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
     Expr.Constant(
       Constant.FromText(
         SingleTextNode("value |> Seq.map JsonNode.encode |> JsonNode.encode", Range.Zero)
       )
     )
   | _ ->
     Expr.Constant(
       Constant.FromText(
         SingleTextNode("value |> Seq.map Encoders.encode |> JsonNode.encode", Range.Zero)
       )
     ))
  |> EscapeHatch
