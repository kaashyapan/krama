module Heirarchy

open System
open System.IO
open Krama.Types
open Krama.Compiler
open Expecto
open FSharpx

let processFile (path: string) =
  printfn "%A" path
  let typs = path |> genModules
  path, typs

let tests =
  Directory.EnumerateFiles("./heirarchy", "*.fsx")
  |> Seq.map processFile
  |> Seq.toList
  |> List.map (fun (path, types) ->
    let testname =
      path
      |> String.splitChar [| '/' |]
      |> Array.last
      |> String.splitChar [| '.' |]
      |> Array.head

    let actual = Krama.Types.getTypeHeirarchy types "Test.Model"

    let expected =
      match testname with
      | "simple" ->
        [
          T.Alias("Test.Name", T.String)
          T.Alias("Test.Persons", T.List(T.Userdef "Test.Name"))
          T.Alias(
            "Test.Model",
            T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Persons")) ]
          )
        ]
      | "multi_module" ->
        [
          T.Alias("Test.Mod1.Name", T.String)
          T.Alias("Test.Mod1.Persons", T.List(T.Userdef "Test.Mod1.Name"))
          T.Alias(
            "Test.Model",
            T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Mod1.Persons")) ]
          )
        ]
      | "skip_unused" ->
        [
          T.Alias("Test.Persons", T.List(T.String))
          T.Alias(
            "Test.Model",
            T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Persons")) ]
          )
        ]
      | "deep_dep" ->
        [
          T.Alias("Test.Name", T.String)
          T.Alias(
            "Test.Model",
            T.Record [ T.RecordMember("allpersons", T.List(T.Option(T.Userdef "Test.Name"))) ]
          )
        ]

      | _ -> []

    test testname { Expect.sequenceContainsOrder actual expected testname }
  )
