solve(X):-
    permutation([5,4,3,2,1],[P1,P2,P3,P4,P5]),
    permutation([5,4,3,2,1],[C1,C2,C3,C4,C5]),
    permutation([5,4,3,2,1],[F1,F2,F3,F4,F5]),
    permutation([5,4,3,2,1],[S1,S2,S3,S4,S5]),
    permutation([5,4,3,2,1],[V1,V2,V3,V4,V5]),

    permutation([_,B,B,B,B],[P3,C3,F3,S3,V3]),

    S5 =:= 6 - 1,
    V5 =:= 6 - 3,
    Sc1 is P1 + C1 + F1 + S1 + V1,
    Sc1 = 24,
    P2+C2+F2+S2+V2<P1+C1+F1+S1+V1,
    P3+C3+F3+S3+V3<P2+C2+F2+S2+V2,
    P4+C4+F4+S4+V4<P3+C3+F3+S3+V3,
    P5+C5+F5+S5+V5<P4+C4+F4+S4+V4,
    !,
    X is 6- S2.