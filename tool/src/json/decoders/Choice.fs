module Krama.Json.Decoders.Choice

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

let encode (tlist: T list) =
  EscapeHatch(expr)
