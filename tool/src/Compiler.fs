module rec Krama.Compiler

open System
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Compiler.Diagnostics
open Krama.Types
open Krama.Log
open FSharpx
open Ionide.ProjInfo

let checker = FSharpChecker.Create(keepAssemblyContents = true)

// Will process the type defn of a derived FSharp abbreviated type.
// If a user defined type is encountered, dig no furthur.
// Will be called from any of the collection / derived types
let makeGenericArguments (tlist: FSharpType seq) =
  let sharpNamespaces = [ "System"; "Microsoft.FSharp.Core"; "Microsoft.FSharp.Collections" ]

  tlist
  |> Seq.map (fun field ->
    //Accesspath will not be available for anonrecords
    if field.IsAnonRecordType then
      makeType field.AbbreviatedType
    else if field.IsTupleType then
      makeType field.AbbreviatedType
    else if
      field.HasTypeDefinition
      && List.contains field.TypeDefinition.AccessPath sharpNamespaces
    then
      makeType field.AbbreviatedType
    else if field.HasTypeDefinition then
      [ field.TypeDefinition.AccessPath; field.TypeDefinition.DisplayName ]
      |> String.concat "."
      |> T.Userdef
    else
      failwith (sprintf "Type not handled %A" field.ToString)

  )
  |> Seq.toList

let rec makeType (t: FSharpType) : T =
  //TODO structs, classes, enum, C# collection types, ValueTuple
  match t with
  | t when t.IsAnonRecordType ->
    t.GenericArguments
    |> Seq.zip t.AnonRecordTypeDetails.SortedFieldNames
    |> Seq.map (fun (name, field) ->
      let f = [ field ] |> makeGenericArguments |> List.head
      T.RecordMember(name, f)
    )
    |> Seq.toList
    |> T.AnonRecord

  | t when t.IsTupleType ->
    t.GenericArguments
    |> Seq.map (fun field -> [ field ] |> makeGenericArguments |> List.head)
    |> Seq.toList
    |> T.Tuple

  | t when t.HasTypeDefinition ->
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
      | "obj" ->
        //let symbol = FSharpSymbol
        //projResults.GetUsesOfSymbol
        log (Log.Err $"Unhandled System type obj")
        T.TypeNotSupported

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
        let f = [ field.FieldType ] |> makeGenericArguments |> List.head
        T.RecordMember(field.Name, f)
      )
      |> Seq.toList
      |> T.Record

    (accessPath, typ) |> T.Alias |> Seq.singleton

  | t when t.IsFSharpUnion ->
    let accessPath = String.concat "." [ t.AccessPath; t.DisplayName ]

    let members =
      t.UnionCases
      |> Seq.map (fun field ->
        let payloads =
          field.Fields
          |> Seq.map (fun f -> [ f.FieldType ] |> makeGenericArguments |> List.head)
          |> Seq.toList

        let unionMemberTyp = (field.Name, payloads) |> T.UnionMember
        (unionMemberTyp, List.length payloads)
      )
      |> Seq.toList

    let payloadCount = members |> List.map (fun (_, payloadCount) -> payloadCount) |> List.sum

    let members' = members |> List.map (fun (umems, _) -> umems)
    let typ = if payloadCount > 0 then T.Union members' else T.UnionSimple members'

    (accessPath, typ) |> T.Alias |> Seq.singleton

  | t when t.IsFSharpModule ->

    t.GetPublicNestedEntities() |> Seq.map getType |> Seq.concat

  | _ -> T.TypeNotSupported |> Seq.singleton

let parseAndTypeCheck (ionide: Ionide.ProjInfo.Types.ProjectOptions) =
  log (Log.Info "Typechecking your project ..")

  let projOptions =
    {
      FSharpProjectOptions.IsIncompleteTypeCheckEnvironment = true
      LoadTime = ionide.LoadTime
      OriginalLoadReferences = []
      OtherOptions = ionide.OtherOptions |> List.toArray
      ProjectFileName = ionide.ProjectFileName
      ProjectId = ionide.ProjectId
      ReferencedProjects = [||]
      SourceFiles = ionide.SourceFiles |> List.toArray
      Stamp = None
      UnresolvedReferences = None
      UseScriptResolutionRules = false
    }

  let projResults = checker.ParseAndCheckProject(projOptions, "krama") |> Async.RunSynchronously

  let severeErrorCount =
    projResults.Diagnostics
    |> Array.where (fun diag -> diag.Severity = FSharpDiagnosticSeverity.Error)
    |> Array.map (fun diag ->
      log (
        Log.Err(
          sprintf
            "Error in %s. Line %d Position %d - %s"
            diag.FileName
            diag.StartLine
            diag.StartColumn
            diag.Message
        )
      )

      diag
    )
    |> Array.length

  match projResults.HasCriticalErrors || severeErrorCount <> 0 with
  | false ->
    projResults.AssemblySignature.Entities
    |> Seq.map getType
    |> Seq.concat
    |> Seq.toList

  | true ->
    log (Log.Err "Typechecking unsuccessful. Check your project for compiler errors")
    []

let processFsFiles (fsProjFile: FileInfo) : Types.T list =
  let toolsPath = Init.init fsProjFile.Directory None
  let loader: IWorkspaceLoader = WorkspaceLoader.Create(toolsPath, [])
  log (Log.Info "Project cracking ..")
  let projectOptions = loader.LoadProjects([ fsProjFile.FullName ]) |> Seq.toArray |> Array.head
  parseAndTypeCheck projectOptions
