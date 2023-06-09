module MultiFile

open System
open System.IO
open Krama.Types
open Krama.Config
open Krama.Compiler
open Krama.CodePrinter
open Expecto
open FSharpx

let config () =
  let includes = [
    Map ["Model.Person", None] 
    Map ["Model.Employee", None]
  ] 

  let output =
    {
      Json.Output.File = "output.fs"
      Json.Output.Namespace = None 
      Json.Output.Module = None
      Json.Output.Includes = includes
      Json.Output.Excludes = None 
    }

  {
      Json.Config.Version = 1
      Json.Config.Case = Some Json.KeyCase.Camel 
      Json.Config.Outputs = [ output ]
      Json.Config.FloatAsString = None
      Json.Config.DateAsFloat = None
  }

let tests =
    let fsproj = FileInfo("/home/ubuntu/krama/tool/tests/multi_file/Sample.fsproj") 
    let typs = Krama.Compiler.processFsFiles fsproj
    let actual = List.length typs
    let expected = 9 

    [
      test "testname" { Expect.equal actual expected "Multi file inputs" }
    ]
