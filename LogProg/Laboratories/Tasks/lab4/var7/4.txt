prefix("вне").
prefix("из").
prefix("ис").
prefix("не").
prefix("ни").
prefix("к").
prefix("раз").
prefix("вы").
prefix("все").
prefix("при").
prefix("без").
prefix("бес").
prefix("па").
prefix("воз").
prefix("вз").
prefix("на").
prefix("об").
prefix("пре").
prefix("тре").
prefix("пере").
prefix("рас").
prefix("под").
prefix("от").
prefix("про").
prefix("до").
prefix("с").
prefix("за").
prefix("вс").
prefix("вос").

suffix("ева").
suffix("онн").
suffix("ен").
suffix("ива").
suffix("ыва").
suffix("ова").
suffix("ан").

ending("ете", genus("НЕТ"), form("мн")).
ending("ите", genus("НЕТ"), form("мн")).
ending("ал", genus("М"), form("ед")).
ending("ут", genus("НЕТ"), form("мн")).
ending("им", genus("НЕТ"), form("мн")).
ending("ят", genus("НЕТ"), form("мн")).
ending("ишь", genus("НЕТ"), form("ед")).
ending("ем", genus("НЕТ"), form("мн")).
ending("ет", genus("НЕТ"), form("ед")).
ending("ила", genus("Ж"), form("ед")).
ending("ел", genus("М"), form("ед")).
ending("ат", genus("НЕТ"), form("мн")).
ending("ило", genus("Ср"), form("ед")).
ending("ешь", genus("НЕТ"), form("ед")).
ending("ют", genus("НЕТ"), form("мн")).
ending("ю", genus("НЕТ"), form("ед")).
ending("ил", genus("М"), form("ед")).
ending("ала", genus("Ж"), form("ед")).
ending("у", genus("НЕТ"), form("ед")).
ending("ит", genus("НЕТ"), form("ед")).
ending("или", genus("НЕТ"), form("мн")).
ending("ела", genus("Ж"), form("ед")).


without_last_letter([_],[]):-!.
without_last_letter([X|T],[X|Y]):-without_last_letter(T,Y).

find_prefix([_|[]], pref(""), _):-!.
find_prefix([P|Word], pref(P), Word):-
    prefix(P).
find_prefix([H, H1|Word], pref(Pref), RemWord):-
    string_concat(H, H1, NewPref),
    append([NewPref], Word, NewWord),
    find_prefix(NewWord, pref(Pref), RemWord).

find_suffix([_|[]], suf(""), _):-!.
find_suffix(Word, suf(Suffix), Rem):-
    last(Word, Suffix),
    without_last_letter(Word, Rem),
    suffix(Suffix).
find_suffix(Word, suf(S), RemWord):-
    last(Word, Suffix),
    without_last_letter(Word, Rem),
    last(Rem, Suffix2),
    without_last_letter(Rem, Rem2),
    string_concat(Suffix2, Suffix, NewSuffix),
    append(Rem2, [NewSuffix], NewWord),
    find_suffix(NewWord, suf(S), RemWord).

find_ending([_|[]], end(""), gen("М"), form("ед"), _):-!.
find_ending(Word, end(End), Gen, Num, Rem):-
    last(Word, End),
    without_last_letter(Word, Rem),
    ending(End, Gen, Num).
find_ending(Word, end(E), Gen, Num, RemWord):-
    last(Word, End),
    without_last_letter(Word, Rem),
    last(Rem, End2),
    without_last_letter(Rem, Rem2),
    string_concat(End2, End, NewEnd),
    append(Rem2, [NewEnd], NewWord),
    find_ending(NewWord, end(E), Gen, Num, RemWord).

string_join([], Res, Res).
string_join([L | Tail], R, Res) :- string_concat(R, L, Res1), string_join(Tail, Res1, Res).

trunc("", Word, Word).
trunc("", _, _) :- fail.
trunc(_, _, _).

convert_string([], []).
convert_string("", []).
convert_string(X, X) :- is_list(X).
convert_string(X, Res) :- string_chars(X, Res).


get_morphem(Chars, morf(pref(Y), root(Root), end(Z), Gen, Num)):-
    convert_string(Chars, Word),
    find_prefix(Word, pref(Y), Word1), !,
    trunc(Y, Word1, Word),
    find_ending(Word1, end(Z), Gen, Num, Word2), !,
    trunc(Z, Word2, Word1),
    find_suffix(Word2, suf(X), Rem), !,
    trunc(X, Rem, Word2), !,
    string_join(Rem, "", Root).