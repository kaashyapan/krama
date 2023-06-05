module Typechecker

open System
open System.IO
open Krama.Types
open Krama.Compiler
open Expecto
open FSharpx

let processFile (path: string) =
  printfn "%A" path
  let typ = path |> genModules
  path, typ

let tests =
  Directory.EnumerateFiles("./typecheck_files", "*.fsx")
  |> Seq.map processFile
  |> Seq.toList
  |> List.map (fun (path, t) ->
    let testname =
      path
      |> String.splitChar [| '/' |]
      |> Array.last
      |> String.splitChar [| '.' |]
      |> Array.head

    let expected =
      match testname with
      | "string" ->
        seq {
          T.Alias("Test.Var1", T.String)
          T.Alias("Test.Var2", T.String)
        }

      | "int64" ->
        seq {
          T.Alias("Test.Var1", T.Int64)
          T.Alias("Test.Var2", T.Int64)
        }

      | "int32" ->
        seq {
          T.Alias("Test.Var1", T.Int32)
          T.Alias("Test.Var2", T.Int32)
          T.Alias("Test.Var3", T.Int32)
        }

      | "int16" ->
        seq {
          T.Alias("Test.Var1", T.Int16)
          T.Alias("Test.Var2", T.Int16)
        }

      | "int8" -> seq { T.Alias("Test.Var1", T.Int8) }

      | "uint64" ->
        seq {
          T.Alias("Test.Var1", T.UInt64)
          T.Alias("Test.Var2", T.UInt64)
        }

      | "uint32" ->
        seq {
          T.Alias("Test.Var1", T.UInt32)
          T.Alias("Test.Var2", T.UInt32)
          T.Alias("Test.Var3", T.UInt32)
        }

      | "uint16" ->
        seq {
          T.Alias("Test.Var1", T.UInt16)
          T.Alias("Test.Var2", T.UInt16)
        }

      | "uint8" -> seq { T.Alias("Test.Var1", T.UInt8) }

      | "result" -> seq { T.Alias("Test.Var1", T.Result [ T.String; T.Int32 ]) }

      | "option" -> seq { T.Alias("Test.Var1", T.Option T.String) }

      | "list" -> seq { T.Alias("Test.Var1", T.List T.String) }

      | "array" -> seq { T.Alias("Test.Var1", T.Array T.String) }

      | "seq" -> seq { T.Alias("Test.Var1", T.Seq T.String) }

      | "set" -> seq { T.Alias("Test.Var1", T.Set T.String) }

      | "voption" -> seq { T.Alias("Test.Var1", T.VOption T.String) }

      | "tuple" -> seq { T.Alias("Test.Var1", T.Tuple [ T.String; T.Int32 ]) }

      | "exception" ->
        seq {
          T.Alias("Test.Var1", T.Exception)
          T.Alias("Test.Var2", T.Exception)
        }

      | "guid" -> seq { T.Alias("Test.Var1", T.Guid) }

      | "union_simple" ->
        seq {
          T.Alias("Test.Sex", T.Union [ T.UnionMember("Male", []); T.UnionMember("Female", []) ])
        }

      | "union_with" ->
        seq {
          T.Alias(
            "Test.Person",
            T.Union [ T.UnionMember("Male", [ T.Int32 ]); T.UnionMember("Female", [ T.Int32 ]) ]
          )
        }

      | "union_nested" ->
        seq {
          T.Alias(
            "Test.Being",
            T.Union
              [
                T.UnionMember("Human", [ T.Userdef "Test.Person" ])
                T.UnionMember("Animal", [ T.Userdef "Test.Person" ])
              ]
          )

          T.Alias(
            "Test.Person",
            T.Union [ T.UnionMember("Male", []); T.UnionMember("Female", []) ]
          )
        }

      | "anon_record" ->
        seq { T.Alias("Test.Var1", T.Option(T.AnonRecord [ T.RecordMember("Name", T.String) ])) }

      | "record" ->
        seq {
          T.Alias(
            "Test.Person",
            T.Record [ T.RecordMember("Name", T.String); T.RecordMember("Age", T.Int32) ]
          )
        }

      | "dep_union" ->
        seq {
          T.Alias("Test.Person", T.Record [ T.RecordMember("Gender", T.Userdef "Test.Sex") ])
          T.Alias("Test.Sex", T.Union [ T.UnionMember("Male", []); T.UnionMember("Female", []) ])
        }

      | "dep_alias" ->
        seq {
          T.Alias("Test.Name", T.String)
          T.Alias("Test.Var2", T.List(T.Userdef "Test.Name"))
        }

      | "dep_record" ->
        seq {
          T.Alias("Test.Var1", T.Record [ T.RecordMember("Name", T.String) ])
          T.Alias("Test.Var2", T.Record [ T.RecordMember("Person", T.Userdef "Test.Var1") ])
        }

      | "dep_multi" ->
        seq {
          T.Alias(
            "Test.Model",
            T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.Persons")) ]
          )

          T.Alias("Test.Name", T.String)
          T.Alias("Test.Persons", T.List(T.Userdef "Test.Name"))
        }

      | "module" ->
        seq {
          T.Alias("Test.Var1", T.Option T.String)
          T.Alias("Test.Var2", T.Option T.String)
        }

      | "half" -> seq { T.Alias("Test.Var1", T.Half) }

      | "double" -> seq { T.Alias("Test.Var1", T.Double) }

      | "single" -> seq { T.Alias("Test.Var1", T.Single) }

      | "dateonly" -> seq { T.Alias("Test.Var1", T.DateOnly) }

      | "datetime" -> seq { T.Alias("Test.Var1", T.DateTime) }

      | "datetimeoffset" -> seq { T.Alias("Test.Var1", T.DateTimeOffset) }

      | "timeonly" -> seq { T.Alias("Test.Var1", T.TimeOnly) }

      | "timespan" -> seq { T.Alias("Test.Var1", T.TimeSpan) }

      | _ -> T.TypeNotSupported |> Seq.singleton

    test testname { Expect.sequenceContainsOrder t expected testname }
  )
