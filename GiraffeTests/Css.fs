module Tests

open Expecto
open System.IO

[<Tests>]
let tests =
  testList "Css" [
    testCase "Big Yellow" <| fun _ ->
      let bigYellow = Css.scopedClass "big-yellow" |> Css.mediaAll [
        Css.background "yellow"
        Css.fontSize "large"
      ]
      let head = Css.Head()
      let className = head.Add bigYellow
      printfn "class name = %s" className
      printfn "%s" (head.toStyleText())
      Expect.equal className (head.Add bigYellow) ""

    testCase "Generate code" <| fun _ ->
      use fout = new StreamWriter("../../../CssProperties.fs")
      File.ReadAllLines "../../../css-properties.txt"
      |> Array.iter (fun line ->
        line.Split('-')
        |> Array.mapi (fun i word ->
          match i with
          | 0 -> word
          | _ -> word.Substring(0, 1).ToUpper() + word.Substring(1)
        )
        |> String.concat ""
        |> fun name -> fprintfn fout "let %s = property \"%s\"" name line)
      
      ()   

    // testCase "when true is not (should fail)" <| fun _ ->
    //   let subject = false
    //   Expect.isTrue subject "I should fail because the subject is false"

    // testCase "I'm skipped (should skip)" <| fun _ ->
    //   Tests.skiptest "Yup, waiting for a sunny day..."

    // testCase "I'm always fail (should fail)" <| fun _ ->
    //   Tests.failtest "This was expected..."

    // testCase "contains things" <| fun _ ->
    //   Expect.containsAll [| 2; 3; 4 |] [| 2; 4 |]
    //                      "This is the case; {2,3,4} contains {2,4}"

    // testCase "contains things (should fail)" <| fun _ ->
    //   Expect.containsAll [| 2; 3; 4 |] [| 2; 4; 1 |]
    //                      "Expecting we have one (1) in there"

    // testCase "Sometimes I want to ༼ノಠل͟ಠ༽ノ ︵ ┻━┻" <| fun _ ->
    //   Expect.equal "abcdëf" "abcdef" "These should equal"

    // test "I am (should fail)" {
    //   "╰〳 ಠ 益 ಠೃ 〵╯" |> Expect.equal true false
    // }
  ]