:-['database.pl'].
:- dynamic sex/2, parent/2.

elem_from_list([], "").
elem_from_list([H|_], H).


brother(Hum1, Hum2):- 
    parent(Z, Hum1),
    parent(Z, Hum2),
    sex(Z, m),
    sex(Hum1, m), not(Hum1 = Hum2).

sister(Hum1, Hum2):-
    parent(Z, Hum1),
    parent(Z, Hum2),
    sex(Z, m),
    sex(Hum1, f), not(Hum1 = Hum2).

batya(Hum1, Hum2):-
    parent(Hum1, Hum2),
    sex(Hum1, m).

mama(Hum1, Hum2):-
    parent(Hum1, Hum2),
    sex(Hum1, f).

wife(Hum1, Hum2):-
    mama(Hum1, Z),
    batya(Hum2, Z).

husband(Hum1, Hum2):-
    batya(Hum1, Z),
    mama(Hum2, Z).

son(Hum1, Hum2):-
    parent(Hum2, Hum1),
    sex(Hum2, m),
    sex(Hum1, m).

daughter(Hum1, Hum2):-
    parent(Hum2, Hum1),
    sex(Hum2, m),
    sex(Hum1, f).

uncle(Hum1, Hum2):-
    brother(Hum1, Z),
    parent(Z, Hum2).

aunt(Hum1, Hum2):-
    sister(Hum1, Z),
    parent(Z, Hum2).

ded(Hum1, Hum2):-
    parent(Hum1, Z),
    parent(Z, Hum2),
    sex(Hum1, m).

babulya(Hum1, Hum2):-
    parent(Hum1, Z),
    parent(Z, Hum2),
    sex(Hum1, f).

vnuk(Hum1, Hum2):-
    son(Hum1, Z),
    parent(Hum2, Z).

vnuchka(Hum1, Hum2):-
    daughter(Hum1, Z),
    parent(Hum2, Z).

shurin(Hum1, Hum2):-
    brother(Hum1, Z),
    wife(Z, Hum2).

%Кузина - двоюродный брат или сестра
cousin(Hum1, Hum2):-
    parent(Z, Hum1),
    (brother(Z, Y); sister(Z, Y)),
    parent(Y, Hum2).

%----------------------------------------------------------------------%
relative('Brother', X, Y):-
    brother(X, Y).

relative('Sister', X, Y):-
    sister(X, Y).

relative('Father', X, Y):-
    setof(X, batya(X, Y), List),
    elem_from_list(List, X).

relative('Mother', X, Y):-
    setof(X, mama(X, Y), List),
    elem_from_list(List, X).

relative('Wife', X, Y):-
    setof(X, wife(X, Y), List),
    elem_from_list(List, X).
    
relative('Husband', X, Y):-
    setof(X, husband(X, Y), List),
    elem_from_list(List, X).

relative('Son', X, Y):-
    son(X, Y).

relative('Daughter', X, Y):-
    daughter(X, Y).

relative('Grandpa', X, Y):-
    ded(X, Y).

relative('Grandma', X, Y):-
    babulya(X, Y).

relative('Grandson', X, Y):-
    vnuk(X, Y).

relative('Granddaughter', X, Y):-
    vnuchka(X, Y).

relative('Brother-in-law', X, Y):-
    setof(X, shurin(X, Y), List),
    elem_from_list(List, X).

relative('Cousin', X, Y):-
    setof(X, cousin(X, Y), List),
    elem_from_list(List, X).

relative(Rel, Fir, Sec):-
    dfs(Fir, Sec, Rel).

% Для перевода списка имён в список отношений.
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