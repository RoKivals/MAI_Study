:- include("two.pl").

average(Subj, X) :-
	bagof(Mark, Group^Stud^grade(Group, Stud, Subj, Mark), Res),
	sum_list(Res, Sum), length(Res, N),
	X is Sum / N.

not_passed_group(Group, X) :-
	setof(Stud, Subj^grade(Group, Stud, Subj, 2), Res),
	length(Res, X).

not_passed_subj(Subj, X) :-
	bagof(Stud, Group^grade(Group, Stud, Subj, 2), Res),
	length(Res, X).
