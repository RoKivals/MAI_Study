:- encoding(utf8).

% split a list into 2 or 3 parts
split(List, Part1, Part2) :- append(Part1, Part2, List), not(length(Part1, 0)), not(length(Part2, 0)).
split(List, Part1, Part2, Part3) :- append(Part1, TMP, List), append(Part2, Part3, TMP),
                                    not(length(Part1, 0)), not(length(Part2, 0)), not(length(Part3, 0)).


% dictionaries
nouns(['игрушки', 'кубики', 'мячи', 'стихи', 'прозы', 'пьесы', 'Саша', 'Ира']).
verbs(['любит']).
separators([['и'], [',', 'но'], [',', 'а'], [',']]).

% checking nouns, verbs and separators: generating a dictionary and searching our word there
check_noun(Noun) :- nouns(List), member(Noun, List).
check_verb(Verb) :- verbs(List), member(Verb, List).
check_separator(Sep) :- separators(List), member(Sep, List).

% generating a term's name for our verb
verb_to_term('любит', 'likes').
verb_to_term(['не' | 'любит'], 'not_likes').



% we contain deep structures in a set
decompose(Phrase, List) :- setof(Deep_st, decompose_phrase(Phrase, Deep_st), List).

decompose_phrase(Phrase, Result) :- check_phrase(Phrase, deep_st(Verb, Subject, Object)), 
                                    verb_to_term(Verb, Verb_Term), Result=..[Verb_Term, Subject, Object].

% splitting a subject and actions
check_phrase([Subject | Actions], deep_st(Verb, Subject, Object)) :- check_noun(Subject), 
                                                                    check_actions(Actions, Verb, Object).

% actions -> verb_group
check_actions(Actions, Verb, Object) :- check_verb_group(Actions, Verb, Object).

% actions -> verb_group + actions
check_actions(Actions, Verb, Object) :- split(Actions, Verb_Group, TMP), check_verb_group(Verb_Group, Verb, Object), 
                                        check_actions(TMP, _, _).

check_actions(Actions, Verb, Object) :- split(Actions, TMP, Actions1),
                                        check_verb_group(TMP, _, _),
                                        check_actions(Actions1, Verb, Object).

% verb_group -> 'НЕ' + verb_group
check_verb_group(['не' | Verb_Group], ['не' | Verb], Object) :- !, check_verb_group(Verb_Group, Verb, Object).

% verb_group -> verb + obj_group
check_verb_group([Verb | Object_Group], Verb, Object) :- check_verb(Verb),
                                                    check_object_group(Object_Group, Object).

% verb_group -> separator + verb_group
check_verb_group([Sep | Verb_Group], Verb, Object) :- check_separator([Sep]), 
                                                        check_verb_group(Verb_Group, Verb, Object).

check_verb_group([Sep1, Sep2 | Verb_Group], Verb, Object) :- check_separator([Sep1, Sep2]), 
                                                            check_verb_group(Verb_Group, Verb, Object).

% obj_group -> object
check_object_group([Object], Object) :- check_noun(Object).

% obj_group -> obj_group + separator + obj_group
check_object_group(Object_Group, Object) :- split(Object_Group, Object_Group1, Sep, TMP), length(Sep, 1), 
                                            check_separator(Sep), check_object_group(Object_Group1, Object),
                                            check_object_group(TMP, _).

check_object_group(Object_Group, Object) :- split(Object_Group, TMP, Sep, Object_Group2), length(Sep, 1),
                                        check_separator(Sep), check_object_group(TMP, _),
                                        check_object_group(Object_Group2, Object).



% TESTS:
% decompose(['Саша', 'любит', 'кубики', ',', 'но', 'не', 'любит', 'мячи', 'и', 'стихи'], X). 
% decompose(['Ира', 'не', 'любит', 'стихи', 'и', 'прозы', ',', 'а', 'любит', 'пьесы' ], X).
% decompose(['Саша', 'любит', 'кубики', 'и', 'игрушки', ',', 'но', 'не', 'любит', 'мячи', 'и', 'стихи',',', 'а', 'любит', 'прозы'], X). 
% decompose(['Ира', 'любит', 'стихи', ',', 'любит', 'пьесы', ',', 'не', 'любит', 'игрушки'], X).