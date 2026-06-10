%% Стандартные предикаты
mylength([], 0).
mylength([_|L], N) :- mylength(L, N1), N is N1 + 1.

mymember(X, [X|_]).
mymember(X, [_|L]) :- mymember(X, L).

myappend([], L, L).
myappend([X|L1], L2, [X|L]) :- myappend(L1, L2, L).

mydelete([X|L], X, L).
mydelete([K|L], X, [K|L1]) :- mydelete(L, X, L1).

mypermute([], []).
mypermute(L, [X|P]) :- mydelete(L, X, L1), mypermute(L1, P).

mysublist([], _).
mysublist([H|S], [H|L]) :- mysublist(S, L).
mysublist([H1|S], [H2|L]) :- mysublist([H1|S], L), H1 \= H2.

%% Предикат обработки списка
remove_last([_], []).
remove_last([H|L], [H|R]) :- remove_last(L, R).

remove_last_std(L, R) :- myappend(R, [_], L).

%% Предикат обработки числового списка
sorted([]).
sorted([_]).
sorted([X,Y|L]) :- X =< Y, sorted([Y|L]).

unsorted_std(L) :- myappend(_, [X,Y|_], L), X > Y.
sorted_std(L) :- not(unsorted_std(L)).

%% Примеры совместного использования предикатов
bad_sort(L, R) :- mypermute(L, R), sorted(R).
