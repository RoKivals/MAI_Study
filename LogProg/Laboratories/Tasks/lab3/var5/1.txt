append([],B,B).
append([H|T],B,[H|NT]):-
    append(T,B,NT).

makeMove(A,B):-
    append(Begin,[n,w|Tail],A),
    append(Begin,[w,n|Tail],B).
makeMove(A,B):-
    append(Begin,[b,n|Tail],A),
    append(Begin,[n,b|Tail],B).
makeMove(A,B):-
    append(Begin,[n,b,w|Tail],A),
    append(Begin,[w,b,n|Tail],B).
makeMove(A,B):-
    append(Begin,[b,w,n|Tail],A),
    append(Begin,[n,w,b|Tail],B).

inHistory(H,[H|_]).
inHistory(H,[_|Tail]):-
    inHistory(H,Tail).

search(B,[B|Tail]):-
    answer([B|Tail]).

search(B,[A|Tail]):-
    makeMove(A,C),
    not(inHistory(C,Tail)),
    search(B,[C,A|Tail]).

answer([]).
answer([H|Tail]):-
    answer(Tail),
    write(H),nl.

solve(A,B):-search(B,[A]).
