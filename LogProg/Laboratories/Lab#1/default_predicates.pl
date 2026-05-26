% Длина списка
arr_length([], 0).
arr_length([_|T], N):- length(T, N1), N is N1 + 1.

% Проверка на наличие элемента в списке
member(A, [A|_]).
member(A, [_|T]):- member(A, T).

% Конкатенация списков
append_arr([], New, New).
append_arr([A|X], New, [A|Z]):-append_arr(X, New, Z).

% Удаление всех элементов списка (возможно)
clear(List, []).

% Удаление одного элемента A слева из списка (Python erase)
remove([A|Y], A, Y).
remove([X|N], Del, [X|T]):-remove(N, Del, T).

% Выделение n элементов с конца.
% L - исх. массив, X - массив последних n элементов, Count - n.
% К массиву подставляется такое X, чтобы получился исходный массив
% Длина X должна быть равна Count
slice_n(L, X, Count):- append(_, X, L), arr_length(X, Count).

% Перестановки
permute([], []).
permute(L, [X|T]):-remove(X, L, R), permute(R, T).


sublist(S, L):-append(_, L1, L),append(S, _, L1).
