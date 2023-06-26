module Krama.Json.Encoder

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

let exprObj (typ: T) =
  match typ with
  | T.Alias(name, typ) ->
    let objName = name.Split(".") |> Array.rev |> Array.head

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
    | T.SByte ->
      Expr.Constant(Constant.FromText(SingleTextNode("JsonNode.encode value", Range.Zero)))
      |> Ast.EscapeHatch

    | T.UnionSimple tlist -> Krama.Json.Encoders.SimpleUnion.encode tlist
    | T.Union tlist -> Krama.Json.Encoders.Union.encode tlist
    | T.Record tlist -> Krama.Json.Encoders.Record.encode tlist
    | T.Choice tlist -> Krama.Json.Encoders.Choice.encode tlist
    | T.Tuple tlist -> Krama.Json.Encoders.Tuple.encode tlist
    | T.List t -> Krama.Json.Encoders.List.encode t
    | T.AnonRecord tlist -> Krama.Json.Encoders.AnonRecord.encode tlist
    | t ->
      printf "One %A" t
      //log(Log.Err (sprintf "Encoder does not handle - %A" t))
      EscapeHatch(expr)

  | t ->
    printf "Two %A" t
    //log(Log.Err "Encoder does not handle - " )
    EscapeHatch(expr)

let exprObjMaybe (typ: T) =
  match typ with
  | T.Alias(name, typ) ->
    let objName = name.Split(".") |> Array.rev |> Array.head
    let objNameLower = objName.ToCamelCase()

    Match(Ast.EscapeHatch(Expr.Constant(Constant.FromText(SingleTextNode("value", Range.Zero))))) {
      MatchClause(
        $"Some {objNameLower}",
        Ast.EscapeHatch(
          Expr.Constant(
            Constant.FromText(SingleTextNode($"Encoders.encode {objNameLower}", Range.Zero))
          )
        )
      )

      MatchClause(
        "None",
        Ast.EscapeHatch(
          Expr.Constant(Constant.FromText(SingleTextNode("JsonNode.encode ()", Range.Zero)))
        )
      )
    }

  | _ -> EscapeHatch(expr)

let mkMember (t: T) =
  match t with
  | T.Alias(name, typ) ->
    let namePattern = PatternWithCurriedParameters([ ("value", Some(CommonType.mkType (name))) ])

    let nameOptionPattern =
      PatternWithCurriedParameters([ ("value", Some(CommonType.mkType (name + " option"))) ])

    [
      (StaticMethodMember("encode", [ namePattern ]) { exprObj t })
        .returnType (CommonType.mkType ("JsonNode"))
      (StaticMethodMember("encode", [ nameOptionPattern ]) { exprObjMaybe t })
        .returnType (CommonType.mkType ("JsonNode"))

    ]
  | _ -> [ (StaticMethodMember("Name", []) { EscapeHatch(expr) })]

let encoders (jsonOuts: Json.Output) (config: Json.Config) (typs: T List) (typesToPrint: T list) =
  let members =
    typs
    |> List.where (fun t -> List.contains t typesToPrint)
    |> List.map mkMember
    |> List.concat

  let generatedmembers =
    ()
    |> Krama.Json.GeneratedAlias.list
    |> Set.toList
    |> List.map mkMember
    |> List.concat

  let generatedTypes =
    ()
    |> Krama.Json.GeneratedAlias.list
    |> Set.toList
    |> List.map (fun t ->
      match t with
      | T.Alias(name, typ) -> Abbrev(name, CommonType.mkType (stringifyType typ))
      | _ -> failwith "Invalid generated type"
    )

  (generatedTypes, generatedmembers, members)
