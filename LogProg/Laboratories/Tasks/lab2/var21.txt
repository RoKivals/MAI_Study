my_member(X, [X|_]).
my_member(X,[_|Y]) :- my_member(X,Y).

my_remove(X,[X|Y],Y).
my_remove(X,[Y|T],[Y|T1]) :- my_remove(X,T,T1).

solve(G,O,D) :-
my_remove(_,[biology,history,chemistry,math],G),
my_remove(_,[biology,history,chemistry,math],O),
my_remove(_,[biology,history,chemistry,math],D),
check(g,[G,O,D]), check(o,[G,O,D]), check(d,[G,O,D]).

check(N,S):-
my_remove(T1,[1,2,3,4],R), my_remove(T2,R,[L1,L2]),
say(N,T1,S), say(N,T2,S), not(say(N,L1,S)), not(say(N,L2,S)).

say(g,1,[G,O,D]) :- my_member(history, D), not(my_member(history, G)), not(my_member(history, O)).
say(g,2,[G,O,_]) :- my_member(P1, G), my_member(P1, O),
	my_member(P2, G), my_member(P2, O),
	my_member(P3, G), my_member(P3, O), P3\=P2, P2\=P1, P3\=P1.
say(g,3,[G,O,D]) :- my_member(biology, G), my_member(biology, O),my_member(biology, D).
say(g,4,[G,O,_]) :- my_member(biology,G), my_member(chemistry,G), my_member(biology,O), my_member(chemistry,O).
say(g,4,[G,_,D]) :- my_member(biology,G), my_member(chemistry,G), my_member(biology,D), my_member(chemistry,D).
say(g,4,[_,O,D]) :- my_member(biology,O), my_member(chemistry,O), my_member(biology,D), my_member(chemistry,D).

say(o,1,[G,O,D]) :- my_member(math, G), my_member(math, O),my_member(math, D).
say(o,2,[G,_,_]) :- my_member(history, G).
say(o,3,[_,O,D]) :- my_member(P1, D), my_member(P1, O),
	my_member(P2, D), my_member(P2, O),
	my_member(P3, D), not(my_member(P3, O)).
say(o,4,[G,_,D]) :- my_member(chemistry, G),my_member(chemistry, D).

say(d,1,[G,O,D]) :- my_member(P, G), my_member(P, O), my_member(P, D), not(say(g,2,[G,O,_])).
say(d,2,[G,O,D]) :- my_member(math, D), not(my_member(math,G)), not(my_member(math,O)).
say(d,3,[G,O,D]) :- D\=G, G\=O, D\=O.
say(d,4,[G,_,D]) :- not(say(o,4,[G,_,D])).

?- solve(G,O,D), write("G="), write(G), nl, write("O="),write(O), nl,
write("D="), write(D), nl.	