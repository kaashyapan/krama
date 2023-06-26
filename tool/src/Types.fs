module rec Krama.Types

open Krama.Log
open Krama.Common

[<RequireQualifiedAccess>]
type T =
  | Char
  | String
  | Int8
  | Int16
  | Int32
  | Int64
  | Int128
  | UInt8
  | UInt16
  | UInt32
  | UInt64
  | UInt128
  | Half
  | Single
  | Double
  | Guid
  | Bool
  | Decimal
  | Byte
  | SByte
  | DateTime
  | DateOnly
  | TimeSpan
  | TimeOnly
  | DateTimeOffset
  | Option of T
  | VOption of T
  | Tuple of T list
  | Map of T list
  | List of T
  | Array of T
  | Seq of T
  | Set of T
  | Alias of string * T //Access path * type
  | Userdef of string
  //| AnonRec of (string * T) list
  | RecordMember of string * T
  | Record of T list
  | AnonRecord of T list
  | UnionSimple of T list
  | Union of T list
  | UnionMember of string * T list
  | Choice of T list
  | Result of T list
  | Exception
  | TypeNotSupported

let rec getDepTypes (acc: T list, alltypes: T List) (topType: T) : (T List * T list) =
  match topType with
  | T.Userdef str ->
    let dependencies = str |> getTypeHeirarchy alltypes
    let acc' = acc @ dependencies
    acc', alltypes
  | T.Option t -> getDepTypes (topType :: acc, alltypes) t
  | T.VOption t -> getDepTypes (topType :: acc, alltypes) t
  | T.List t -> getDepTypes (topType :: acc, alltypes) t
  | T.Seq t -> getDepTypes (topType :: acc, alltypes) t
  | T.Array t -> getDepTypes (topType :: acc, alltypes) t
  | T.Set t -> getDepTypes (topType :: acc, alltypes) t
  | T.Map tlist -> List.fold getDepTypes (topType :: acc, alltypes) tlist
  | T.Tuple tlist -> List.fold getDepTypes (topType :: acc, alltypes) tlist
  | T.Result tlist -> List.fold getDepTypes (topType :: acc, alltypes) tlist
  | T.Choice tlist -> List.fold getDepTypes (topType :: acc, alltypes) tlist
  | T.Record tlist -> List.fold getDepTypes (topType :: acc, alltypes) tlist
  | T.Union tlist -> List.fold getDepTypes (topType :: acc, alltypes) tlist
  | T.AnonRecord tlist -> List.fold getDepTypes (topType :: acc, alltypes) tlist
  | T.UnionMember(_, tlist) -> List.fold getDepTypes (topType :: acc, alltypes) tlist
  | T.RecordMember(_, t) -> getDepTypes (topType :: acc, alltypes) t
  (**
  | T.AnonRec tlist ->
    tlist
    |> List.map (fun (_, t) -> t)
    |> List.fold getDepTypes (topType :: acc, alltypes)
  *)
  | T.Alias(str, t) -> getDepTypes (acc, alltypes) t
  | _ -> (acc, alltypes)

let getTypeHeirarchy (alltypes: T list) (typeName: string) : T list =
  alltypes
  |> List.where (fun t ->
    match t with
    | T.Alias(str, t') -> str = typeName
    | _ -> false
  )
  |> List.tryHead
  |> function
    | Some t ->
      let (dependencies, _) = t |> getDepTypes ([ t ], alltypes)
      dependencies |> List.where (fun t -> t <> T.TypeNotSupported) |> List.distinct
    | None ->
      log (Log.Err $"{typeName} was not found in any file")
      [ T.TypeNotSupported ]

let rec stringifyType (t: T) =
  match t with
  | T.Char -> "char"
  | T.String -> "string"
  | T.Int8 -> "int8"
  | T.Int16 -> "int16"
  | T.Int32 -> "int32"
  | T.Int64 -> "int64"
  | T.Int128 -> "int128"
  | T.UInt8 -> "uint8"
  | T.UInt16 -> "uint16"
  | T.UInt32 -> "uint32"
  | T.UInt64 -> "uint64"
  | T.UInt128 -> "uint128"
  | T.Half -> "half"
  | T.Single -> "single"
  | T.Double -> "double"
  | T.Guid -> "System.Guid"
  | T.Bool -> "bool"
  | T.Decimal -> "System.Decimal"
  | T.Byte -> "byte"
  | T.SByte -> "sbyte"
  | T.DateTime -> "System.DateTime"
  | T.DateOnly -> "System.DateOnly"
  | T.TimeSpan -> "System.TimeSpan"
  | T.TimeOnly -> "System.TimeOnly"
  | T.DateTimeOffset -> "System.DateTimeOffset"
  | T.Option t -> (stringifyType t) + " option"
  | T.VOption t -> (stringifyType t) + " voption"
  | T.Tuple tlist -> tlist |> List.map stringifyType |> String.join " * "
  | T.Map tlist -> ""
  | T.List t -> (stringifyType t) + " list"
  | T.Array t -> (stringifyType t) + " array"
  | T.Seq t -> (stringifyType t) + " seq"
  | T.Set t -> (stringifyType t) + " set"
  | T.Alias(name, typ) -> name
  | T.Userdef name -> name
  | T.RecordMember(name, typ) -> $"{name}: " + stringifyType typ
  | T.Record tlist ->
    let members = tlist |> List.map stringifyType |> String.join " ; "
    "{ " + members + " }"
  | T.AnonRecord tlist ->
    let members = tlist |> List.map stringifyType |> String.join " ; "
    "{| " + members + " |}"
  | T.UnionSimple tlist -> ""
  | T.Union tlist -> ""
  | T.UnionMember(name, tlist) -> ""
  | T.Choice tlist ->
    let members = tlist |> List.map stringifyType |> String.join ", "
    "Choice<" + members + ">"
  | T.Result tlist ->
    let members = tlist |> List.map stringifyType |> String.join ", "
    "Result<" + members + ">"
  | T.Exception -> "exn"
  | T.TypeNotSupported -> "NotSupported"
