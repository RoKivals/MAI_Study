true(river_volga).
true(pupil_vasya).
true(true).


calculate(List, Res):-
	solver(List, [Res]).

% Обработка случая, когда true или false обёрнуты в скобки (ну мало ли).
% + это одновременно с тем удаляет скобки после предварительного подсчёта выражения.
solver(['(', true, ')'|T], Res):-
	solver([true|T], Res).

solver(['(',false,')'|T],[false|T]):-
	solver([false|T],Res).

% Обработка ЛОГИЧЕСКОГО И.
solver([A, '&', B | T], Res):-
	true(A),
	true(B),
	isvar(A),
	isvar(B),
	solver([true|T], Res),
    !.
solver([A, '&', B|T], Res):-
    isvar(A),
	isvar(B),
	solver([false|T],Res).

% Обработка импликации.
solver([A, '=>', B|T], Res):-
	(not(true(A)); true(B)),
	isvar(A),
	isvar(B),
	solver([true|T], Res),
	!.
solver([A, '=>' ,B | T], Res):-
	isvar(A),
	isvar(B),
	solver([false|T], Res).

% Обработка логического отрицания (если я правильно понял)...
solver(['~', B|T], Res):-
	true(B),
	isvar(B),   
	solver([false|T], Res),
	!.
solver(['~', B|T], Res):-
	isvar(B),
	solver([true|T], Res).

% Обработка логического ИЛИ.
solver([A, 'V', B|T], Res):-
	(true(A); true(B)),
	isvar(A),
	isvar(B),
	solver([true|T], Res),
	!.
solver([A, 'V', B|T], Res):-
	isvar(A),
	isvar(B),
	solver([false|T], Res).


solver([A | [B|T]], Res):-
	solver([B|T], Y),
	[B|T] \= Y,
	solver([A|Y], Res),!.

solver(X, X).

% Проверяет не является ли переменная X одним из знаков.
isvar(X):-
	X \= '(',
	X \= ')',
	X \= '~',
	X \= 'V',
	X \= '&',
	X \= '=>'.
