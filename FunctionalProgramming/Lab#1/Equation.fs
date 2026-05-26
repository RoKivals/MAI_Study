let ctg = fun (x : float) -> 1. / tan (x)

let first_equation = fun x -> tan (x / 2.) - ctg (x / 2.) + x 
let second_equation = fun x -> 0.4 + atan (sqrt(x)) - x
let third_equation = fun x -> 3. * sin(sqrt(x)) + 0.35 * x - 3.8

// Differential of functions to Newton method
let first_diff = fun x -> 1. / (sin x) + 1.
let second_diff = fun x ->  1. / (2. * sqrt(x) * (x + 1.)) - 1.
let third_diff = fun x -> 3. * cos(sqrt(x)) / (2. * sqrt(x)) + 0.35

// Phi-functions to iterations method
let first_phi = fun x -> atan(ctg(x / 2.) - x) * 2.
let second_phi = fun x -> 0.4 + atan(sqrt(x))
let third_phi = fun x -> (-3. * sin(sqrt(x)) + 3.8) / 0.35 

let epsilon = 0.1 ** 5

let bisection func (left : float) (right : float) = 
    let rec iter left right = 
        let mid = (left + right) / 2.
        if (func mid = 0.) || ((mid - left) < epsilon) then mid
        elif (func left) * (func mid) < 0. then iter left mid 
        else iter mid right
    iter left right

let iterations phi x0 = 
    let rec iter x =
        let x' = phi x
        if abs(x - x') < epsilon then x'
        else iter x'
    iter x0

let newton_method func diff_func (x0: float) =
    let phi = fun x -> x - (func x) / (diff_func x)
    iterations phi x0 
    

printfn $"Решение уравнений с применением методам бисекции:
     1) {bisection first_equation 1 2} 
     2) {bisection second_equation 1 2} 
     3) {bisection third_equation 2 3}\n"

printfn $"Решение уравнений с применением методам итераций:
     1) {iterations first_phi 1.} 
     2) {iterations second_phi 1.} 
     3) {iterations third_phi 2.}\n"

printfn $"Решение уравнений с применением методам Ньютона: 
     1) {newton_method first_equation first_diff 1 } 
     2) {newton_method second_equation second_diff 1} 
     3) {newton_method third_equation third_diff 2}\n"