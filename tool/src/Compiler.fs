module rec Krama.Compiler

open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open Krama.Types
open Krama.Log
open FSharpx

let checker = FSharpChecker.Create()

// Will process the type defn of a derived FSharp abbreviated type.
// If a user defined type is encountered, dig no furthur.
// Will be called from any of the collection / derived types
let makeGenericArguments (tlist: FSharpType seq) =
  let sharpNamespaces = [ "System"; "Microsoft.FSharp.Core"; "Microsoft.FSharp.Collections" ]

  tlist
  |> Seq.map (fun field ->
    if field.IsAnonRecordType then
      makeType field.AbbreviatedType
    else if List.contains field.TypeDefinition.AccessPath sharpNamespaces then
      makeType field.AbbreviatedType
    else
      [ field.TypeDefinition.AccessPath; field.TypeDefinition.DisplayName ]
      |> String.concat "."
      |> T.Userdef

  )
  |> Seq.toList

let rec makeType (t: FSharpType) : T =
  //TODO structs, classes, enum, C# collection types, ValueTuple
  match t with
  | t when t.IsAnonRecordType ->
    t.GenericArguments
    |> Seq.allPairs t.AnonRecordTypeDetails.SortedFieldNames
    |> Seq.map (fun (name, field) ->
      let f = [ field ] |> makeGenericArguments |> List.head
      T.RecordMember(name, f)
    )
    |> Seq.toList
    |> T.AnonRecord

  | _ ->
    match t.TypeDefinition.AccessPath with
    | "Microsoft.FSharp.Core" ->
      match String.toLower t.TypeDefinition.DisplayName with
      | "char" -> T.Char
      | "string" -> T.String
      | "bool" -> T.Bool
      | "byte" -> T.Byte
      | "sbyte" -> T.SByte
      | "int" -> T.Int32
      | "int8" -> T.Int8
      | "int16" -> T.Int16
      | "int32" -> T.Int32
      | "int64" -> T.Int64
      | "int128" -> T.Int128
      | "uint" -> T.UInt32
      | "uint8" -> T.UInt8
      | "uint16" -> T.UInt16
      | "uint32" -> T.UInt32
      | "uint64" -> T.UInt64
      | "uint128" -> T.UInt128
      | "single" -> T.Single
      | "float32" -> T.Single
      | "double" -> T.Double
      | "float" -> T.Double
      | "option" -> t.GenericArguments |> makeGenericArguments |> List.head |> T.Option
      | "voption" -> t.GenericArguments |> makeGenericArguments |> List.head |> T.VOption
      | "valueoption" -> t.GenericArguments |> makeGenericArguments |> List.head |> T.VOption
      | "result" -> t.GenericArguments |> makeGenericArguments |> T.Result
      | "array" -> t.GenericArguments |> makeGenericArguments |> List.head |> T.Array
      | "choice" -> t.GenericArguments |> makeGenericArguments |> T.Choice
      | "exn" -> T.Exception
      | x ->
        log (Log.Err $"Unhandled {x} in Microsoft.FSharp.Core")
        T.TypeNotSupported

    | "Microsoft.FSharp.Collections" ->
      match String.toLower t.TypeDefinition.DisplayName with
      | "list" -> t.GenericArguments |> makeGenericArguments |> List.head |> T.List
      | "seq" -> t.GenericArguments |> makeGenericArguments |> List.head |> T.Seq
      | "set" -> t.GenericArguments |> makeGenericArguments |> List.head |> T.Set
      | "map" -> t.GenericArguments |> makeGenericArguments |> T.Map

      | x ->
        log (Log.Err $"Unhandled {x} in Microsoft.FSharp.Collections")
        T.TypeNotSupported

    | "System" ->
      match String.toLower t.TypeDefinition.DisplayName with
      | "char" -> T.Char
      | "string" -> T.String
      | "byte" -> T.Byte
      | "sbyte" -> T.SByte
      | "bool" -> T.Bool
      | "int" -> T.Int32
      | "int8" -> T.Int8
      | "int16" -> T.Int16
      | "int32" -> T.Int32
      | "int64" -> T.Int64
      | "int128" -> T.Int128
      | "uint" -> T.UInt32
      | "uint8" -> T.UInt8
      | "uint16" -> T.UInt16
      | "uint32" -> T.UInt32
      | "uint64" -> T.UInt64
      | "uint128" -> T.UInt128
      | "half" -> T.Half
      | "single" -> T.Single
      | "float" -> T.Double
      | "float32" -> T.Single
      | "double" -> T.Double
      | "guid" -> T.Guid
      | "boolean" -> T.Bool
      | "exception" -> T.Exception
      | "datetime" -> T.DateTime
      | "dateonly" -> T.DateOnly
      | "datetimeoffset" -> T.DateTimeOffset
      | "timeonly" -> T.TimeOnly
      | "timespan" -> T.TimeSpan
      | "array" -> t.GenericArguments |> makeGenericArguments |> List.head |> T.Array
      | "tuple" -> t.GenericArguments[0].GenericArguments |> makeGenericArguments |> T.Tuple

      | x ->
        log (Log.Err $"Unhandled System type {x}")
        T.TypeNotSupported

    | userDefinedType ->
      [ t.TypeDefinition.AccessPath; t.TypeDefinition.DisplayName ]
      |> String.concat "."
      |> T.Userdef

let rec getType (t: FSharpEntity) : T seq =
  match t with

  | t when t.IsFSharpAbbreviation ->
    let accessPath = String.concat "." [ t.AccessPath; t.DisplayName ]
    let typ = makeType t.AbbreviatedType
    (accessPath, typ) |> T.Alias |> Seq.singleton

  | t when t.IsFSharpRecord ->
    let accessPath = String.concat "." [ t.AccessPath; t.DisplayName ]

    let typ =
      t.FSharpFields
      |> Seq.map (fun field ->
        (**
        if
          field.FieldType.BasicQualifiedName = "Microsoft.FSharp.Core.obj"
          && field.FieldType.HasTypeDefinition
          && field.FieldType.IsAbbreviation
        then
          let f = T.Unresolved(field.Name)
          T.RecordMember(field.Name, f)
        else
              *)

        let f = [ field.FieldType ] |> makeGenericArguments |> List.head
        T.RecordMember(field.Name, f)
      )
      |> Seq.toList
      |> T.Record

    (accessPath, typ) |> T.Alias |> Seq.singleton

  | t when t.IsFSharpUnion ->
    let accessPath = String.concat "." [ t.AccessPath; t.DisplayName ]

    let typ =
      t.UnionCases
      |> Seq.map (fun field ->
        let payloads =
          field.Fields
          |> Seq.map (fun f -> [ f.FieldType ] |> makeGenericArguments |> List.head)
          |> Seq.toList

        (field.Name, payloads) |> T.UnionMember

      )
      |> Seq.toList
      |> T.Union

    (accessPath, typ) |> T.Alias |> Seq.singleton

  | t when t.IsFSharpModule ->

    t.GetPublicNestedEntities() |> Seq.map getType |> Seq.concat

  | _ -> T.TypeNotSupported |> Seq.singleton

let parseAndTypeCheckSingleFile (file, input) =
  let projOptions =
    {
      FSharpProjectOptions.IsIncompleteTypeCheckEnvironment = true
      LoadTime = System.DateTime.Now
      OriginalLoadReferences = []
      OtherOptions = [||]
      ProjectFileName = ".fsproj"
      ProjectId = Some ""
      ReferencedProjects = [||]
      SourceFiles = [| file |]
      Stamp = None
      UnresolvedReferences = None
      UseScriptResolutionRules = true
    }
  // Get context representing a stand-alone (script) file
  let parseFileResults, checkFileResults =
    checker.ParseAndCheckFileInProject(file, 0, input, projOptions)
    // GetProjectOptionsFromScript(file, input, assumeDotNetFramework = false)
    |> Async.RunSynchronously

  // let parseFileResults, checkFileResults =
  //   checker.ParseAndCheckFileInProject(file, 0, input, projOptions)
  //   |> Async.RunSynchronously

  // Wait until type checking succeeds (or 100 attempts)

  match checkFileResults with
  | FSharpCheckFileAnswer.Succeeded(res) -> parseFileResults, res
  | res -> failwithf "Parsing did not finish... (%A)" res

let rec genModules (projFile: string) (file: string) =

  let text = File.ReadAllText(file)

  let parseFileResults, checkFileResults =
    parseAndTypeCheckSingleFile (file, SourceText.ofString text)

  let partialAssemblySignature = checkFileResults.PartialAssemblySignature

  partialAssemblySignature.TryGetEntities()
  |> Seq.map getType
  |> Seq.concat
  |> Seq.where (fun t -> t <> T.TypeNotSupported)
  |> Set.ofSeq
  |> Set.toList
