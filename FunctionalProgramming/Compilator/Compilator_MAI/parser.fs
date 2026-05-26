open FParsec
open System.IO
open Interpreter.Interpreter

module Parser = 
    let Parse, ParseRef = createParserForwardedToRef()

    let ParseFloat = pfloat .>> spaces |>> Float

    let ParseBool =
        pstring "true" <|> pstring "false"  .>> spaces |>> 
            function
                | "true" -> Bool(true)
                | "false" -> Bool(false)
                | _ -> failwith "value error"

    let ID =
        let isFirstSymID sym = isLetter sym || sym = '_'
        let isSymID sym = isLetter sym || isDigit sym || sym = '_'

        many1Satisfy2L isFirstSymID isSymID "Invalid name of func / variable" .>> spaces |>> Variable

    let operators = ["&&"; "||"; "!"; "+"; "-"; "*"; "/"; "="; "<>"; ">"; "<"; ">="; "<="]
    let Operator = choice (List.map (fun op -> pstring op |>> fun _ -> op) operators)

    let Func =
        (pstring "fun" >>. spaces >>. ID) .>>. 
        (skipChar '(' >>. spaces  >>. many ID .>> skipChar ')') .>>. 
        (spaces >>. skipChar '{' >>. spaces >>. many Parse .>> spaces .>> skipChar '}' .>> spaces) .>>. 
        (many Parse .>> spaces) |>>
            fun(((Variable(func), param), body), expr_list) -> 
                let valid_params = 
                    param |> List.map(
                                function
                                    |Variable(x) -> x
                                    | _ -> failwith "error"
                            )

                Let(func, Lambda(valid_params, Prog(body)), Prog(expr_list))

    let RecursiveFunc =
        (pstring "rec fun" >>. spaces >>. ID) .>>. 
        (skipChar '(' >>. spaces  >>. many ID .>> skipChar ')') .>>. 
        (spaces >>. skipChar '{' >>. spaces >>. many Parse .>> spaces .>> skipChar '}' .>> spaces) .>>. 
        (many Parse .>> spaces) |>> 
            fun(((Variable(func), param), body), expr_list) -> 
                let valid_params = 
                    param |> List.map(
                        function
                            |Variable(x) -> x
                            | _ -> failwith "error"
                )

                LetRec(func, Lambda(valid_params, Prog(body)), Prog(expr_list))

    let Variable = 
        pstring "let" >>. spaces >>. ID .>>. 
        (spaces >>. skipChar '=' >>. spaces >>. Parse) .>> 
        spaces .>>. many Parse .>> spaces
        |>> fun ((Variable(name), value), expr_list) -> 
            Let(name, value, Prog(expr_list)) 

    let ParseList = 
        skipChar '[' >>. spaces >>. many Parse .>> spaces .>> skipChar ']' .>> spaces 
        |>> List

    let ApplicationFunc =
        ID .>>. 
        (spaces >>. skipChar '(' >>. spaces >>. many Parse .>> spaces .>> skipChar ')' .>> spaces) 
        |>> fun(func, arg) -> 
            Composition(func, List(arg))

    let Operation = 
        (ParseFloat <|> attempt ApplicationFunc <|> ID <|> ParseBool) .>> spaces .>>. 
        Operator .>> spaces .>>. 
        (ParseFloat <|> attempt ApplicationFunc <|> ID <|> ParseBool) .>> spaces 
        |>> fun ((e1, op), e2) -> 
            Composition(Composition(PrimitiveOperation(op),  e1),  e2)

    let IndexList = 
        ID .>>.
        (spaces >>. skipChar '[' >>. spaces >>. pint32 .>> spaces .>> skipChar ']' .>> spaces) 
        |>> fun(list, num) -> 
            Composition(Composition(PrimitiveOperation("[]"), list), Int(num))

    let Method = 
        ID .>> spaces .>>.
        (skipChar '.' >>. ID .>> spaces) .>>.
        (skipChar '(' >>. many Parse .>> spaces .>> skipChar ')' .>> spaces) 
        |>> fun((list, Variable(method)), args) -> 
            let acc = Composition(PrimitiveOperation(method), list)
            List.fold (fun acc x -> Composition(acc, x)) acc args 
            
    let IfThenElse =
        (pstring "if" >>. spaces >>. Parse .>> spaces) .>>.
        (pstring "then" >>. spaces >>. skipChar '{' >>. spaces >>. many Parse .>> spaces .>> skipChar '}' .>> spaces) .>>.
        (pstring "else" >>. spaces >>. skipChar '{' >>. spaces >>. many Parse .>> spaces .>> skipChar '}' .>> spaces) 
        |>> fun ((cond, res), alt) -> 
            IfThenElse(cond, Prog(res), Prog(alt))

    let ParsePrint =
        pstring "print" >>. spaces >>. skipChar '(' >>. spaces >>. many Parse .>> spaces .>> skipChar ')' .>> spaces 
        |>> fun(arg) -> 
            Print(Prog(arg))

    ParseRef := 
        choice [
            RecursiveFunc
            Func
            ParsePrint
            IfThenElse
            attempt Operation
            attempt ApplicationFunc 
            attempt IndexList 
            attempt Method
            Variable
            ParseList 
            ParseFloat
            ParseBool
            ID
        ]


    let final = 
        spaces >>. many Parse .>> eof |>> Prog

    let readFile filePath =
        try
            File.ReadAllText(filePath)
        with
        | :? FileNotFoundException ->
            failwithf "File not found: %s" filePath
        | ex ->
            failwithf "An error occurred while reading the file: %s" ex.Message




[<EntryPoint>]
let main argv =
    if argv.Length <> 1 then
        printfn "ERROR: please provide one argument - name of the file containing the code"
    else
        let filename = argv.[0]
        
        if File.Exists(filename) then
            let content = Parser.readFile filename
            match run Parser.final content with
            | Success(result,_,_) -> 
                eval result Map.empty |> ignore
                ()
            | Failure(err,_,_) -> 
                printfn "%A" err
        else
            printfn "ERROR: File '%s' does not exist." filename
    0
