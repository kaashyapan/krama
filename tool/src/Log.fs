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
    | Log.Err err ->
      AnsiConsole.Foreground <- Color.Red
      AnsiConsole.MarkupLine(err)
    | Log.Msg msg ->
      AnsiConsole.Foreground <- Color.Green
      AnsiConsole.MarkupLine(msg)
    | Log.Info info ->
      AnsiConsole.Foreground <- Color.Blue
      AnsiConsole.MarkupLine(info)

let log (logtype: Log) =
  let a = A.Instance
  a.Action(logtype)
