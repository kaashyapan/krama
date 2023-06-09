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
          T.Option(T.Userdef "Test.Persons")
          T.RecordMember("allpersons", T.Option(T.Userdef "Test.Persons")) 
          T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Persons")) ]
          T.Alias(
            "Test.Model",
            T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Persons")) ]
          )
          T.List(T.Userdef "Test.Name")
          T.Alias("Test.Persons", T.List(T.Userdef "Test.Name"))
          T.Alias("Test.Name", T.String)
        ]
      | "multi_module" ->
        [
          T.Option(T.Userdef "Test.Mod1.Persons")
          T.RecordMember("allpersons", T.Option(T.Userdef "Test.Mod1.Persons")) 
          T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Mod1.Persons")) ]
          T.Alias(
            "Test.Model",
            T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Mod1.Persons")) ]
          )
          T.List(T.Userdef "Test.Mod1.Name")
          T.Alias("Test.Mod1.Persons", T.List(T.Userdef "Test.Mod1.Name"))
          T.Alias("Test.Mod1.Name", T.String)
        ]
      | "skip_unused" ->
        [
          T.Option(T.Userdef "Test.Persons")
          T.RecordMember("allpersons", T.Option(T.Userdef "Test.Persons"))
          T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Persons")) ]
          T.Alias(
            "Test.Model",
            T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Persons")) ]
          )
          T.List(T.String)
          T.Alias("Test.Persons", T.List(T.String))
        ]
      | "deep_dep" ->
        [
          T.Option(T.Userdef "Test.Name")
          T.List(T.Option(T.Userdef "Test.Name"))
          T.RecordMember("allpersons", T.List(T.Option(T.Userdef "Test.Name")))
          T.Record [ T.RecordMember("allpersons", T.List(T.Option(T.Userdef "Test.Name"))) ]
          T.Alias(
            "Test.Model", 
            T.Record [ T.RecordMember("allpersons", T.List(T.Option(T.Userdef "Test.Name"))) ]
          )
          T.Alias("Test.Name", T.String)
        ]

      | _ -> []

    test testname { Expect.sequenceContainsOrder actual expected testname }
  )
