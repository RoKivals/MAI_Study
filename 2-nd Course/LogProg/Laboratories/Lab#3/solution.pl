% Вариант №8.

% Упорядоченные переходы. Такие что, true дают только искомые переходы по порядку.
less(r, w).
less(w, b).
less(r, b).

% Перестановки элементов в списке.
permute([], []).
permute([X|Rest], L) :-
    permute(Rest, L1),
    select(X, L, L1).

% Проверка на необходимость перестановки
check([A, B]) :-
    less(B, A).

% Можно ли добавить в список ещё один элемент
check_length(A, B) :-
    length(A, ALen), length(B, BLen),
    ALen < 2, BLen < 2.


move(A, B) :- move(A, B, [], []).
move([], [], A, B) :-
    length(A, 2), length(B, 2),
    permute(A, B),
    not(A = B),
    check(B).

% Если элементы в головах одинаковые, то просто игнорируем их,
% поскольку их не надо переставлять
move([X|T], [X|R], A, B) :-
    move(T, R, A, B).

% Разбиваем списки на 4 единичных элемента.
% По сути аналогия разбиения при сортировке слиянием. 
move([X|T], [Y|R], A, B) :-
    check_length(A, B),
    move(T, R, [X|A], [Y|B]).

prolong([X|T], [Y, X|T]) :-
    move(X, Y), not(member(Y, [X|T])).

bdth([[X|T]|_], X, [X|T]).
bdth([P|Q1], X, R) :-
	findall(Z, prolong(P, Z), T),
	append(Q1, T, Q0), !,
	bdth(Q0, X, R).
bdth([_|T], Y, R) :-
	bdth(T, Y, R).

dpth([X|T], X, [X|T]).
dpth(P, F, L) :-
    prolong(P, P1), dpth(P1, F, L).

depth_id([Finish|T], Finish, [Finish|T], 0).
depth_id(Path, Finish, R, N) :-
    N > 0,
    prolong(Path, NewPath),
    N1 is N - 1,
    depth_id(NewPath, Finish, R, N1).

search_dpth(A, B, P) :-
    dpth([A], B, L), reverse(L, P).

search_bdth(X, Y, P) :-
	bdth([[X]], Y, L), reverse(L, P).

search_id(Start, Finish, Path, DepthLimit) :-
    depth_id([Start], Finish, Res, DepthLimit),
    reverse(Res, Path).

search_bdth_time() :-
    get_time(Start),
    search_bdth([b, r, b, w, r, r, w, w, b], [r, r, r, w, w, w, b, b, b], Res),
    get_time(End),
    Time is End - Start,
    write('BFS: '), write(Res), nl,
    write('Time: '), write(Time), nl.

search_dpth_time() :-
    get_time(Start),
    search_dpth([b, r, b, w, r, r, w, w, b], [r, r, r, w, w, w, b, b, b], Res),
    get_time(End),
    Time is End - Start,
    write('DFS: '), write(Res), nl,
    write('Time: '), write(Time), nl.

search_id_time() :-
    get_time(Start),
    search_id([b, r, b, w, r, r, w, w, b], [r, r, r, w, w, w, b, b, b], Res2, 4),
    get_time(End),
    Time is End - Start,
    write('SID: '), write(Res2), nl,
    write('Time: '), write(Time), nl.

solve() :-
    search_bdth_time,
    search_dpth_time,
    search_id_time.
