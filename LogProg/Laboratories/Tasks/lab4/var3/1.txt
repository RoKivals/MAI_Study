calculate(L, V):-
    a_expr(L, V),!.

a_expr(E, V):-
    append(T, ['+'| E1], E),
    a_expr(T, V1),
    a_term(E1, V2),
    V is V1 + V2.

a_expr(E, V):-
    append(T, ['-'| E1], E),
    a_expr(T, V1),
    a_term(E1, V2),
    V is V1 - V2.

a_expr(E, V):-
    a_term(E, V).

a_term(T, V):-
    append(N, ['*' | T1], T),
    a_term(N, V1),
    a_power(T1, V2),
    V is V1 * V2.

a_term(T, V):-
    append(N, ['/' | T1], T),
    a_term(N, V1),
    a_power(T1, V2),
    V is V1 / V2.

a_term(T, V):-
    a_power(T, V).

a_power(T, V):-
    append(N, ['^', T2], T),
    a_power(N, T1),
    V is T1 ** T2.

a_power([V], V):-
    number(V).
