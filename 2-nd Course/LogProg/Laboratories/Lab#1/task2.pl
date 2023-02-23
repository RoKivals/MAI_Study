% Task 2: Relational Data

% The line below imports the data
:- encoding(utf8).
:- ['four.pl'].

% Вариант 1. Представление 4.

% Задание 1: Получить таблицу групп и средний балл по каждой из групп

    % Считает сумму. Используется для вычисления среднего.
    sum([], 0).
    sum([X|Y], S) :- sum(Y, S1), S is S1 + X.

    % Считает среднее. Используется для подсчета среднего балла.
    avg(L, R) :- 
    length(L, Len), 
    sum(L, Sum), R is Sum / Len.

    % Выдает оценки все студента. 
    grades(Student, N) :- 
    subject(_, X), 
    member(grade(Student, N), X).

    % Среднее по всем предметам, оценкам определенного студента.
    stud_to_overall_avg(Student, Res) :- 
    findall(N, grades(Student, N), List), avg(List, Res).

    % Средний балл каждого студента в определенной группе.
    group_to_every_studs_avg(Group, Res) :- 
    group(Group, StudList), 
    member(Stud, StudList), 
    stud_to_overall_avg(Stud, Res).

    % В каждой группе считаем сумму средних баллов всех ее студентов. Считаем среднее арифм. этой суммы. Выводим результат.
    groups_avg() :- 
        group(Group, _), 
        findall(Res, group_to_every_studs_avg(Group, Res), ListValue),
        avg(ListValue, Ans),
    write('Группа №'), write(Group), write(', средний бал: '), write(Ans), write('\n'), fail.

% Задание 2: Для каждого предмета получить список студентов, не сдавших экзамен (grade=2)

    % Находит имена студентов, имеющих двойки по опред. предмету
    subj_to_failed_studs(Subj, Name) :-
        subject(Subj, Students),
        member(grade(Name, 2), Students).

    % По каждому предмету выводит список несдавших его студентов, если такие имеются
    subj_failed() :-
        subject(Subj, _),
        findall(Name, subj_to_failed_studs(Subj, Name), List),
        length(List, ListLen),
        write('Предмет "'), write(Subj), 
            ((ListLen \= 0, write('" не сдали: '), write(List), write('\n'));
            (ListLen == 0, write('" сдали все!\n'))),
            fail.

% Задание 3 Найти количество не сдавших студентов в каждой из групп

    % Студенты с неудами в опред. группе
    group_subj_to_failed_studs(Group, Stud) :-
        subject(_, GradesList),
        group(Group, StudsList),
        member(Stud, StudsList),
        member(grade(Stud, 2), GradesList).

    % Кол-во студентов с недуами в опред. группе
    group_to_cnt_failed(Group, Cnt) :-
        findall(Name, group_subj_to_failed_studs(Group, Name), List),
        length(List, Cnt).

    % В каждой группе считает число студентов с неудами
    cnt_failed() :-
        group(Group, _),
        group_to_cnt_failed(Group, Cnt),
        write('Группа №'), write(Group), write(', число несдавших: '), write(Cnt), write('\n'), fail.