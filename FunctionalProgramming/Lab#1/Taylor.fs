let orig_equation = fun (x: float) -> (2. * x - 3.) / (x - 1.) ** 2

let a = 0.1
let b = 0.6
let n = 10

let epsilon = 0.1 ** 9

let custom_pow (number: float) (grade: int)  = 
  let rec my_for func iteration a b =
    if a <= b then func a (my_for func iteration (a + 1) b)
    else iteration
  my_for (fun _ res -> res * number) 1. 1 grade


let elem_from_taylor (x: float) (n: int)  =  float(-(n + 3)) * (custom_pow x  n)

// Define a function to compute f using naive taylor series method
let taylor_naive x =
  let rec new_while curr_elem x sum n = 
    if abs(curr_elem) > epsilon then new_while (elem_from_taylor x (n + 1)) x (sum + curr_elem) (n + 1)
    else (sum, n)
  new_while (elem_from_taylor x 1) x 0. 1


let difference_elements (x : float) (n : float) = x * (n + 3.) / (n + 2.)

let first (left, right) = left
let second (left, right) = right


// Define a function to do the same in a more efficient way
let taylor x = 
  let rec new_while (curr_elem: float) (x: float) sum (n: int) = 
    if abs(curr_elem) > epsilon then new_while (curr_elem * difference_elements x n) x (sum + curr_elem) (n + 1)
    else (sum, n)
  new_while (elem_from_taylor x 1) x 0. 1


let main =
    printfn "--------------------------------------------------------------------------"
    printfn "|  x  |    f(x)    |   Naive    |    Iters    |   Smart    |    Iters    |"
    printfn "--------------------------------------------------------------------------"
    for i=0 to n do
      let x = a+(float i)/(float n)*(b-a)
      let res_naive = taylor_naive x
      let res_smart = taylor x
      printfn "|%5.2f|  %10.6f|  %10.6f|   %10d|  %10.6f|   %10d|" x (orig_equation x) (first res_naive) (second res_naive) (first res_smart) (second res_smart)
    printfn "--------------------------------------------------------------------------"

main