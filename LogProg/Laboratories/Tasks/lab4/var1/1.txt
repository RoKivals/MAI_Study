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

%% answer([volodia, brat , toli, '?'],X).
%% answer([kto, tolin, brat, '?'],X).
%% answer([chei, brat, volodia, '?'],X).