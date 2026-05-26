namespace Interpreter

module Interpreter =

    type id = string
    type expr =
        | Variable of id
        | Lambda of id list*expr
        | Composition of expr*expr        
        | Bool of bool
        | Int of int
        | IfThenElse of expr*expr*expr
        | Float of float
        | List of expr list
        | Let of id*expr*expr
        | LetRec of id*expr*expr
        | PrimitiveOperation of id
        | Op of id*int*expr list
        | Closure of expr*env
        | RClosure of expr*env*id
        | Prog of expr list
        | Print of expr
        | None
    and
        env = Map<id,expr>

    let ArgsNum = 
        function
        | "&&" -> 2
        | "||" -> 2
        | "!" -> 1

        | "+" -> 2
        | "-" -> 2
        | "*" -> 2
        | "/" -> 2
        | "%" -> 2

        | "=" -> 2
        | "<>" -> 2
        | ">" -> 2
        | "<" -> 2
        | ">=" -> 2
        | "<=" -> 2

        | "[]" -> 2 
        | "front" -> 1
        | "back" -> 1
        | "append" -> 2
        | "len" -> 1
        | _ -> failwith "unknown operator error"

    let Operation = 
        function
        | "[]" -> function
            | [List(list); Int(n)] -> list.[n] 
            | _ -> failwith "type error"
        | "front" -> function
            | [List(list)] -> (List.head list)
            | _ -> failwith "type error"
        | "back" -> function
            | [List(list)] -> (List.last list)
            | _ -> failwith "type error"
        | "append" -> function
            | [List(list); x] -> List(list@[x])
            | _ -> failwith "type error"
        | "len" -> function
            | [List(list)] -> Float(List.length list)
            | _ -> failwith "type error"
            
        | "&&" -> function 
            | [Bool(a); Bool(b)] -> Bool(a&&b)
            | _ -> failwith "type error"
        | "||" -> function 
            | [Bool(a); Bool(b)] -> Bool(a||b)
            | _ -> failwith "type error"
        | "!" -> function 
            | [Bool(a)] -> Bool(not a)
            | _ -> failwith "type error"

        | "+" -> function 
            | [Float(a); Float(b)] -> Float(a+b)
            | _ -> failwith "type error"
        | "-" -> function 
            | [Float(a); Float(b)] -> Float(a-b)
            | _ -> failwith "type error"
        | "*" -> function 
            | [Float(a); Float(b)] -> Float(a*b)
            | _ -> failwith "type error"
        | "/" -> function 
            | [Float(a); Float(b)] -> Float(a/b)
            | _ -> failwith "type error"
        | "%" -> function
            | [Float(a); Float(b)] -> Float(a%b)
            | _ -> failwith "type error"
        | ">" -> function 
            | [Float(a); Float(b)] -> Bool(a>b)
            | _ -> failwith "type error"
        | "<" -> function 
            | [Float(a); Float(b)] -> Bool(a<b)
            | _ -> failwith "type error"
        | ">=" -> function 
            | [Float(a); Float(b)] -> Bool(a>=b)
            | _ -> failwith "type error"
        | "<=" -> function 
            | [Float(a); Float(b)] -> Bool(a<=b)
            | _ -> failwith "type error"
        | "=" -> function 
            | [Float(a); Float(b)] -> Bool(a=b)
            | _ -> failwith "type error"
        | "<>" -> function 
            | [Float(a); Float(b)] -> Bool(a<>b)
            | _ -> failwith "type error"
        | _ -> failwith "operator error"
 
        

    let rec eval exp env =
        match exp with
        | Composition(e1,e2) -> apply (eval e1 env) (eval e2 env)
        | Bool(n) -> Bool(n)
        | Int(n) -> Int(n)
        | Float(n) -> Float(n)
        | List(n) -> List(List.map(fun x -> eval x env) n)
        | Variable(x) -> Map.find x env
        | PrimitiveOperation(f) -> Op(f, ArgsNum f, [])
        | IfThenElse(cond, yes, no) ->
            if Bool(true) = eval cond env then eval yes env else eval no env
        | Op(id,n,el) -> Op(id,n,el)
        | Let(id, e1, e2) ->
            let r = eval e1 env in
                eval e2 (Map.add id r env)
        | LetRec(id, e1, e2) ->
            eval e2 (Map.add id (RClosure(e1, env, id)) env)
        | Lambda(param,body) -> Closure(exp, env)
        | Closure(exp, env) -> exp
        | Prog(exp_list) -> 
            if (List.isEmpty exp_list) then None
            else exp_list |> List.map(fun x -> eval x env) |> List.last
        | Print(x) -> 
            let res = eval x env
            match res with
            | Float(n) -> 
                printfn "%A" n
                None
            | Int(n) ->
                printfn "%A" n  
                None
            | Bool(n) -> 
                printfn "%A" n
                None
            | List(n) ->
                printfn "%A" n
                None
            | _ -> 
                printfn "the type does not support printing"
                None
        | _ -> 
            printfn "invalid evaluation"
            exp
    and apply e1 e2 = 
        match e1 with
        | Closure(Lambda(param,body),env) -> 
            match e2 with
            |List(value_list) -> 
                value_list |> ignore
                let env_add = List.zip param value_list |> Map.ofList
                let new_env = Map.fold(fun acc key value -> Map.add key value acc) env env_add
                eval body new_env
            | _ -> failwith "error"

        | RClosure(Lambda(param,body),env, id) -> 
            match e2 with
            |List(value_list) -> 
                value_list |> ignore
                let env_add = List.zip param value_list |> Map.ofList
                let new_env = Map.fold(fun acc key value -> Map.add key value acc) env env_add
                eval body (Map.add id e1 new_env)
            | _ -> failwith "error"
        | Op(id,n,args) ->
            if n=1 then (Operation id)(args@[e2])
            else Op(id, n-1, args@[e2])
        | _ -> failwith "error"
