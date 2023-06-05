module Krama.Log

open Spectre.Console

type Log =
  | Err of string
  | Info of string
  | Msg of string

type A private () =
  static let instance = A()
  static member Instance = instance

  member this.Action(logType: Log) =
    match logType with
    | Log.Err err -> AnsiConsole.MarkupLine($"[red]-[/] {err}")
    | Log.Msg msg -> AnsiConsole.MarkupLine($"[green]-[/] {msg}")
    | Log.Info info -> AnsiConsole.MarkupLine($"[blue]-[/] {info}")

let log (logtype: Log) =
  let a = A.Instance
  a.Action(logtype)
