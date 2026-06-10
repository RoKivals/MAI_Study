% example 1:
% ?- phrase(sentence(P), [every,man,that,lives,loves,a,woman]).
% P = all(X):(man(X)&lives(X)=>exists(Y):(woman(Y)&loves(X, Y))) .

% example 2:
% ?- phrase(sentence(P), [john,loves,a,woman]).
% P = exists(X):(woman(X)&loves(john, X)) .

:- op(900, xfx, =>).
:- op(800, xfy, &).
:- op(300, xfx, :).

sentence(P) --> noun_phrase(X, P1, P), verb_phrase(X, P1).

noun_phrase(X, P1, P) -->
        determiner(X, P2, P1, P), noun(X, P3), rel_clause(X, P3, P2).
noun_phrase(X, P, P) --> name(X).

verb_phrase(X, P) --> trans_verb(X, Y, P1), noun_phrase(Y, P1, P).
verb_phrase(X, P) --> intrans_verb(X, P).

rel_clause(X, P1, P1&P2) --> [that], verb_phrase(X, P2).
rel_clause(_, P, P) --> [].

determiner(X, P1, P2, all(X):(P1=>P2)) --> [every].
determiner(X, P1, P2, exists(X):(P1&P2)) --> [a].

noun(X, man(X)) --> [man].
noun(X, woman(X)) --> [woman].

name(john) --> [john].

trans_verb(X, Y, loves(X,Y)) --> [loves].
intrans_verb(X, lives(X)) --> [lives].





