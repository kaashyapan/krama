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

  log (Log.Info $"Inspecting configuration {args.YamlFile}")

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
  |> Result.map (fun config ->
    Directory.SetCurrentDirectory(args.YamlFile.Directory.FullName)
    config
  )

//////////////// dir processing //////////////////

let getProjFile (config: Config) =

  match config with
  | Json jsonConfig -> jsonConfig.FsProjFile
  | Bare bareConfig -> bareConfig.FsProjFile
  |> (fun file ->
    let projfile = FileInfo(file)

    match projfile.Exists with
    | true ->
      log (Log.Info $"Loading {projfile.FullName}")
      Ok projfile
    | false ->
      log (Log.Err $"{projfile} not found.")
      Error $"{projfile} not found."
  )

let loadProj (yamlFile: IO.FileInfo option) =
  yamlFile
  |> processOptions
  |> Result.map (fun config ->
    config
    |> getProjFile
    |> Result.map (fun fsProjFile ->
      Directory.SetCurrentDirectory(fsProjFile.Directory.FullName)
      let types = processFsFiles fsProjFile

      match config with
      | Json jsonConfig -> writeJson jsonConfig types
      | Bare bareConfig -> writeBare bareConfig types

      log (Log.Msg "Serializers have been generated")
    )
  )
  |> ignore
