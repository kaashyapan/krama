module Krama.Json.GeneratedAlias

open System
open System.IO
open Krama.Types

type A private () =
  let mutable acc = Set.empty
  static let instance = A()
  static member Instance = instance

  member this.Add(t: T) = acc <- Set.add t acc

  member this.Get() = acc

let add (t: T) =
  let a = A.Instance
  a.Add(t)

let list () =
  let a = A.Instance
  a.Get()
