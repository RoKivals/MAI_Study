exp(E) --> term(T),terms(T,E).

terms(E,E) --> [].
terms(E,F) --> op1(S),term(T), terms([S,E,T],F).

term(T) --> factor(F),factors(F,T).

factors(T,T) --> [].
factors(T,E) --> op2(S),factor(F), factors([S,T,F],E).

factor(T) --> pow_t(T).

pow_t(T) --> number(T).
pow_t([S,L,R]) --> number(L), op3(S), pow_t(R).
pow_t(T) --> exp(T).

number(N) --> [N], {number(N)}.

op1(S) --> {member(S, ['+', '-'])}, [S].
op2(S) --> {member(S, ['*', '/'])}, [S].
op3('^') --> ['^'].

parse_expr(Expr, Lst) :- exp(Lst, Expr, []).

calc([S,A1,A2], Res):-calc(A1,N1),calc(A2,N2),calc(S,N1,N2, Res),!.
calc(A1,A1):- not(A1 = [_|_]).
calc('*',N1,N2, Res):-  Res is N1 * N2.
calc('/',N1,N2, Res):-  Res is N1 / N2.
calc('+',N1,N2, Res):-  Res is N1 + N2.
calc('-',N1,N2, Res):-  Res is N1 - N2.
calc('^',N1,N2, Res):- pow(N1, N2,  Res).

solve(Expr, Ans) :-
	parse_expr(Expr, Lst),
	calc(Lst, Ans).