module Krama.Console

open Spectre.Console
open System
open Krama.Config

type Args = { YamlFile: IO.FileInfo; Version: string }

type LoadConfigResult =
  | Valid of string
  | Invalid of error: string
  | NotFound

/// Reads a config from yaml.
let tryLoadConfig (yamlFile: IO.FileInfo) =
  if yamlFile.Exists then
    try
      let yaml = IO.File.ReadAllText(yamlFile.FullName)
      let config = Config.ToYaml (yaml)
      Valid yaml
    with ex ->
      Invalid ex.Message
  else
    NotFound

/// Creates a sqlhydra-*.yaml file if necessary.
let getOrCreateConfig (args: Args) =
  AnsiConsole.MarkupLine($"[blue]-[/] v[yellow]{args.Version}[/]")

  match tryLoadConfig (args.YamlFile) with
  | Valid cfg -> cfg
  | Invalid exMsg ->
      AnsiConsole.MarkupLine($"[red]-[/] `{args.YamlFile.FullName}` . \n{exMsg}")
      "Invalid yaml config."
  | NotFound -> 
      AnsiConsole.MarkupLine($"[red]-[/] Did not find yaml config `{args.YamlFile.FullName}`\n")
      "Did not find yaml config."

/// Runs code generation for a given database provider.
let run (args: Args) =
  let cfg = getOrCreateConfig (args)
  printfn "%A" cfg
  AnsiConsole.MarkupLine($"[green]-[/] Serializers have been generated!")
