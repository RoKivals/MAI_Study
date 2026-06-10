for(L, U, L) :- L =< U.
for(L, U, X) :-
	L =< U,
	L1 is L + 1,
	for(L1, U, X).

check(M, C) :- M = 0; M > 0, M >= C.

move(s(ML, CL, left, MR, CR), s(NML, NCL, right, NMR, NCR)) :-
	for(0, ML, M),
	for(0, CL, C),
	M+C > 0, M+C =< 3,
	check(M, C),
	NML is ML-M, NCL is CL-C, NMR is MR+M, NCR is CR+C,
	check(NML, NCL), check(NMR, NCR).

move(s(ML, CL, right, MR, CR), s(NML, NCL, left, NMR, NCR)) :-
	for(0, MR, M),
	for(0, CR, C),
	M+C > 0, M+C =< 3,
	check(M, C),
	NML is ML+M, NCL is CL+C, NMR is MR-M, NCR is CR-C,
	check(NML, NCL), check(NMR, NCR).

write_path([]).
write_path([s(ML, CL, left, MR, CR)|T]) :- write_path(T), 
	write("M: "), write(ML), write(", C: "), write(CL),
	write(" |Boat------| "),
	write("M: "), write(MR), write(", C: "), write(CR), nl.
write_path([s(ML, CL, right, MR, CR)|T]) :- write_path(T), 
	write("M: "), write(ML), write(", C: "), write(CL),
	write(" |------Boat| "),
	write("M: "), write(MR), write(", C: "), write(CR), nl.

prolong([X|T],[Y,X|T]) :- move(X,Y), not(member(Y, [X|T])).

dfs([X|T], X, [X|T]).
dfs(P1, X, Res) :-
	prolong(P1, P2),
	dfs(P2, X, Res).

bfs([[X|T]|_], X, [X|T]).
bfs([P|Q1],X,R) :-
	findall(Z, prolong(P,Z), T),
	append(Q1, T, Q2),
	bfs(Q2, X, R).

num(1).
num(A) :- num(B), A is B + 1.

ids([X|T], X, [X|T], 0).
ids(P1, X, R, N) :- N > 0,
	prolong(P1, P2), N1 is N - 1,
	ids(P2, X, R, N1).

path(bfs, X, Y, Path, Len) :- get_time(Start), bfs([[X]], Y, Path), get_time(End),
	length(Path, Len),
	write("Time: "), Time is End-Start, write(Time), nl, write_path(Path).

path(dfs, X, Y, Path, Len) :- get_time(Start), dfs([X], Y, Path), get_time(End),
	length(Path, Len),
	write("Time: "), Time is End-Start, write(Time), nl, write_path(Path).

path(ids, X, Y, Path, Len) :- get_time(Start), num(Lvl), ids([X], Y, Path, Lvl), get_time(End),
	length(Path, Len),
	write("Time: "), Time is End-Start, write(Time), nl, write_path(Path).
