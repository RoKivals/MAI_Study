# Отчет по курсовому проекту
## по курсу "Логическое программирование"

### студент: <Орусский Вячеслав Русланович>

## Результат проверки

Вариант задания:

 - [x] стандартный, без NLP (на 3)
 - [ ] стандартный, с NLP (на 3-4)
 - [ ] продвинутый (на 3-5)
 
| Преподаватель     | Дата         |  Оценка       |
|-------------------|--------------|---------------|
| Сошников Д.В. |              |               |
| Левинская М.А.|              |               |

> *Комментарии проверяющих (обратите внимание, что более подробные комментарии возможны непосредственно в репозитории по тексту программы)*

## Введение

В процессе выполнения данного курсового проекта я изучил различные программы для построения родословного дерева, узнал новый модуль в Python, а также убедился в том, что Prolog порой уж очень тяжёл для простых задач.

## Задание
У меня 15 номер в группе и мне выпали 2-ой и 1-ый варианты в 2 и 3 задании соответственно.

То есть, от меня требовалось представить связи в дереве в виде двух предикатов (Родитель и пол), а также составить предикат нахождения Шурина (брат жены).

## Получение родословного дерева
В начале я столкнулся с выбором программы для создания дерева. MyHerritage - одна из самых популярных в мире, но, увы, заблокирована на территории РФ, поэтому пришлось прибегнуть к "Древу Жизни", очень удобный, хорошо работающий аналог. Немного неинтуитивный интерфейс у него и нет поддержки различных функций, но ничего страшного. 
В качестве родословной я решил обратиться к одной из самых известных, обсуждаемых королевских семей мира... Британской. Родословная состоит из 3 поколений королевской семьи от Елизаветы Второй до её внуков. Всего получилось 18 представителей дерева.

Файл типа GedCom был получен экспортом из программы Древо Жизни.

## Конвертация родословного дерева

Для конвертации GedCom файла в правила Prolog'а использовался язык Python. Он был выбран потому что в нём есть модуль для работы с этим типом файлов `gedcom`, что кратно упрощает работу с файлом. 
Парсинг файла производился следующий образом:
1) Брались все члены дерева.
```{python}
    gedcom_parser = Parser()
    gedcom_parser.parse_file(file)
    root_child_elements = gedcom_parser.get_root_child_elements()
```

2) Для каждого из них находились родители. А также пол самих элементов. Из этой информации формировались строки соответствующие правилам пролога и добавлялись в соответствующие множества (это сделано для того, чтобы правила были упорядочены).
```{python}
for parent in parents:
                if parent.get_gender() == 'M':
                    father_name, father_surname = parent.get_name()
                else:
                    mother_name, mother_surname = parent.get_name()

            if child_element.get_gender() == "M":
                sex.add(f'sex("{name} {surname}", m).\n')
            else:
                sex.add(f'sex("{name} {surname}", f).\n')         
```

3) Если родители существовали (если нет, то нас интересует только элемента, а его мы обработали в п.2), то мы записываем их имена в правило Prolog.
```{python}
if not (father_surname == "" and father_name == "" and mother_surname == "" 
and mother_name == ""):
                parents_str.add(f'parent("{father_name} {father_surname}",
                 "{name} {surname}").\n')
                parents_str.add(f'parent("{mother_name} {mother_surname}",
                 "{name} {surname}").\n')
```

4) В конце, оба множества правил (по очереди) отправлялись в новосозданный .pl файл, который хранит в себе все правила родства.
```{python}
fo = open("database.pl", mode="w", encoding="utf-8")
    for i in parents_str:
        fo.write(i)
    for j in sex:
        fo.write(j)
    fo.close()
```
## Предикат поиска родственника

В каждом предикате логически прописаны родственные связи между родственниками. Например, чтобы найти брата, нам нужно, чтобы оба человека имели общих родитилей, а левый элемент (по которому идёт запрос) был мужского пола. Однако есть особенность в том, что пролог найдёт для обоих элементов родителя дважды (маму и папу), а потому и выведет брата несколько раз. Тут я пришёл к 2 разным решениям. Для одних предикатов (например для брата, я находил родителя мужского рода и это ограничивало поиск и не давало повтора из-за мамы). А в некоторых других предикатах, например для жены, где нельзя отталкиваться от рода детей, ведь мы не знаем наверняка, есть ли у них мальчик или девочка, я использовал предикат `setof()` уже внутри предиката родства.
```prolog
brother(Hum1, Hum2):- 
    parent(Z, Hum1),
    parent(Z, Hum2),
    sex(Z, m),
    sex(Hum1, m), not(Hum1 = Hum2).

relative('Wife', X, Y):-
    setof(X, wife(X, Y), List),
    elem_from_list(List, X).
```

Разделение на 2 предиката позволяет создавать более сложную родственную связь из 2 или 3 более мелких, например Шурин - брат жены, реализован следующий образом:
```prolog
shurin(Hum1, Hum2):-
    brother(Hum1, Z),
    wife(Z, Hum2).

relative('Brother-in-law', X, Y):-
    setof(X, shurin(X, Y), List),
    elem_from_list(List, X).
```

P.S. Brother-in-law - Шурин.

```prolog
2 ?- relative('Cousin', "Beatrice Elizabeth Mary Mountbatten-Windsor", Y).
Y = "Henry Charles Albert David Windsor" ;
Y = "James Alexander Philip Theo Mountbatten-Windsor" ;
Y = "Louise Alice Elizabeth Mary Mountbatten-Windsor" ;
Y = "Peter Mark Andrew Phillips" ;
Y = "William Arthur Philip Louis Windsor" ;
Y = "Zara Anne Elizabeth Tindall (Phillips)".
```
## Определение степени родства

Для поиска степени родства используется поиск в глубину. Все предикаты родства сохраняются.
Сам поиск разбирался нами уже в 3 ЛР, приведу лишь его код сюда:
```prolog
translator([X, Y], [R]):-
	relative(R, X, Y).
translator([X, Y|T], [P, Q|R]):-
	relative(P, X, Y),
	translator([Y|T], [Q|R]), !.

move(Hum1, Hum2):-
	batya(Hum1, Hum2);
	mama(Hum1, Hum2);
	brother(Hum1, Hum2);
	sister(Hum1, Hum2);
	son(Hum1, Hum2);
	daughter(Hum1, Hum2);
	husband(Hum1, Hum2);
	wife(Hum1, Hum2),
	ded(Hum1, Hum2),
	babulya(Hum1, Hum2),
	vnuk(Hum1, Hum2),
	vnuchka(Hum1, Hum2),
	aunt(Hum1, Hum2),
	uncle(Hum1, Hum2),
    cousin(Hum1, Hum2).

% Depth-First Search
prolong([X|T], [Y, X|T]):-
	move(X, Y),
	not(member(Y, [X|T])).
path1([X|T], X, [X|T]).
path1(L, Y, R):-
	prolong(L, T),
	path1(T, Y, R).
dfs(X, Y, R2):-
	path1([X], Y, R1),
	translator(R1, R2).
```
После поиска мы получаем список из родственных связей, причём их стоит читать справа налево.
```prolog
4 ?- relative(W, "Beatrice Elizabeth Mary Mountbatten-Windsor", "Elizabeth Alexandra Mary Windsor"). 
W = 'Granddaughter' ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Sister', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Brother', 'Father'] ;
W = ['Wife', 'Father', 'Father'] ;
W = ['Wife', 'Father', 'Father'] ;
W = ['Wife', 'Father', 'Father'] ;
W = ['Wife', 'Father', 'Father'] ;
```
Вот, приведу пример, на последнем выведенном W. Елизавета вторая является женой отца отца Биатрис, а не наоборот. 
## Выводы

Данная курсовая работа научила меня составлению родословных, работе с типом GedCom (до того как я нашёл модуль для работы с ним, я успел разобраться в его структуре почти полностью). Я узнал, что члены королевской семьи не носят фамилию (а если нужно, то используют фамилию своего рода - Виндзоры (если речь про Английскую королевскую семью)). Так же я освежил в голове процесс работы 3 и 4 ЛР. В очередной раз для себя убедился в том, что Prolog может быть НЕВЕРОЯТНО удобен для сложных логических задач, но при этом он критически сложен и неповоротлив для простых задач по типу добавления уникальных элементов в список (если не использовать стандартные предикат `setof`). Также, увидел, по-настоящему полезное применение Prolog'а в жизненной ситуации. Сама курсовая работа была очень интересной.
