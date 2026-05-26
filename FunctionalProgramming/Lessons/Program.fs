// let определяет какое-то новое имя, которому присваивается что-то (используется для запоминания). Понятия переменная в F# нет
// Значение происвоенное let - не изменяется
let twice x = x * 2

twice 5

// 5 * 2 * 2
twice (twice 5)

// Композиция функций
(twice >> twice) 5

// x |> func подаёт x на вход func 
5 |> twice |> twice
twice <| 5|> twice // аналогично пред. выглядит ужасно


let solve (a, b, c) =
    let D = b * b - 4. * a * c // Точка после цифры, даёт понять, что переменная имеет значение float
    let x1 = (-b + sqrt(D)) / (2. * a) 
    let x2 = (-b - sqrt(D)) / (2. * a)
    (x1, x2) // возвращаемое значение

// Для создания лямбда функций используется ключ слово fun.
// Такую функцию можн6о перадать в качестве аргумента
let f = fun x -> x * 2

// Каррирование. Возвращение функции внутри функции.
// Это аналогично передаче пары аргументов, просто передаются по очереди 
let plus = fun x -> fun y -> x + y
// По сути, один из двух аргументов всегда равен 1
let incr = plus 1

// Условный оператор. Похож на тернарный оператор из C-подобных языков.
// Возвращаемое значение должно быть одного типа.
// Должны использоваться, как if, так и else. 
// Однако, если нет возвращаемого значения (происходит какое-то действие и всё, то достаточно использовать then без else)
let min (x, y) = if x < y then x else y

let hello age = if age > 18 then printf "Welcome, boy"
hello 14


// Циклы. Каждая итерация работает, как отдельный вызов функции с переданным параметром счётчика
let table f = 
    printf " X    f(X)"
    for i = 0 to 10 do
        let x = 1. / float(i)
        printfn "%7.4f %7.4f" x (f x)


let rec repeat cnt func = 
    if cnt = 0 then fun x-> x
    else func >> repeat (cnt - 1) func

repeat 8 twice 1

// Явное указание типа передаваемого аргумента
let mandel_func (c:System.Numerics.Complex) (z: System.Numerics.Complex) = z * z + c


let is_fractal (c:System.Numerics.Complex)  = 
    System.Numerics.Complex.Abs(repeat 20 (mandel_func c) (System.Numerics.Complex.Zero)) < 1.0

// Масштабирование из одного интервала в другой
let scale (x: float, y: float) (u, v) n = 
    float(n - u) / float(v - u) * (y - x) + x

// Рисуем в консоли
for i = 1 to 40 do
    for j = 1 to 40 do
        let lscale = scale (-1.2, 1.2) (1, 40) in
        let t = new System.Numerics.Complex(lscale j, lscale i) in
        System.Console.Write(if is_fractal t then "*" else " ")
    System.Console.WriteLine("")

open System.Drawing
open System.Windows.Forms

let form =
    let image = new Bitmap(1080, 720)
    let lscale = scale (-1.2, 1.2) (0, min(image.Height, image.Width) - 1)
    for y = 0 to (image.Height - 1) do
        for x = 0 to (image.Width - 1) do
            let dot = new System.Numerics.Complex(lscale y, lscale x) in
                image.SetPixel(x, y, if is_fractal dot then Color.Black else Color.White)
    
    let temp = new Form(Visible = true)
    temp.Paint.Add(fun e -> e.Graphics.DrawImage(image, 0, 0))
    temp.Show()
    Application.Run(temp)


