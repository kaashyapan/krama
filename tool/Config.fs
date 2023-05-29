module Krama.Config

open Spectre.Console
open System

module Json =
    type KeyCase =
      | Camel
      | Pascal
      | Snake
      | AsIs

    type RemapType = 
        | Rename   

    type FieldName = string

    type FieldMap = Map<string, string>

    type Instruction = Map<RemapType, FieldMap>

    type InstructionList = Instruction list

    type Includes = Map<FieldName, InstructionList option> 
     
    type Config =
      {
        Version: int
        Case: KeyCase option
        Includes: List<Includes> option
        Excludes: List<string> option 
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
     
    type Config =
      {
        Version: int
        Includes: List<Includes> option
        Excludes: List<string> option 
      }

open Legivel.Serialization
open Legivel.Attributes

[<YamlField("Strategy")>]
type Config =
  | Json of Json.Config
  | Bare of Bare.Config
with
  static member ToYaml(yml: string) = Deserialize<Config> yml
