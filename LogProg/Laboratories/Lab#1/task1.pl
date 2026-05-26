% Вариант №16. 
% Нахождение номера первого вхождения элемента со заданным значением 

% Без использования стандартных предикатов
index([Elem|_], Elem, 1).
index([_|T], Elem, Count):- index(T, Elem, Count1), Count is Count1 + 1,  !.

% С использованием стандартных предикатов
index_2(L, Elem, N):-
    append(T, [Elem|_], L), append(T, [Elem], T1), length(T1, N), !.

% Вариант №1. 
% Вычисление суммы элементов

% Обход списка
sum([], 0).
sum([A|T], Sum):- sum(T,  Sum1), Sum is Sum1 + A.

% Хвостовая рекурсия
sum_t(L, S):- sum_t(L, 0, S).
sum_t([], R, R):- !.
sum_t([H|T], Sum, R):- Sum1 is Sum + H, sum_t(T, Sum1, R).

