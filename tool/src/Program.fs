module Krama.Exec

open System
open Krama.Console
open FSharp.SystemCommandLine
open System.IO

let handler (yamlFile: IO.FileInfo option) = ignore <| Console.loadProj yamlFile

[<EntryPoint>]
let main argv =
  rootCommand argv {
    description "Krama serializer/deserializer codegen for F#"

    inputs (
      Input.OptionMaybe<IO.FileInfo>(
        [ "-f"; "--yaml-file" ],
        "The yaml configuration filename. Default: './krama.yaml'"
      )
    )

    setHandler handler
  }
