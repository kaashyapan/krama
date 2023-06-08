module Heirarchy

open System
open System.IO
open Krama.Types
open Krama.Compiler
open Expecto
open FSharpx

let tests =
  Directory.EnumerateDirectories("./heirarchy") 
  |> Seq.where (fun x -> not <| String.contains @"/." x)
  //|> Seq.where (fun x -> String.contains @"multi" x)
  //|> (fun x -> printfn "%A" x ; x)
  |> Seq.map (fun dir -> sprintf "%s/%s" dir "test.fsproj")
  |> Seq.toList
  |> List.map (fun projfile  ->
    let testname =
      projfile
      |> String.splitChar [| '/' |]
      |> Array.rev
      |> Array.tail
      |> Array.head

    let types = Krama.Compiler.processFsFiles <| FileInfo(projfile)
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
