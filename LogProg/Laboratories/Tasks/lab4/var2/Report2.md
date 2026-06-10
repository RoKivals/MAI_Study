#№ Отчет по лабораторной работе №4
## по курсу "Логическое программирование"

## Обработка естественного языка

### студент: Мхитарян С.А.

## Результат проверки

| Преподаватель     | Дата         |  Оценка       |
|-------------------|--------------|---------------|
| Сошников Д.В. |              |               |
| Левинская М.А.|  26.12.17    |    5-         |

> *Комментарии проверяющих (обратите внимание, что более подробные комментарии возможны непосредственно в репозитории по тексту программы)*
Включите в отчет описание грамматики (четверка: терминальный нетерминальный алфавит, правила перехода, начальный символ).
## Введение

Подходы обработки естественных языков:

* Предварительный анализ - выделение абзацев, предложений, слов
* Морфологический анализ - установление форм слова и приписывание им морфологических признаков
* Синтаксический анализ - анализ строения предложения и так же выделяются зависимости между словами.
* Семантический анализ - определение смысла слова

Слова в предложении представляются в виде списка. Существуют какие-то правила, для обработки предложения --- предикаты в Prolog. Допустим, как у меня в задании есть цвета, артикли, существительные, размер. Из-за этого Prolog очень подходит для решения задач обработки естественных и искуственных языков.

## Задание

Реализовать разбор предложений английского языка. В предложениях у объекта(подлежащего) могут быть заданы цвет, размер, положение. В результате разбора должны получиться структуры представленные в примере.

Вопрос:
```prolog
?- sentence(["The", "big", "book", "is", "under", "the", "table"], X).
?- sentence(["The", "red", "book", "is", "on", "the", "table"], X).
?- sentence(["The", "little", "pen", "is", "red"], X).
```
Ответ:
```prolog
X = s(location(object(book, size(big)), under(table))).
X = s(location(object(book, color(red)), on(table))).
X = s(object(pen, size(little)), color(red)).
```
## Принцип решения

АЛГОРИТМ:
1.  Разбиваем на составные части: артикли, существительные, размер, цвет.
2.  Задаем правила распознавания.
<br>В зависимости от наличия составных частей, ответ будет выглядеть по-разному.
Первым будет идти объект, с которым связаны его характеристики: цвет, размер. После будет идти предлог , и, если предлог указывает на место объекта в пространстве относительно другого объекта, то после характеристик выводим предлог и объект, относительно которого данный предлог уместен.


```prolog
art(a).
art(an).
art(the).
item(book).
item(disk).
item(lamp).
item(pen).
item(bottle).
item(table).
color(red).
color(green).
color(blue).
color(yellow).
color(orange).
size(little).
size(medium).
size(big).
location(in, X, in(X)).
location(on, X, on(X)).
location(under, X, under(X)).
location(behind, X, behind(X)).
location(before, X, before(X)).
location(after, X, after(X)).
analize([H], s(H)).
analize([A, B], s(A, B)).
analize([Art, Size, Item | T], Res):-
    art(Art),
    size(Size),
    item(Item),
    analize([object(Item, size(Size)) | T], Res).
analize([Art, Color, Item | T], Res):-
    art(Art),
    color(Color),
    item(Item),
    analize([object(Item, color(Color)) | T], Res).
analize([object(Item, size(Size)), is, X, Y, Z | T], Res):-
    art(Y),
    item(Z),
    location(X, Z, Loc),
    analize([location(object(Item, size(Size)), Loc) | T], Res).
analize([object(Item, color(Color)), is, X, Y, Z | T], Res):-
    art(Y),
    item(Z),
    location(X, Z, Loc),
    analize([location(object(Item, color(Color)), Loc) | T], Res).
analize([object(Item, size(Size)), is, X | T], Res):-
    color(X),
    analize([object(Item, size(Size)), color(X) | T], Res).
analize([object(Item, color(Color)), is, X | T], Res):-
    size(X),
    analize([object(Item, color(Color)), size(X) | T], Res).
```
## Результаты
```prolog
?- analize([the, big, book, is, under, the, table], X).
X = s(location(object(book, size(big)), under(table))) .
?- analize([the, red, book, is, on, the, table], X).
X = s(location(object(book, color(red)), on(table))) .
?- analize([a, medium, lamp, is, on, the, table], X).
X = s(location(object(lamp, size(medium)), on(table))) .
?- analize([the, little, pen, is, red], X).
X = s(object(pen, size(little)), color(red)) .
?- analize([a, red, disk, is, behind, the, lamp], X).
X = s(location(object(disk, color(red)), behind(lamp))) .
?- analize([the, little, bottle, is, blue], X).
X = s(object(bottle, size(little)), color(blue)) .
?- analize([the, yellow, pen, is, in, a, bottle], X).
X = s(location(object(pen, color(yellow)), in(bottle))) .
```
## Выводы

В ходе проделанной лабораторной работы я получил навык работы с методами анализа естественно-языковых текстов. Язык программирования Пролог – мощный 
инструмент для грамматического разбора предложений. Он предлагает совершенно иные возможности для написания программ, нежели популярные императивные и 
функциональные языки программирования.