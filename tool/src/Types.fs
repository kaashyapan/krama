module rec Krama.Types

open Krama.Log
open FSharpx

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
  | DateTime
  | DateOnly
  | TimeSpan
  | TimeOnly
  | DateTimeOffset
  | Decimal
  | Byte
  | SByte
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
  | AnonRec of (string * T) list
  | RecordMember of string * T
  | Record of T list
  | AnonRecord of T list
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
  | T.Option t -> getDepTypes ( topType :: acc, alltypes) t
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
  | T.AnonRec tlist -> tlist |> List.map (fun (_, t) -> t) |> List.fold getDepTypes (topType :: acc, alltypes)
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
