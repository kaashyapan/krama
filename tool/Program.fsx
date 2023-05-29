#r "nuget: Legivel, 0.4.6"

open Legivel.Serialization
open Legivel.Attributes

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
 
type JsonConfig =
  {
    Version: int
    Case: KeyCase option
    Includes: List<Includes> option
    Excludes: List<string> option 
  }

type BareConfig =
  {
    Version: int
    Includes: List<Includes> option
    Excludes: List<string> option 
  }


[<YamlField("Strategy")>]
type Config =
  | Json of JsonConfig
  | Bare of BareConfig
  | Xml
  | Msgpack

//  example : http://www.yaml.org/spec/1.2/spec.html#id2760519
let yaml = """
Version: 1
Strategy: Bare
Case: Camel
Excludes:
  - Type1
Includes:
  - Type1: 
    - Rename:
        key: val
        key2: val2
"""

Deserialize<Config> yaml
|> printfn "%A"
