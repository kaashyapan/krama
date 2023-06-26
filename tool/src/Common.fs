module Krama.Common

open System
open System.Text
open System.Security.Cryptography
open FSharp.Core.CompilerServices

module List =
  let intersperse (separator) (source: List<'T>) =
    let mutable coll = new ListCollector<'T>()
    let mutable notFirst = false

    source
    |> List.iter (fun element ->
      if notFirst then coll.Add separator

      coll.Add element
      notFirst <- true
    )

    coll.Close()

module String =
  let join (sep: string) (ls: string list) =
    ls |> List.intersperse sep |> List.reduce (fun s1 s2 -> s1 + s2)

let getHash (str: string) =
  let bytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str))
  let bitString = BitConverter.ToUInt32(bytes)
  bitString.ToString()
