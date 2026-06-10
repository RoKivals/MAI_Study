#№ Отчет по лабораторной работе №3
## по курсу "Логическое программирование"

## Решение задач методом поиска в пространстве состояний

### студент: Саженов К.С.

## Результат проверки

| Преподаватель     | Дата         |  Оценка       |
|-------------------|--------------|---------------|
| Сошников Д.В. |              |               |
| Левинская М.А.|              |               |

> *Комментарии проверяющих (обратите внимание, что более подробные комментарии возможны непосредственно в репозитории по тексту программы)*

## Введение

Решение многих задач в интеллектуальных системах можно определить как проблему поиска, где искомое решение – это цель поиска, а множество возможных путей достижения цели представляет собой пространство состояний. Поиск решений в пространстве состоит в определении последовательности операторов, которые преобразуют начальное состояние в конечное.

Пространство состояний можно представить как граф, вершины которого помечены состояниями, а дуги - операторами. Получается, что такие задачи сводятся к задаче поиска в графе. Если два состояния связаны, то возможен переход системы из одного состояния в другое. Для решения такой задачи, я использовал поиск в глубину, поиск в ширину и поиск с итеративным заглублением.

Для представление графа в программировании обычно используют матричное представление, где граф задается своей матрицей смежности. В Прологе граф описывается предикатами - путем явного перечисления всех дуг в виде пар вершин. Задание графа при помощи дуг является более гибким, чем матрица смежности, поскольку дуги могут задаваться не только явным перечислением, но и при помощи правил, что позволяет нам описывать очень сложные и большие графы, для которых матричное представление нерационально и вообще не всегда возможно.

## Задание

4. "Расстановка мебели". Площадь разделена на шесть квадратов, пять из них заняты мебелью, шестой - свободен. Переставить мебель так, чтобы шкаф и кресло поменялись местами, при этом никакие два предмета не могут стоять на одном квадрате.

## Принцип решения

Я представил поле перемещения оьъектов в виде единственного списка, в котором первые три элемента равны первой строке, а вторые три - второй соответственно. И выписал все возможные перемещения в данном поле:
```prolog
possbble_movement([-, B, C, D, E, F],[B, -, C, D, E, F]).
possbble_movement([-, B, C, D, E, F],[D, B, C, -, E, F]).
possbble_movement([A, -, C, D, E, F],[A, C, -, D, E, F]).
possbble_movement([A, -, C, D, E, F],[A, E, C, D, -, F]).
possbble_movement([A, B, -, D, E, F],[A, B, F, D, E, -]).
possbble_movement([A, B, C, -, E, F],[A, B, C, E, -, F]).
possbble_movement([A, B, C, D, -, F],[A, B, C, D, F, -]).
```
Предикат move отражает переход между состояниями в графе. Все возможные переходы реализованы в предикате possible_movement. Список, представляющий одно состояние, заменяется на список, в котором элемент пустоты может меняться либо с крайними относительно себя, либо с верхним или нижним элементом относительно себя, что позволяет получить другое состояние.

```prolog
move(X,Y) :-
    possbble_movement(X,Y);
    possbble_movement(Y,X).
```


Я реализовал три алгоритма решения: поиск в глубину. поиск в ширину и поиск с итеративным заглублением. Они отражены в предикатах search_dpth, search_brdth и search_iter соответственно.

Поиск в глубину основан на рекурсивном заглублении в дереве, пока не будет достигнут лист или необходимый элемент. При достиженни листа, алгоритм будет возвращаться на предыдущие вершины, пока не найдет вершину с необработанными детьми:
```prolog
% Основной предикат преобразования для поиска в глубину
dpth([Finish|Tail], Finish, [Finish|Tail]).
dpth(TempWay, Finish, Way) :-
    prolong(TempWay, NewWay),
    dpth(NewWay, Finish, Way).
% Поиск в глубину(изначальный список, список после обработки)
% Замеряет время и выводит шаги работы алгоритма
search_dpth(Start, Finish) :-
    write('~DEPTH SEARCH~'), nl,
    get_time(TIME1),
    dpth([Start], Finish, Way),
    pprint(Way),
    get_time(TIME2),
    write('~DEPTH SEARCH END~'), nl, nl,
    T1 is TIME2 - TIME1,
    write('Time is '),
    write(T1), nl, nl, !.
```
Поиск в ширину включает в себя последовательный обход элементов на одном уровне дерева и перемещении на следующий в случае отсутствия необходимого элемента на данном уровне. Главным преимуществом данного алгоритма ялвяется то, что число итераций не будет превышать число итераций алгоритма поиска в глубину. Также этот алгоритм является не очень эффективным по потреблению памяти в сильноветвящихся графах, но всегда находит наикратчайший путь к решению:
```prolog
% Основной предикат преобразования для поиска в ширину
brdth([[Finish | Tail] | _], Finish, [Finish | Tail]).
brdth([TempWay | OtherWays], Finish, Way) :-
    findall(Z, prolong(TempWay, Z), Ways),
    append(OtherWays, Ways, NewWays),
    brdth(NewWays, Finish, Way).
brdth([_ | Tail], Y, List) :- brdth(Tail, Y, List).
% Поиск в ширину(изначальный список, список после обработки)
% Замеряет время и выводит шаги работы алгоритма
search_brdth(Start, Finish) :-
    write('~BREADTH SEARCH~'), nl,
    get_time(TIME1),
    brdth([[Start]], Finish, Way),
    pprint(Way),
    get_time(TIME2),
    write('~BREADTH SEARCH END~'), nl, nl,
    T1 is TIME2 - TIME1,
    write('Time is '),
    write(T1), nl, nl, !.
```
Поиск с итерационным заглублением собрал в себя все лучшее от алгоритмов поиска в глубину и ширину. Он осуществляет поиск в глубину до достижения определенной степени погружения. Вложенность для первой итерации равна единице, для каждой последующей это число увеличивается на единицу. Можно сказать, что данный поиск - поиск в ширину, который углубляется не на один элемент, а на количество элементов, определенное номером итерации:
```prolog
% Основной предикат преобразования для поиска с итерационным заглублением
iter([Finish | Tail], Finish, [Finish | Tail], 0).
iter(TempWay, Finish, Way, N) :-
    N > 0,
    prolong(TempWay, NewWay),
    M is N - 1,
    iter(NewWay, Finish, Way, M).
% Поиск с итерационным заглублением(изначальный список, список после обработки)
% Замеряет время и выводит шаги работы алгоритма
search_iter(Start, Finish, Way) :-
    integeres(Level),
    iter([Start], Finish, Way, Level).
search_iter(Start, Finish) :-
    write('~ITER SEARCH~'), nl,
    get_time(TIME1),
    search_iter(Start, Finish, Way),
    pprint(Way),
    get_time(TIME2),
    write('~ITER SEARCH END~'), nl, nl,
    T1 is TIME2 - TIME1,
    write('Time is '),
    write(T1), nl, nl, !.
```
Рассмотрим часть, общую для всех трех алгоритмов. Предикат prolong нужен, чтобы продлить все пути в графе, предотвращая зацикливания:
```prolog
% Продление пути без зацикливания
prolong([Temp|Tail], [New, Temp|Tail]) :-
    move(Temp, New),
    not(member(New,[Temp|Tail])).
```



## Результаты

```prolog
?- search_dpth([table, chair, wardrobe, chair, -, armchair], [table, chair, armchair, chair, -, wardrobe]).
~DEPTH SEARCH~
[table,chair,wardrobe,chair,-,armchair]
[table,chair,wardrobe,chair,armchair,-]
[table,chair,-,chair,armchair,wardrobe]
[table,-,chair,chair,armchair,wardrobe]
[table,armchair,chair,chair,-,wardrobe]
[table,armchair,chair,chair,wardrobe,-]
[table,armchair,-,chair,wardrobe,chair]
[table,-,armchair,chair,wardrobe,chair]
[table,wardrobe,armchair,chair,-,chair]
[table,wardrobe,armchair,chair,chair,-]
[table,wardrobe,-,chair,chair,armchair]
[table,-,wardrobe,chair,chair,armchair]
[-,table,wardrobe,chair,chair,armchair]
[chair,table,wardrobe,-,chair,armchair]
[chair,table,wardrobe,chair,-,armchair]
[chair,table,wardrobe,chair,armchair,-]
[chair,table,-,chair,armchair,wardrobe]
[chair,-,table,chair,armchair,wardrobe]
[chair,armchair,table,chair,-,wardrobe]
[chair,armchair,table,chair,wardrobe,-]
[chair,armchair,-,chair,wardrobe,table]
[chair,-,armchair,chair,wardrobe,table]
[chair,wardrobe,armchair,chair,-,table]
[chair,wardrobe,armchair,chair,table,-]
[chair,wardrobe,-,chair,table,armchair]
[chair,-,wardrobe,chair,table,armchair]
[-,chair,wardrobe,chair,table,armchair]
[chair,chair,wardrobe,-,table,armchair]
[chair,chair,wardrobe,table,-,armchair]
[chair,chair,wardrobe,table,armchair,-]
[chair,chair,-,table,armchair,wardrobe]
[chair,-,chair,table,armchair,wardrobe]
[chair,armchair,chair,table,-,wardrobe]
[chair,armchair,chair,table,wardrobe,-]
[chair,armchair,-,table,wardrobe,chair]
[chair,-,armchair,table,wardrobe,chair]
[chair,wardrobe,armchair,table,-,chair]
[chair,wardrobe,armchair,-,table,chair]
[-,wardrobe,armchair,chair,table,chair]
[wardrobe,-,armchair,chair,table,chair]
[wardrobe,armchair,-,chair,table,chair]
[wardrobe,armchair,chair,chair,table,-]
[wardrobe,armchair,chair,chair,-,table]
[wardrobe,-,chair,chair,armchair,table]
[wardrobe,chair,-,chair,armchair,table]
[wardrobe,chair,table,chair,armchair,-]
[wardrobe,chair,table,chair,-,armchair]
[wardrobe,-,table,chair,chair,armchair]
[wardrobe,table,-,chair,chair,armchair]
[wardrobe,table,armchair,chair,chair,-]
[wardrobe,table,armchair,chair,-,chair]
[wardrobe,table,armchair,-,chair,chair]
[-,table,armchair,wardrobe,chair,chair]
[table,-,armchair,wardrobe,chair,chair]
[table,armchair,-,wardrobe,chair,chair]
[table,armchair,chair,wardrobe,chair,-]
[table,armchair,chair,wardrobe,-,chair]
[table,-,chair,wardrobe,armchair,chair]
[table,chair,-,wardrobe,armchair,chair]
[table,chair,chair,wardrobe,armchair,-]
[table,chair,chair,wardrobe,-,armchair]
[table,-,chair,wardrobe,chair,armchair]
[-,table,chair,wardrobe,chair,armchair]
[wardrobe,table,chair,-,chair,armchair]
[wardrobe,table,chair,chair,-,armchair]
[wardrobe,table,chair,chair,armchair,-]
[wardrobe,table,-,chair,armchair,chair]
[wardrobe,-,table,chair,armchair,chair]
[wardrobe,armchair,table,chair,-,chair]
[wardrobe,armchair,table,chair,chair,-]
[wardrobe,armchair,-,chair,chair,table]
[wardrobe,-,armchair,chair,chair,table]
[wardrobe,chair,armchair,chair,-,table]
[wardrobe,chair,armchair,chair,table,-]
[wardrobe,chair,-,chair,table,armchair]
[wardrobe,-,chair,chair,table,armchair]
[-,wardrobe,chair,chair,table,armchair]
[chair,wardrobe,chair,-,table,armchair]
[chair,wardrobe,chair,table,-,armchair]
[chair,wardrobe,chair,table,armchair,-]
[chair,wardrobe,-,table,armchair,chair]
[chair,-,wardrobe,table,armchair,chair]
[chair,armchair,wardrobe,table,-,chair]
[chair,armchair,wardrobe,table,chair,-]
[chair,armchair,-,table,chair,wardrobe]
[chair,-,armchair,table,chair,wardrobe]
[chair,chair,armchair,table,-,wardrobe]
[chair,chair,armchair,-,table,wardrobe]
[-,chair,armchair,chair,table,wardrobe]
[chair,-,armchair,chair,table,wardrobe]
[chair,armchair,-,chair,table,wardrobe]
[chair,armchair,wardrobe,chair,table,-]
[chair,armchair,wardrobe,chair,-,table]
[chair,-,wardrobe,chair,armchair,table]
[chair,wardrobe,-,chair,armchair,table]
[chair,wardrobe,table,chair,armchair,-]
[chair,wardrobe,table,chair,-,armchair]
[chair,-,table,chair,wardrobe,armchair]
[chair,table,-,chair,wardrobe,armchair]
[chair,table,armchair,chair,wardrobe,-]
[chair,table,armchair,chair,-,wardrobe]
[chair,table,armchair,-,chair,wardrobe]
[-,table,armchair,chair,chair,wardrobe]
[table,-,armchair,chair,chair,wardrobe]
[table,armchair,-,chair,chair,wardrobe]
[table,armchair,wardrobe,chair,chair,-]
[table,armchair,wardrobe,chair,-,chair]
[table,-,wardrobe,chair,armchair,chair]
[table,wardrobe,-,chair,armchair,chair]
[table,wardrobe,chair,chair,armchair,-]
[table,wardrobe,chair,chair,-,armchair]
[table,-,chair,chair,wardrobe,armchair]
[table,chair,-,chair,wardrobe,armchair]
[table,chair,armchair,chair,wardrobe,-]
[table,chair,armchair,chair,-,wardrobe]
~DEPTH SEARCH END~
Time is 0.00612330436706543
true.
?- search_brdth([table, chair, wardrobe, chair, -, armchair], [table, chair, armchair, chair, -, wardrobe]).
~BREADTH SEARCH~
[table,chair,wardrobe,chair,-,armchair]
[table,chair,wardrobe,chair,armchair,-]
[table,chair,-,chair,armchair,wardrobe]
[table,-,chair,chair,armchair,wardrobe]
[table,armchair,chair,chair,-,wardrobe]
[table,armchair,chair,-,chair,wardrobe]
[-,armchair,chair,table,chair,wardrobe]
[armchair,-,chair,table,chair,wardrobe]
[armchair,chair,-,table,chair,wardrobe]
[armchair,chair,wardrobe,table,chair,-]
[armchair,chair,wardrobe,table,-,chair]
[armchair,-,wardrobe,table,chair,chair]
[-,armchair,wardrobe,table,chair,chair]
[table,armchair,wardrobe,-,chair,chair]
[table,armchair,wardrobe,chair,-,chair]
[table,armchair,wardrobe,chair,chair,-]
[table,armchair,-,chair,chair,wardrobe]
[table,-,armchair,chair,chair,wardrobe]
[table,chair,armchair,chair,-,wardrobe]
~BREADTH SEARCH END~
Time is 0.1780538558959961
true.
?- search_iter([table, chair, wardrobe, chair, -, armchair], [table, chair, armchair, chair, -, wardrobe]).
~ITER SEARCH~
[table,chair,wardrobe,chair,-,armchair]
[table,chair,wardrobe,chair,armchair,-]
[table,chair,-,chair,armchair,wardrobe]
[table,-,chair,chair,armchair,wardrobe]
[table,armchair,chair,chair,-,wardrobe]
[table,armchair,chair,-,chair,wardrobe]
[-,armchair,chair,table,chair,wardrobe]
[armchair,-,chair,table,chair,wardrobe]
[armchair,chair,-,table,chair,wardrobe]
[armchair,chair,wardrobe,table,chair,-]
[armchair,chair,wardrobe,table,-,chair]
[armchair,-,wardrobe,table,chair,chair]
[-,armchair,wardrobe,table,chair,chair]
[table,armchair,wardrobe,-,chair,chair]
[table,armchair,wardrobe,chair,-,chair]
[table,armchair,wardrobe,chair,chair,-]
[table,armchair,-,chair,chair,wardrobe]
[table,-,armchair,chair,chair,wardrobe]
[table,chair,armchair,chair,-,wardrobe]
~ITER SEARCH END~
Time is 0.09709739685058594
true.
```

| Алгоритм поиска |  Длина найденного первым пути  |  Время работы  |
|-----------------|--------------------------------|-----------------|
| В глубину       |  115                           |0.0061233043670654|
| В ширину        |  19                            |0.1780538558959961|
| ID              |  19                            |0.0970973968505859|

## Выводы

Все три использованные мною алгоритма решили поставленную задачу.
Поиск в глубину является естественным для языка Пролог, он используется машиной вывода Пролога для вычисления целей. Поэтому поиск в глубину путей на графах реализуется в языке Пролог наиболее просто. Этот алгоритм означает, что из начального состояния мы делаем один некий шаг, дальше из нового состояния делаем еще один шаг и т.д., пока не дойдем до конечного состояния или до состояния, из которого нельзя больше сделать ни одного шага. В этом случае мы рекурсивно возвращаемся назад и снова делаем шаги из того состояния, в которое вернулись, пока не найдем решение. В итоге, процесс перебора в глубину характеризуется тем, что вначале раскрывается та вершина, которая была построена самой последней.
Алгоритм поиска в глубину оказался самым быстрым из трех реализованных алгоритмов, но путь, который он нашел во много раз превышает путь, найденный алгоритмом поиска в ширину и алгоритмом поиска с итеративным заглублением. Если нужно было бы выбирать по критерию длины пути, то алгоритм поиска в глубину можно было бы сразу исключить из рассмотрения.

Поиск в ширину программируется не так легко, как поиск в глубину. Причина состоит в том, что нам приходится сохранять все множество альтернативных вершин-кандидатов, а не только одну вершину, как при поиске в глубину. Несложно заметить, что такой поиск в ширину имеет экспоненциальную сложность как по времени, так и по памяти. В лучшем случае решение  может найтись сразу, тогда можно избежать таких растрат, но зачастую это маловероятно.

Поиск с итеративным заглублением оказался чем-то средним между поиском в ширину и поиском в глубину как по длине пути, так и времени исполнения, так как является оптимизацией поиска в глубину и в ширину, которая гарантированно позволяет найти самое близкое к начальному состоянию решение, избегая экспоненциальной сложности. Длина пути, найденная алгоритмом поиска с итеративным погружением, равна длине пути, найденной с помощью поиска в ширину. Время, потраченное на поиск, находится где-то посередине относительно времени поиска в глубину и поиска в ширину. Поэтому, если рассматривать алгоритм с точки зрения отношения времени работы к длине пути, то алгоритм поиска с итеративным погружением самый эффективный из данных алгоритмов. В моей задаче не получилось доказать тот факт, что поиск с итеративным заглублением обычно работает быстрее поиска в глубину, но в типичных задачах алгоритм поиска с итеративным заглублением все же эффективнее во всех аспектах.
 
Какие алгоритмы поиска в каких случаях удобно использовать?

Поиск в глубину и поиск с итеративным заглублением стоит выбирать, если важен объем оперативной памяти и процессорное время, но также нужно обращать внимание на объем входных данных. Лучший показатель при больших входных данных будет все же именно у поиска с итеративным погружением. В номинации лучшего поиска по отношению времени исполнения к длине пути также побеждает поиск с итеративным заглублением. Если немалозначимую роль играет длина пути поиска решения, то стоит рассмотреть не только алгоритм поиска с итеративным заглублением, но и алгоритм поиска в ширину. Но, опять же, поиск с итеративным погружением и здесь намного обходит поиск в ширину по затраченному времени.

После выполнения данной лабораторной работы я пришел к выводу, что алгоритм поиска с итеративным заглублением, хоть и сложнее в реализации, чем поиск в ширину или глубину, зато он компенсирует данный недостаток своей высокой эффективностью для большинства задач.