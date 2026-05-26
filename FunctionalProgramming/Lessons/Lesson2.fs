let p3 f x y z = f (f x y) z
p3 (+) 1 2 3

// Типы - создание собственных структур (типов)
type PersonAge = 
    | Exact of int
    | Description of string
let student = Description("Молодой")
let teacher = Exact(43)

// Частным случаем создания типа является Option Type (аналог option из c++)
// Тип может либо иметь значение, либо не иметь его вовсе. Ниже представлен ОБЩИЙ вид
// option <'T>
type 'T option = 
    | Some of 'T
    | None

type Result = 
    | NoSolution
    | Quad of float * float
    | Linear of float


// Теперь при решении квадратного уравнения, мы можем обработать случай, если D < 0
let solve (a, b, c) =
    let D = b * b - 4. * a * c // Точка после цифры, даёт понять, что переменная имеет значение float
    if a = 0. then 
        if b = 0. then NoSolution
        else Linear(-c / b)
    else
        if D < 0 then NoSolution
        else 
            let x1 = (-b + sqrt(D)) / (2. * a) 
            let x2 = (-b - sqrt(D)) / (2. * a)
            Quad(x1, x2) // возвращаемое значение


// Сравнение с образцом (оператор match). Можно сказать, что это более продвинутый аналог switch-case 
// function использует match неявно (поддерживает только 1 аргумент для сопоставления с образцом)
let print = function 
    // match r with 
    | NoSolution -> printfn "No solutions"
    | Linear(x) -> printfn $"Linear soluton: x = {x}"
    | Quad(x1, x2) when x1=x2 -> printfn $"Solution: x={x1}"
    | Quad(x1, x2) -> printfn $"Solutions: x1={x1}, x2={x2}"
    | _ -> printfn "Cringe" // Default-case


// Упорядочивание пары
let order = function
    | (x1, x2) when x1 > x2 -> (x2, x1)
    | _ as pair -> pair

// Рекурсивные функции
let rec factorial n = 
    if n=1 then 1
    else n * factorial(n - 1)

// Факториал, используя сопоставление с образцом
// Рекурсия создаётся с помощью rec
let rec factorial1 = function
    | 1 -> 1
    | n -> n * factorial1(n - 1)

let rec factorial_table a b = 
    if a < b then 
        printfn $"a: {factorial1 a}"
        factorial_table (a + 1) b
    else
        printfn "Final"
let rec table func a b = 
    if a < b then 
        printfn $"a: {func a}"
        table func (a + 1) b
    else
        printfn "Final"

let rec myfor func a b = 
    if a < b then
        func a
        myfor func (a + 1) b

let table_myfor func a b =
    myfor (fun x -> printfn $"x: {factorial(x)}") a b 

// Цикл с аккумулятором (счётчиком, накоплением). F# создан для работы с рекурсивными функциями, 
// в нём они работают кратно быстрее, чем в других языках.
let rec iter f iteration a b =
    if a > b then iteration
    else f a (iter f iteration (a + 1) b)

// iter (fun acc i -> acc * i) 1 1 6
// Сумма чисел
let sum a b = iter (+)  0

// Факториал
let fact = iter (*) 1 1

//Факториал больших чисел
let bigfactorial = iter (fun i acc -> acc * System.Numerics.BigInteger(i)) 1I 1

// Возведение в степень
let power x = iter (fun _ acc  -> acc * x) 1. 1

// Разложение в ряд тейлора
// Большая декомпозиция снижает эффективность и увеличивает кол-во ошибок
// В идале стоит использовать вычисление на основе предыдущего члена
let exp_teilor x = iter(fun i acc -> acc + (power x i) / (fact i|>float)) 0. 0 7


// В обычной рекурсии, мы раскручиваем стек вызовов до тех пор, пока не попадём в условие выхода из рекурсии,
// после чего, получив результат, мы возвращаемся в каждый из вызовов, дабы завершить его, зная нужное нам значение
// Память тратится на хранение ссылок между вызовами + хранение локальных переменных
// Хвостовая рекурсия (хорошая рекурсия) может быть преобразована в итерацию (цикл)
// Условия хвостовой рекурсии:
// 1) Линейная - для каждой итерации только один рекурсивный вызов
// 2) Рекурсивный вызов - последняя операция в теле функции
// 3) Нет локальных переменных

// Хвостовой факториал
let tail_fact =   
    let rec big_factorial_tail acc  = function
        | n when n = 1I -> acc
        | n -> big_factorial_tail (acc * n) (n - 1I)
    big_factorial_tail 1I


let rec tail_iter f iteration a b =
    if a > b then iteration
    else f a (iter f iteration (a + 1) b)