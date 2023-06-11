namespace Foo

open System
open System.Runtime.CompilerServices
open Krama.Json

module Bar =
    type Encoders () =
        static member encode (value: Types.Cht) = 23
        static member encode (value: Types.Cht option) = 23
        static member encode (value: Types.Sex) = 23
        static member encode (value: Types.Sex option) = 23
        static member encode (value: Types.Pet) = 23
        static member encode (value: Types.Pet option) = 23
        static member encode (value: Types.Age) = 23
        static member encode (value: Types.Age option) = 23
        static member encode (value: Types.Address) = 23
        static member encode (value: Types.Address option) = 23
        static member encode (value: Model.Employee) = 23
        static member encode (value: Model.Employee option) = 23
        static member encode (value: Model.Person) = 23
        static member encode (value: Model.Person option) = 23

    [<Extension>]
    type Decoders () =
        [<Extension>]
        static member Name () = 23

    type Model.Person with

        static member ToJson (person: Model.Person) =
            let encoder = Encoders.encode person
            Json.serialize encoder person

        static member FromJson (jsonstr: string) = Json.parse Decoders.AsPerson jsonstr

    type Model.Employee with

        static member ToJson (employee: Model.Employee) =
            let encoder = Encoders.encode employee
            Json.serialize encoder employee

        static member FromJson (jsonstr: string) = Json.parse Decoders.AsEmployee jsonstr
