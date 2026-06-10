oficer('General').
oficer('Polkovnik').
oficer('Mayor').
oficer('Capitan').
oficer('Letenant').

prof('Saper').
prof('Pehotinec').
prof('Tankist').
prof('Svyasist').
prof('Artelerist').

voen('Rodstv1', 'Prof1','Oficer', 'Prof', 'Rodstv2', 'Prof2').

unique([]):-!.
unique([Head|Tail]):-
    member(Head, Tail), !, fail;
    unique(Tail).

solve :-
	Solve = [voen(W,Wprof,X,Xprof,Y,Yprof),voen(X,Xprof,Y,Yprof,Z,Zprof),voen(Y,Yprof,Z,Zprof,Q,Qprof),
			voen(Z,Zprof,Q,Qprof,W,Wprof),voen(Q,Qprof,W,Wprof,X,Xprof)],

	oficer(X),
	oficer(Y),
	oficer(Z),
	oficer(Q),
	oficer(W),
	unique([X,Y,Z,Q,W]),
	prof(Xprof),
	prof(Yprof),
	prof(Zprof),
	prof(Qprof),
	prof(Wprof),
	unique([Xprof, Yprof, Zprof, Qprof, Wprof]),
	

	not(member(voen(_,_,'General','Svyasist',_,_),Solve)),
	not(member(voen(_,_,'Сapitan',_,'General',_),Solve)),
	not(member(voen(_,_,'General',_,'Capitan',_),Solve)),
	not(member(voen('Capitan',_,'General',_,_,_),Solve)),
	not(member(voen('General',_,'Сapitan',_,_,_),Solve)),
	not(member(voen(_,_,'General','Pehotinec',_,_),Solve)),
	not(member(voen(_,_,'General','Tankist',_,_),Solve)),
	not(member(voen(_,'Tankist',_,'Pehotinec',_,_),Solve)),
	not(member(voen(_,_,_,'Pehotinec',_,'Tankist'),Solve)),
	not(member(voen(_,_,'Letenant','Tankist',_,_),Solve)),
	not(member(voen(_,'Artelerist',_,'Tankist',_,_),Solve)),
	not(member(voen(_,_,_,'Tankist',_,'Artelerist'),Solve)),
	not(member(voen(_,_,'Letenant',_,_,'Tankist'),Solve)),
	not(member(voen(_,'Tankist','Letenant',_,_,_),Solve)),
	not(member(voen(_,_,'Polkovnik','Tankist',_,_),Solve)),
	not(member(voen(_,_,'Polkovnik',_,_,'Tankist'),Solve)),
	not(member(voen(_,'Tankist','Polkovnik',_,_,_),Solve)),
	not(member(voen(_,_,'General',_,'Polkovnik',_),Solve)),
	not(member(voen('Polkovnik',_,'General',_,_,_),Solve)),
	not(member(voen(_,_,'General','Artelerist',_,_),Solve)),
	not(member(voen(_,_,'General',_,_,'Artelerist'),Solve)),
	not(member(voen(_,'Artelerist','General',_,_,_),Solve)),
	not(member(voen('Letenant',_,'General',_,_,_),Solve)),
	X = 'General',
	write(X),write(" - "),write(Xprof),nl,
	write(Y),write(" - "),write(Yprof),nl,
	write(Z),write(" - "),write(Zprof),nl,
	write(Q),write(" - "),write(Qprof),nl,
	write(W),write(" - "),write(Wprof),nl.