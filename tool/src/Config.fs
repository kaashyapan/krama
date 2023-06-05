module Krama.Config

open Spectre.Console
open System
open Krama.Log

module Json =
  type KeyCase =
    | Camel
    | Pascal
    | Snake
    | AsIs

  type TypePath = string

  type FieldMap = Map<string, string>

  type Includes = Map<TypePath, FieldMap option>

  type Output =
    {
      File: string
      Namespace: string option
      Module: string option
      Includes: List<Includes>
      Excludes: List<string> option
    }

  type Config =
    {
      Version: int
      Case: KeyCase option
      Outputs: Output list
      FloatAsString: bool option
      DateAsFloat: bool option
    }

module Bare =
  type RemapType =
    | Index
    | Length

  type FieldName = string

  type FieldMap = Map<string, int>

  type Instruction = Map<RemapType, FieldMap>

  type InstructionList = Instruction list

  type Includes = Map<FieldName, InstructionList option>

  type Config = { Version: int; Includes: List<Includes> option; Excludes: List<string> option }

open Legivel.Serialization
open Legivel.Attributes

[<YamlField("Strategy")>]
type Config =
  | Json of Json.Config
  | Bare of Bare.Config

  static member ToYaml(yml: string) =
    let legivalToResult (results: DeserializeResult<Config> list) : Result<Config, string> =
      results
      |> List.fold
        (fun s r ->
          match r with
          | Success { Data = d } -> Ok d
          | Error { Error = [ { Location = loc; Message = msg } ] } ->
            Result.Error(sprintf "%s. Location: %s" msg loc.AsString)
          | x -> Result.Error "Error parsing yaml"
        )
        (Result.Error "Error parsing yaml")

    yml |> Deserialize<Config> |> legivalToResult
