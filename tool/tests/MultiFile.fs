module MultiFile

open System
open System.IO
open Krama.Types
open Krama.Config
open Krama.Compiler
open Krama.Files
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
    let dir = DirectoryInfo("/home/ubuntu/krama/tool/tests/multi_file") 
    let typs = Krama.Files.processFsFiles dir
    let typeHeirarchy = Krama.CodePrinter.writeJson (config()) typs 
    let actual = []
    let expected = []

    [
      test "testname" { Expect.sequenceContainsOrder actual expected "Multi file inputs" }
    ]
