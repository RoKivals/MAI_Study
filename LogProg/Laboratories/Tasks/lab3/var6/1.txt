my_append([],B,B). 
my_append([H|Tail],B,[H|NewTail]):-my_append(Tail,B,NewTail). 
 
my_member(H,[H|_]). 
my_member(H,[_|Tail]):-my_member(H,Tail). 
 
step(A,B):- 
my_append(H,["_","w"|Tail],A), 
my_append(H,["w","_"|Tail],B). 
step(A,B):- 
my_append(H,["b","_"|Tail],A), 
my_append(H,["_","b"|Tail],B). 
step(A,B):- 
my_append(H,["_","b","w"|Tail],A), 
my_append(H,["w","b","_"|Tail],B). 
step(A,B):- 
my_append(H,["b","w","_"|Tail],A), 
my_append(H,["_","w","b"|Tail],B). 
 
answer([]). 
answer([H|Tail]):-answer(Tail),write(H),nl. 
 
depth(To,Tail):-!,answer(Tail). 
depth(To,[From|Tail]):-step(From,Tmp),not(member(Tmp,Tail)),depth(To,[Tmp,From|Tail]). 
  
next(X, Prev, Y) :- 
step(X,Y), 
not(my_member(Y,Prev)). 
 
breadth([From|_],To,From) :- From = _. 
 
breadth([[Last|HasBeen]|Other],To,Way):- 
findall([Z,Last|HasBeen], 
next(Last, HasBeen, Z), List), 
my_append(List,Other,NewWays), 
breadth(NewWays,To,Way). 
 
extend([X|T],[Y,X|T]) :- 
step(X,Y), 
not(my_member(Y,[X|T])). 
 
iter(Tail,To,Tail,0). 
iter(TempWay,To,Way,N):- 
N > 0, 
extend(TempWay,NewWay), 
N1 is N-1, 
iter(NewWay,To,Way,N1). 
 
for(X, X, _). 
for(I, X, Y):- 
X < Y, 
X1 is X + 1, 
for(I, X1 ,Y). 
 
solve(From, To):- 
write('Iterative'), nl, 
get_time(S1), 
for(Lvl, 1, 20), 
iter([From], To, Way, Lvl), 
answer(Way), 
get_time(F1), 
T is F1 - S1, 
write('TIME IS '), write(T), nl, nl, 
 
write('DEPTH'), nl, 
get_time(S2), 
depth(To, [From]), 
get_time(F2), 
T1 is F2 - S2, 
write('TIME IS '), write(T1), nl, nl, 
 
write('BREADTH'), nl, 
get_time(S3), 
breadth([[From]],To,Way), 
answer(Way), 
get_time(F3), 
T2 is F3 - S3, 
write('TIME IS '), write(T2), nl, nl, 
!.
