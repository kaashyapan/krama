#r "nuget: FSharp.Compiler.Service, 43.7.200"

open System
open System.IO
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

// Create an interactive checker instance
let checker = FSharpChecker.Create()

let parseAndTypeCheckSingleFile (file, input) =
  // Get context representing a stand-alone (script) file
  let projOptions, errors =
    checker.GetProjectOptionsFromScript(file, input, assumeDotNetFramework = false)
    |> Async.RunSynchronously

  let parseFileResults, checkFileResults =
    checker.ParseAndCheckFileInProject(file, 0, input, projOptions)
    |> Async.RunSynchronously

  // Wait until type checking succeeds (or 100 attempts)
  match checkFileResults with
  | FSharpCheckFileAnswer.Succeeded(res) -> parseFileResults, res
  | res -> failwithf "Parsing did not finish... (%A)" res

let file = "/home/ubuntu/krama/sample/Kitchensink.fs"
let text = File.ReadAllText("/home/ubuntu/krama/sample/Kitchensink.fs")

let parseFileResults, checkFileResults =
  parseAndTypeCheckSingleFile (file, SourceText.ofString text)

let partialAssemblySignature = checkFileResults.PartialAssemblySignature

let rec makeType (t: FSharpType) =
  printfn "Compiled type %A" t.TypeDefinition.CompiledName
  printfn "Has type defn %A" t.HasTypeDefinition
  printfn "Nested type %A" t.AbbreviatedType
  printfn "Is Abbreviation %A" t.IsAbbreviation

  if t.IsGenericParameter then
    printfn "Generic %A" t.GenericParameter.Name
    printfn "Generic %A" t.GenericArguments

    for field in t.GenericArguments do
      printfn "GenericArg %A" field.AbbreviatedType

  if t.IsTupleType then printfn "Tuple type %A" t.IsAnonRecordType

  if t.IsAnonRecordType then printfn "Anon type %A" t.IsAnonRecordType

let rec getType (t: FSharpEntity) =

  printfn "----------------------------------------"

  if t.IsArrayType then
    printfn "Array type %A" t.DisplayName
    printfn "Array rank %A" t.ArrayRank

  if t.IsMeasure then printfn "Measure type %A" t.DisplayName

  if t.IsEnum then printfn "Enum type %A" t.DisplayName

  if t.IsValueType then printfn "Value type %A" t.DisplayName

  if t.IsFSharpAbbreviation then
    printfn "Abbr type %A" t.DisplayName
    makeType t.AbbreviatedType

  if t.IsFSharpRecord then
    printfn "Record - %A" <| t.DisplayName

    for field in t.FSharpFields do
      printfn "Member - %A" field.DisplayName
      printfn "Type - %A" field.FieldType

  if t.IsFSharpUnion then
    printfn "Union - %A" <| t.DisplayName

    for field in t.UnionCases do
      printfn "Member - %A" field.Name
      printfn "Has fields - %A" field.HasFields

      for f in field.Fields do
        makeType f.FieldType

  t.GetPublicNestedEntities() |> Seq.map getType |> Seq.toList |> ignore

  ignore 0

partialAssemblySignature.TryGetEntities() |> Seq.map getType |> Seq.toList

0
