module Krama.Json.Encoders

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Types
open Fabulous.AST
open Fantomas.Core.SyntaxOak
open type Fabulous.AST.Ast
open Fantomas.FCS.Text

let expr = Expr.Constant(Constant.FromText(SingleTextNode("23", Range.Zero)))

let mkMember (t: T) =
  match t with
  | T.Alias(name, typ) ->
    [
      (Method("encode", [| $"(value: {name})" |]) { EscapeHatch(expr) }).isStatic ()
      (Method("encode", [| $"(value: {name} option)" |]) { EscapeHatch(expr) }).isStatic ()
    ]
  | _ -> [ (Method("Name", [| "()" |]) { EscapeHatch(expr) }).isStatic () ]

let encoders (jsonOuts: Json.Output) (config: Json.Config) (typs: T List) (typesToPrint: T list) =

  let members =
    typs
    |> List.where (fun t -> List.contains t typesToPrint)
    |> List.map mkMember
    |> List.concat

  Class("Encoders", []) {
    for m in members do
      m
  }
