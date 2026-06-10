# Отчет по лабораторной работе №4
## по курсу "Логическое программирование"

## Обработка естественного языка

### студент: Бирюков В.В.

## Результат проверки

| Преподаватель     | Дата         |  Оценка       |
|-------------------|--------------|---------------|
| Сошников Д.В. |              |               |
| Левинская М.А.|              |       5       |

> *Комментарии проверяющих (обратите внимание, что более подробные комментарии возможны непосредственно в репозитории по тексту программы)*

## Введение

Существует два основных подхода к анализу текста на естественном языке. Это статистический и лингвистический анализ. Статистический анализ выясняет наиболее часто встречающиеся слова и другие особенности лексики текста. Лингвистический анализ в свою очередь разделяется на предварительный анализ, морфологический анализ, синтаксический анализ и семантический анализ.

Обработка текста на исскуственном языке состоит из лексического анализа и синтаксического анализа.

Синтаксический анализ текста на любом языке основывается на понятии грамматики. Язык Пролог очень удобен для описания таких грамматик.

## Задание

Генеалогическое дерево задано фактами вида
```prolog
parent(alexei,tolia).
parent(alexei,volodia).
parent(tolia,tima).
...
```

Написать программу на Прологе, запросы к которой будут выглядеть следующим образом:

Запросы:
```prolog
?- answer([volodia, brat, toli, ‘?’], X).
?- answer([kto, tolin, brat, ‘?’], X).
?- answer([chei, brat, volodia, ‘?’], X).
```
Результаты: `X=yes, X=volodia, X=tolia`. 

## Принцип решения

Необходимые вопросительные предложения можно описать при помощи следующей констекстно-свободной грамматики:

```
Фраза --> Подлежащее Сказуемое Определение |
          НеизвПодлежащее Определение Сказуемое |
          НеизвОпределение Сказуемое Подлежащее
Подлежащее --> *Имя в именительном падеже*
Сказуемое --> *Отношение родства*
Определение --> *Имя в родительном или творительном падеже*
НеизвПодлежащее --> Кто
НеизвОпределение --> Чей
```

Для выполнения запросов необходимы предикаты, выполняющие работу с базой фактов.
```prolog
relation(brother, A, B) :- parent(C, A), parent(C, B), A \= B.
relation(father, A, B) :- parent(A, B).
relation(son, A, B) :- parent(B, A).
```

Для синтаксичекого анализа предложений нужны словари: словарь родственных отношений и словарь имен, включающий также падежные формы.
```prolog
:- op(200, xfy, ':').

dict(people, Dict) :- Dict = [
	'volodia' : nominative : 'volodia',
	'volodia' : genitive   : 'volodi',
	'volodia' : possessive : 'volodin',
	'tolia'   : nominative : 'tolia',
	'tolia'   : genitive   : 'toli',
	'tolia'   : possessive : 'tolin',
	'tima'    : nominative : 'tima',
	'tima'    : genitive   : 'timy',
	'tima'    : possessive : 'timin',
	'alexei'  : nominative : 'alexei',
	'alexei'  : genitive   : 'alexeya',
	'alexei'  : possessive : 'alexeya'
].

dict(relations, Dict) :- Dict = [
	brother : 'brat',
	father  : 'otetz',
	son     : 'syn'
].

find(Dict, Word) :- dict(Dict, List), member(Word, List).
```

Предикаты синтаксического анализа:
```prolog
answer(L, Res) :- append(L1,L4,L), append(L2,L5,L4), append(L3,['?'],L5),
	an_subject(L1, Name1), an_relation(L2, Rel), an_object(L3, Name2), !, yesno(relation(Rel, Name1, Name2), Res).

answer(L, Res) :- append(L1,L4,L), append(L2,L5,L4), append(L3,['?'],L5),
	an_q_subject(L1), an_object(L2, Name), an_relation(L3, Rel), !, relation(Rel, Res, Name).

answer(L, Res) :- append(L1,L4,L), append(L2,L5,L4), append(L3,['?'],L5),
	an_q_object(L1), an_relation(L2, Rel), an_subject(L3, Name), !, relation(Rel, Name, Res).

an_relation([Word], Rel) :- find(relations, Rel:Word).

an_q_subject(['kto']).
an_subject([Word], Name) :- find(people, Name:nominative:Word).

an_q_object(['chei']).
an_object([Word], Name) :- find(people, Name:genitive:Word).
an_object([Word], Name) :- find(people, Name:possessive:Word).
```

Нетерминалы `an_subject`, `an_object` и `an_relation`, возвращают имена в именительном падеже и использующуюся в предикате `relation` форму родственного отношения, из которых и формируется запрос к фактам генеологического дерева. Вспомогательный предикат `yesno/2` сопоставляет с переменной атомы `yes` или `no` в зависимости от успеха выполнения переданного терма.

Множество запросов к программе можно легко увеличить, расширив словари и добавив соответствующие варианты предиката `relation/3` и факты генеалогического дерева. В частности, мною добавлены родственные отношения "отец" и "сын".

## Результаты
	
Примеры запросов:
```prolog
?- answer([volodia, brat, toli, '?'], X).
X = yes.
?- answer([kto, tolin, brat, '?'], X).
X = volodia ;
false.
?- answer([chei, brat, volodia, '?'], X).
X = tolia ;
false.
?- answer([kto, volodin, otetz, '?'], X).
X = alexei.
?- answer([tima, syn, alexeya, '?'], X).
X = no.
?- answer([chei, syn, tima, '?'], X).
X = tolia.
?- answer([alexei, otetz, toli , '?'], X).
X = yes.
?- answer([chei, otetz, alexei , '?'], X).
X = tolia ;
X = volodia.
```

Текст программы:
```prolog
parent(alexei,tolia).
parent(alexei,volodia).
parent(tolia,tima).

relation(brother, A, B) :- parent(C, A), parent(C, B), A \= B.
relation(father, A, B) :- parent(A, B).
relation(son, A, B) :- parent(B, A).

:- op(200, xfy, ':').

dict(people, Dict) :- Dict = [
	'volodia' : nominative : 'volodia',
	'volodia' : genitive   : 'volodi',
	'volodia' : possessive : 'volodin',
	'tolia'   : nominative : 'tolia',
	'tolia'   : genitive   : 'toli',
	'tolia'   : possessive : 'tolin',
	'tima'    : nominative : 'tima',
	'tima'    : genitive   : 'timy',
	'tima'    : possessive : 'timin',
	'alexei'  : nominative : 'alexei',
	'alexei'  : genitive   : 'alexeya',
	'alexei'  : possessive : 'alexeya'
].

dict(relations, Dict) :- Dict = [
	brother : 'brat',
	father  : 'otetz',
	son     : 'syn'
].

find(Dict, Word) :- dict(Dict, List), member(Word, List).

yesno(Term, Res) :- Term, !, Res = 'yes'.
yesno(_, Res) :- Res = 'no'.

answer(L, Res) :- append(L1,L4,L), append(L2,L5,L4), append(L3,['?'],L5),
	an_subject(L1, Name1), an_relation(L2, Rel), an_object(L3, Name2), !, yesno(relation(Rel, Name1, Name2), Res).

answer(L, Res) :- append(L1,L4,L), append(L2,L5,L4), append(L3,['?'],L5),
	an_q_subject(L1), an_object(L2, Name), an_relation(L3, Rel), !, relation(Rel, Res, Name).

answer(L, Res) :- append(L1,L4,L), append(L2,L5,L4), append(L3,['?'],L5),
	an_q_object(L1), an_relation(L2, Rel), an_subject(L3, Name), !, relation(Rel, Name, Res).

an_relation([Word], Rel) :- find(relations, Rel:Word).

an_q_subject(['kto']).
an_subject([Word], Name) :- find(people, Name:nominative:Word).

an_q_object(['chei']).
an_object([Word], Name) :- find(people, Name:genitive:Word).
an_object([Word], Name) :- find(people, Name:possessive:Word).
```

## Выводы

В ходе лабораторной работы я познакомился с понятиями констекстно-свободных грамматик и синтаксическим анализом и построил синтаксический анализатор на языке Пролог для разбора ограниченных предложений русского языка.

Язык Пролог оказался очень удобен для задач грамматического разбора, так как в силу декларативной природы он позволяет просто описывать правила грамматики.
