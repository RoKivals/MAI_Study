%[b, b, b, b, *, w, w, w]
move(In, Out) :-
    (   In=[*, Elem, w], % w может идти влево, но через одну ячейку
        Out=[w, Elem, *]
    ;   append(Sub, [b, *|End], In), % Если есть пара b,*
        append(Sub, [*, b|End], Out)
    ;   append(Sub, [*, w|End], In), % Если есть пара *,w
        append(Sub, [w, *|End], Out)
    ;   In=[b, Elem, *], % b может идти только вправо, но через одну ячейку
        Out=[*, Elem, b]
    ).

prolong(In, Out) :-
    (   A=(*) % Так немного быстрее
    ;   B=(*)
    ;   C=(*)
    ),
    append(Head, [A, B, C|Tail], In),
    move([A, B, C], [D, E, F]),
    append(Head, [D, E, F|Tail], Out).
prolong([A|T], [B, A|T]) :-
    prolong(A, B).
prolong([], []).

dpth([X|T], X, [X|T]).
dpth([P|T], F, L) :-
    prolong(P, P1),
    dpth([P1, P|T], F, L).

width([[X|T]|_], X, [X|T]). % конец рекурсии, необходимый путь найден
width([P|QI], Search_Elem, Res) :-
    findall(Z, prolong(P, Z), From_P),
    append(QI, From_P, Q0), % вместо Р добавляем в конец списка все пути из него
    width(Q0, Search_Elem, Res).

int(1).
int(X) :-
    int(Y),
    X is Y+1.

iter([Finish|T], Finish, [Finish|T], 0).
iter(Path, Finish, R, N) :-
    N@>0,
    prolong(Path, NewPath),
    N1 is N-1,
    iter(NewPath, Finish, R, N1).

search(Sort, A, B) :-
    (   Sort==deep,
        get_time(STime),
        dpth([A], B, L),
        get_time(ETime)
    ;   Sort==width,
        get_time(STime),
        width([[A]], B, L),
        get_time(ETime)
    ;   Sort==iter,
        get_time(STime),
        int(Depth),
        iter([A], B, L, Depth),
        get_time(ETime)
    ),
    print(L),
    nl,
    length(L, Len),
    write('Solution length: '),
    writeln(Len),
    Time is ETime-STime,
    write('TIME IS '),
    writeln(Time).

print([]).
print([A|T]) :-
    print(T),
    writeln(A).