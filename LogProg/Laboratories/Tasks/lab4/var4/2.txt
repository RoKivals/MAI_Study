isNumber(X):-
    number(X).
low_operator('+').
low_operator('-').
hi_operator('/').
hi_operator('*').
l_scob('(').
r_scob(')').

reverseUnder([], X, Y):-
    append([], X, Y).
reverseUnder([Head1|Tail1], X, Y):-
    not(l_scob(Head1)),
    not(r_scob(Head1)),
    reverseUnder(Tail1, [Head1|X], Y).
reverseUnder([Head1|Tail1], X, Y):-
    l_scob(Head1),
    r_scob(Lelem),
    reverseUnder(Tail1, [Lelem|X], Y).
reverseUnder([Head1|Tail1], X, Y):-
    r_scob(Head1),
    l_scob(Lelem),
    reverseUnder(Tail1, [Lelem|X], Y).

reverse(List1, List2):-
    reverseUnder(List1, [], List2).

addLast([], X, [X]).
addLast([Head1|Tail1], X, [Head1|Tail2]):-
    addLast(Tail1, X, Tail2).

prefix([Head1|Tail1], Res, Stack, Y):-
    isNumber(Head1),
    addLast(Res, Head1, OutP),
    prefix(Tail1, OutP, Stack, Y).
prefix([Head1|Tail1], Res, [], Y):-
    (low_operator(Head1); hi_operator(Head1); l_scob(Head1)),
    prefix(Tail1, Res, [Head1], Y).
prefix([Head1|Tail1], Res, Stack, Y):-
    l_scob(Head1),
    prefix(Tail1, Res, [Head1|Stack], Y).
prefix([Head1|Tail1], Res, [Up|Stack], Y):-
    low_operator(Head1),
    (low_operator(Up); hi_operator(Up)),
    addLast(Res, Up, OutP),
    prefix([Head1|Tail1], OutP, Stack, Y).
prefix([Head1|Tail1], Res, [Up|Stack], Y):-
    low_operator(Head1),
    l_scob(Up),
    prefix(Tail1, Res, [Head1, Up|Stack], Y).
prefix([Head1|Tail1], Res, [Up|Stack], Y):-
    hi_operator(Head1),
    hi_operator(Up),
    addLast(Res, Up, OutP),
    prefix([Head1|Tail1], OutP, Stack, Y).
prefix([Head1|Tail1], Res, [Up|Stack], Y):-
    hi_operator(Head1),
    (low_operator(Up); l_scob(Up)),
    prefix(Tail1, Res, [Head1, Up|Stack], Y).
prefix([Head1|Tail1], Res, [Up|Stack], Y):-
    r_scob(Head1),
    not(l_scob(Up)),
    addLast(Res, Up, OutP),
    prefix([Head1|Tail1], OutP, Stack, Y).
prefix([Head1|Tail1], Res, [Up|Stack], Y):-
    r_scob(Head1),
    l_scob(Up),
    prefix(Tail1, Res, Stack, Y).
prefix([], Res, [], Y):-
    append(Res, [], Y).
prefix([], Res, [Up|Stack], Y):-
    addLast(Res, Up, OutP),
    prefix([], OutP, Stack, Y).

calculate(Get, Res):-
    reverse(Get, Piv1),
    prefix(Piv1, [], [], Piv),
    reverse(Piv, Res), !.
