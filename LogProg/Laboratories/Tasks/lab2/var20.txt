% remove(element, list, list_without_element).
remove(Element, [Element|Tail], Tail).
remove(Element, [Head|Tail], [Head|Tail2]) :- remove(Element, Tail, Tail2).

% permute(list, permutation).
permute([], []).
permute(List, [Head|Tail]) :- remove(Head, List, Result), permute(Result, Tail).


% searching for contradictions in generated variant
contradiction(Variant) :- data(Variant, Fact, Arg, TF), logical_not(TF, FT), data(Variant, Fact, Arg, FT).

% logical inversion
logical_not(true, false).
logical_not(false, true).

% data(variant, fact, argument, T/F) - given facts
% mother <=> argument has mother
data([_, _, _, Roma, _], mother, [Roma], false).
data(_, mother, [shevchenko], true).
data(_, mother, [karpenko], true).
data(_, mother, [lysenko], true).
data(_, mother, [boychenko], true).


% meet <=> arg1 met arg2
data(_, meet, [shevchenko, karpenko], true).
data(_, meet, [karpenko, shevchenko], true).

data(_, meet, [lysenko, boychenko], true).
data(_, meet, [boychenko, lysenko], true).

data([Dina, _, Kolya, _, _], meet, [Dina, Kolya], false).
data([Dina, _, Kolya, _, _], meet, [Kolya, Dina], false).

% boy <=> arg is a boy
data(_, boy, [karpenko], true).
data([Dina, _, _, _, _], boy, [Dina], false).
data([_, Sonya, _, _, _], boy, [Sonya], false).
data([_, _, Kolya, _, _], boy, [Kolya], true).
data([_, _, _, Roma, _], boy, [Roma], true).
data([_, _, _, _, Misha], boy, [Misha], true).

% same_gender(Variant, Person1, Person2) -> true if X has the same gender as Y
same_gender(Variant, X, Y) :- data(Variant, boy, [X], TF), data(Variant, boy, [Y], FT), TF == FT, !.


% predicate to solve the problem
solve(Dina, Sonya, Kolya, Roma, Misha) :- 
    permute([Dina, Sonya, Kolya, Roma, Misha], [boychenko, karpenko, lysenko, savchenko, shevchenko]),
    Kolya \= karpenko,
    same_gender([Dina, Sonya, Kolya, Roma, Misha], shevchenko, boychenko),
    not(same_gender([Dina, Sonya, Kolya, Roma, Misha], lysenko, boychenko)),
    not(contradiction([Dina, Sonya, Kolya, Roma, Misha])), !.

% solve(Dina, Sonya, Kolya, Roma, Misha).