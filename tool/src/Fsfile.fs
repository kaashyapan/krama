module Krama.Files

open System
open System.IO
open Krama.Config
open Krama.Log
open Krama.Compiler
open FSharpx
open System.Text.RegularExpressions
open Regx
open Regx.Helpers
open Fantomas.Core
open Fantomas.Core.SyntaxOak

let fable_modules =
  group {
    zeroOrMore { wordChar }

    oneOf {
      verbatimString @"/fable_modules/"
      verbatimString @"\fable_modules\"
    }

    zeroOrMore { wordChar }
  }

let bin =
  group {
    zeroOrMore { wordChar }

    oneOf {
      verbatimString @"/bin/"
      verbatimString @"\bin\"
    }

    zeroOrMore { wordChar }
  }

let obj =
  group {
    zeroOrMore { wordChar }

    oneOf {
      verbatimString @"/obj/"
      verbatimString @"\obj\"
    }

    zeroOrMore { wordChar }
  }

let node_modules =
  group {
    zeroOrMore { wordChar }

    oneOf {
      verbatimString @"/node_modules/"
      verbatimString @"\node_modules\"
    }

    zeroOrMore { wordChar }
  }

let hidden_folder =
  group {
    zeroOrMore { wordChar }

    oneOf {
      group {
        verbatimString @"/."
        word
        verbatimString @"/"
      }

      group {
        verbatimString @"\."
        word
        verbatimString @"\"
      }
    }

    zeroOrMore { wordChar }
  }

let pattern =
  regex {
    oneOf {
      fable_modules
      bin
      obj
      node_modules
      hidden_folder
    }
  }
  |> Regx.make

let processFsFiles (dir: DirectoryInfo) : Types.T list =
  dir.EnumerateFiles("*.fsproj", SearchOption.TopDirectoryOnly)
  |> Seq.tryHead
  |> function
    | Some fsprojfile ->
      dir.EnumerateFiles("*.fs?", SearchOption.AllDirectories)
      |> Seq.map (fun filename -> filename.FullName)
      |> Seq.where (fun filename -> not <| Regex.IsMatch(filename, pattern))
      |> Seq.map (fun f -> genModules fsprojfile.FullName f)
      |> Seq.concat
      |> Seq.toList

    | None ->
      log (Log.Err "Did not find an fsproj file")
      []
