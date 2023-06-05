module Program

open Expecto

[<EntryPoint>]
let main _ =
  (**
  let tests1 = testList "Type checker tests" Typechecker.tests
  (runTestsWithCLIArgs [] [||] tests1) |> ignore

  let tests2 = testList "Heirarchy tests" Heirarchy.tests
  (runTestsWithCLIArgs [] [||] tests2) |> ignore
  *)

  let tests3 = testList "Multifile tests" MultiFile.tests
  (runTestsWithCLIArgs [] [||] tests3) |> ignore

  0
