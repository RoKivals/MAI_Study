% Вариант №15.
:-encoding(utf8).


% Предикат проверки на соседство.
neighbours(A, B, C):- 
    C=[A, B, _, _];
    C=[B, A, _, _].

%игроки на 2-м и 3-м месте списка (учитываем, что игроки и neighboors пересекаются)
play(A, B, C):- 
    C=[_, A, B, _]. 

rides(A, C):- neighbours(A, _, C).

%если встречались 1 раз, то они не neighboors и не играют вместе 
meetOnce(A, B, C):- 
    not(neighbours(A, B, C)),
    not(play(A, B, C)).


desicion(C) :- % список вида (Фамилия, Профессия, Возраст)
length(C, 4),  %  всего 4 человека
member([_, _, 4], C), %  кто-то самый старший (условный возраст 4)   
member([_, _, 3], C), %  а есть также и младше
member([_, _, 2], C), %  и еще младше
member([_, _, 1], C), %  возможно кто-то самый младший
member([_, baker, _], C),    
member([_, doctor, _], C),
member([_, policeman, _], C),
member([_, engineer, _], C),
neighbours([korneev, _, _], [dokshin, _, _], C), % Корнеев и Докшин - соседи 
rides([korneev, _, _], C), 
rides([dokshin, _, _], C),
member([dokshin, _, DockshinAGE], C),
member([mareev, _, MareevAGE], C),
DockshinAGE > MareevAGE, % Докшин старше Мареева
play([korneev, _, _], [skobelev, _, _], C),
not(rides([_, baker, _], C)), % baker ходит пешком (не rides)
not(neighbours([_, policeman, _], [_, doctor, _], C)), % Коп и доктор не соседи
meetOnce([_,engineer,_], [_,policeman,_], C), % коп и инж встречались 1 раз
member([_, policeman, POLAGE], C),
member([_, doctor, DOCAGE], C), POLAGE>DOCAGE, % коп старше дока 
member([_, engineer, ENGAGE], C), POLAGE>ENGAGE. % коп старше инжа

%Просто скучно, вот и сделал)
retranslator(Prof, RuProf):-
    RuProf="Инженер",
    Prof=engineer;
    RuProf="Полицейский",
    Prof=policeman;
    RuProf="Пекарь",
    Prof=baker;
    RuProf="Доктор",
    Prof=doctor.
    
print_person([SNAME, PROF, _]):- 
    retranslator(PROF, T),
    write(SNAME), write(": "), writeln(T).

print_ans([]).
print_ans([A|T]):-
    print_person(A), print_ans(T).


answer():-
    desicion(R),
    print_ans(R).