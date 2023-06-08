module Typechecker

open System
open System.IO
open Krama.Types
open Krama.Compiler
open Expecto
open FSharpx

let types =
  let fsProjFile = FileInfo("./typecheck_files/Typechecker.fsproj")
  processFsFiles fsProjFile

let tests =
  Directory.EnumerateFiles("./typecheck_files", "*.fs")
  |> Seq.toList
  |> List.map (fun path ->
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
          T.Alias("Test.String.Var1", T.String)
          T.Alias("Test.String.Var2", T.String)
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "int64" ->
        seq {
          T.Alias("Test.Int64.Var1", T.Int64)
          T.Alias("Test.Int64.Var2", T.Int64)
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "int32" ->
        seq {
          T.Alias("Test.Int32.Var1", T.Int32)
          T.Alias("Test.Int32.Var2", T.Int32)
          T.Alias("Test.Int32.Var3", T.Int32)
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "int16" ->
        seq {
          T.Alias("Test.Int16.Var1", T.Int16)
          T.Alias("Test.Int16.Var2", T.Int16)
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "int8" -> seq { T.Alias("Test.Int8.Var1", T.Int8) } |> Seq.map (fun e -> Seq.contains e types)
      | "uint64" ->
        seq {
          T.Alias("Test.Uint64.Var1", T.UInt64)
          T.Alias("Test.Uint64.Var2", T.UInt64)
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "uint32" ->
        seq {
          T.Alias("Test.Uint32.Var1", T.UInt32)
          T.Alias("Test.Uint32.Var2", T.UInt32)
          T.Alias("Test.Uint32.Var3", T.UInt32)
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "uint16" ->
        seq {
          T.Alias("Test.Uint16.Var1", T.UInt16)
          T.Alias("Test.Uint16.Var2", T.UInt16)
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "uint8" -> seq { T.Alias("Test.Uint8.Var1", T.UInt8) }
                      |> Seq.map (fun e -> Seq.contains e types)

      | "result" -> seq { T.Alias("Test.Result.Var1", T.Result [ T.String; T.Int32 ]) } |> Seq.map (fun e -> Seq.contains e types)


      | "option" -> seq { T.Alias("Test.Option.Var1", T.Option T.String) } |> Seq.map (fun e -> Seq.contains e types)


      | "list" -> seq { T.Alias("Test.List.Var1", T.List T.String) } |> Seq.map (fun e -> Seq.contains e types)


      | "array" -> seq { T.Alias("Test.Array.Var1", T.Array T.String) } |> Seq.map (fun e -> Seq.contains e types)


      | "seq" -> seq { T.Alias("Test.Seq.Var1", T.Seq T.String) } |> Seq.map (fun e -> Seq.contains e types)


      | "set" -> seq { T.Alias("Test.Set.Var1", T.Set T.String) } |> Seq.map (fun e -> Seq.contains e types)


      | "voption" -> seq { T.Alias("Test.Voption.Var1", T.VOption T.String) } |> Seq.map (fun e -> Seq.contains e types)


      | "tuple" -> seq { T.Alias("Test.Tuple.Var1", T.Tuple [ T.String; T.Int32 ]) } |> Seq.map (fun e -> Seq.contains e types)


      | "exception" ->
        seq {
          T.Alias("Test.Exception.Var1", T.Exception)
          T.Alias("Test.Exception.Var2", T.Exception)
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "guid" -> seq { T.Alias("Test.Guid.Var1", T.Guid) } |> Seq.map (fun e -> Seq.contains e types)


      | "union_simple" ->
        seq {
          T.Alias("Test.UnionSimple.Sex", T.Union [ T.UnionMember("Male", []); T.UnionMember("Female", []) ])
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "union_with" ->
        seq {
          T.Alias(
            "Test.UnionWith.Person",
            T.Union [ T.UnionMember("Male", [ T.Int32 ]); T.UnionMember("Female", [ T.Int32 ]) ]
          )
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "union_nested" ->
        seq {
          T.Alias(
            "Test.UnionNested.Being",
            T.Union
              [
                T.UnionMember("Human", [ T.Userdef "Test.UnionNested.Person" ])
                T.UnionMember("Animal", [ T.Userdef "Test.UnionNested.Person" ])
              ]
          )

          T.Alias(
            "Test.UnionNested.Person",
            T.Union [ T.UnionMember("Male", []); T.UnionMember("Female", []) ]
          )
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "anon_record" ->
        seq { T.Alias("Test.AnonRecord.Var1", T.Option(T.AnonRecord [ T.RecordMember("Name", T.String) ])) }
         |> Seq.map (fun e -> Seq.contains e types)

      | "record" ->
        seq {
          T.Alias(
            "Test.Record.Person",
            T.Record [ T.RecordMember("Name", T.String); T.RecordMember("Age", T.Int32) ]
          )
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "dep_union" ->
        seq {
          T.Alias("Test.DepUnion.Person", T.Record [ T.RecordMember("Gender", T.Userdef
          "Test.DepUnion.Sex") ])
          T.Alias("Test.DepUnion.Sex", T.Union [ T.UnionMember("Male", []); T.UnionMember("Female", []) ])
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "dep_alias" ->
        seq {
          T.Alias("Test.DepAlias.Name", T.String)
          T.Alias("Test.DepAlias.Var2", T.List(T.Userdef "Test.DepAlias.Name"))
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "dep_record" ->
        seq {
          T.Alias("Test.DepRecord.Var1", T.Record [ T.RecordMember("Name", T.String) ])
          T.Alias("Test.DepRecord.Var2", T.Record [ T.RecordMember("Person", T.Userdef "Test.DepRecord.Var1") ])
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "dep_multi" ->
        seq {
          T.Alias(
            "Test.DepMulti.Model",
            T.Record [ T.RecordMember("allpersons", T.Option(T.Userdef "Test.DepMulti.Persons")) ]
          )

          T.Alias("Test.DepMulti.Name", T.String)
          T.Alias("Test.DepMulti.Persons", T.List(T.Userdef "Test.DepMulti.Name"))
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "module" ->
        seq {
          T.Alias("Test.Module.Var1", T.Option T.String)
          T.Alias("Test.Module.Var2", T.Option T.String)
        }
        |> Seq.map (fun e -> Seq.contains e types)

      | "half" -> seq { T.Alias("Test.Half.Var1", T.Half) } |> Seq.map (fun e -> Seq.contains e types)


      | "double" -> seq { T.Alias("Test.Double.Var1", T.Double) } |> Seq.map (fun e -> Seq.contains e types)


      | "single" -> 
          seq { T.Alias("Test.Single.Var1", T.Single) } 
          |> Seq.map (fun e -> Seq.contains e types) 


      | "dateonly" -> seq { T.Alias("Test.DateOnly.Var1", T.DateOnly) } |> Seq.map (fun e -> Seq.contains e types)


      | "datetime" -> seq { T.Alias("Test.DateTime.Var1", T.DateTime) } |> Seq.map (fun e -> Seq.contains e types)


      | "datetimeoffset" -> seq { T.Alias("Test.DateTimeOffset.Var1", T.DateTimeOffset) } |> Seq.map (fun e -> Seq.contains e types)


      | "timeonly" -> seq { T.Alias("Test.TimeOnly.Var1", T.TimeOnly) } |> Seq.map (fun e -> Seq.contains e types)


      | "timespan" -> seq { T.Alias("Test.TimeSpan.Var1", T.TimeSpan) } |> Seq.map (fun e -> Seq.contains e types)


      | _ -> Seq.empty

    let expected' = Seq.contains false expected
    test testname { Expect.isFalse expected' testname }
  )
