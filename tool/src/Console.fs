module Krama.Console

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Compiler
open Krama.CodePrinter

type Args = { YamlFile: IO.FileInfo; Version: string }

type LoadConfigResult =
  | Valid of Config
  | Invalid of error: string
  | NotFound

/// Reads a config from yaml.
let tryLoadConfig (yamlFile: IO.FileInfo) =
  if yamlFile.Exists then
    try
      let yaml = IO.File.ReadAllText(yamlFile.FullName)

      match Config.ToYaml(yaml) with
      | Ok config -> Valid config
      | Error err -> Invalid err
    with ex ->
      Invalid ex.Message
  else
    NotFound

/// Validate config file and map to internal datastructures.
let getConfig (args: Args) =

  log (Log.Info $"Loading config {args.YamlFile}")

  match tryLoadConfig (args.YamlFile) with
  | Valid cfg -> Ok cfg
  | Invalid exMsg ->
    log (Log.Err $"Yaml error: {exMsg}")
    Error "Invalid yaml config."
  | NotFound ->
    log (Log.Err $"Did not find yaml config `{args.YamlFile.FullName}`")
    Error "Did not find yaml config."

/// Make a lookup table that will hold the config options.
let processOptions (yamlFile: FileInfo option) =
  let args: Args =
    {
      YamlFile = yamlFile |> Option.defaultWith (fun () -> FileInfo($"krama.yaml"))
      Version = Reflection.Assembly.GetAssembly(typeof<Args>).GetName().Version |> string
    }

  getConfig (args)

//////////////// dir processing //////////////////

let loadProj (fsProjFile: FileInfo) (yamlFile: IO.FileInfo option) =
  match fsProjFile.Exists with
  | true ->
    log (Log.Info $"Loading {fsProjFile.FullName}/*")
    let projectRoot = fsProjFile.Directory

    yamlFile
    |> processOptions
    |> Result.map (fun config ->
      let types = processFsFiles fsProjFile

      match config with
      | Json jsonConfig -> writeJson projectRoot jsonConfig types
      | Bare bareConfig -> writeBare projectRoot bareConfig types

      log (Log.Msg "Serializers have been generated")
    )
    |> ignore

  | false -> log (Log.Err $"{fsProjFile} not found.")
