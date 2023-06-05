module Krama.Exec

open System
open Krama.Console
open FSharp.SystemCommandLine
open System.IO

let handler (path: DirectoryInfo, yamlFile: IO.FileInfo option) =
  ignore <| Console.loadProj path yamlFile

[<EntryPoint>]
let main argv =
  rootCommand argv {
    description "Krama serializer/deserializer codegen for F#"

    inputs (
      Input.Argument<DirectoryInfo>("dir", "The directory containing the sln or fsproj file"),
      Input.OptionMaybe<IO.FileInfo>(
        [ "-f"; "--yaml-file" ],
        "The yaml configuration filename. Default: 'krama.yaml'"
      )
    )

    setHandler handler
  }
