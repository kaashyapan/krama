module Krama.Exec

open System
open Krama.Console
open FSharp.SystemCommandLine

let handler (yamlFile: IO.FileInfo option) =

  let args: Console.Args =
    {
      YamlFile = yamlFile |> Option.defaultWith (fun () -> IO.FileInfo($"krama.yaml"))
      Version =
        Reflection.Assembly.GetAssembly(typeof<Console.Args>).GetName().Version
        |> string
    }

  Console.run args

[<EntryPoint>]
let main argv =
  rootCommand argv {
    description "Krama"

    inputs (
      Input.OptionMaybe<IO.FileInfo>([ "-f"; "--yaml-file" ], "The yaml configuration filename. Default: 'krama.yaml'")
    )

    setHandler handler
  }
