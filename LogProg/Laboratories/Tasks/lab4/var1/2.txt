parent(alexei, tolia).
parent(alexei, volodia).
parent(alexei, vitia).
parent(tolia, tima).
parent(tima, sasha).

% ввели оператор : для словаря
:- op(200, xfy, ':').

% словарь, генерирующий различные формы имён
gen(File) :- File = [
    'alexei':pad('именит'):['alexei'],
    'alexei':pad('родит'):['alexeia'],
    'alexei':pad('чей'):['leshin'],
    'tolia':pad('именит'):['tolia'],
    'tolia':pad('родит'):['toli'],
    'tolia':pad('чей'):['tolin'],
    'volodia':pad('именит'):['volodia'],
    'volodia':pad('родит'):['volodi'],
    'volodia':pad('чей'):['volodin'],
    'sasha':pad('именит'):['sasha'],
    'sasha':pad('родит'):['sashi'],
    'sasha':pad('чей'):['sashin'],
    'tima':pad('именит'):['tima'],
    'tima':pad('родит'):['timi'],
    'tima':pad('чей'):['timin'],
    'vitia':pad('именит'):['vitia'],
    'vitia':pad('родит'):['viti'],
    'vitia':pad('чей'):['vitin']
].

% предикат поиска по словарю
fin(X, XC, K, File) :- 
    member(M, File),
    condition(X, XC, K, M).

condition(X, XC, K, XC:K:L) :- member(X, L).

%предикат анализа фразы
answer([X,Y,_,_], Res) :- 
    X = 'kto',
    an_name(Y1, pad('чей'), Y),
    parent(P,Y1), parent(P,Res), Y1 \= Res.

answer([X,_,Z,_], Res) :- 
    X = 'kto',
    an_name(Z1, pad('родит'), Z),
    parent(P,Z1), parent(P,Res), Z1 \= Res.

answer([X,_,Z,_], Res) :- 
    X = 'chei',
    an_name(Z1, pad('именит'), Z),
    parent(P,Z1), parent(P,Res), Z1 \= Res.

answer([X,_,Z,_], _) :- 
    an_name(X1, pad('именит'), X),
    an_name(Z1, pad('родит'), Z),
    parent(P,X1), parent(P,Z1), X1 \= Z1.

% предикат анализирует слово=а, проверяющий что это слово - имя из базы фактов
an_name(XC, K, X) :- gen(File), fin(X, XC, K, File).