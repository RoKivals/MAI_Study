#include <stdio.h>
//глухие согласные: p f k t s c h

typedef unsigned int set;

const set oneSet = (set)1;

const set voicelessLiters =
        oneSet << ('p' - 'a') |
        oneSet << ('f' - 'a') |
        oneSet << ('k' - 'a') |
        oneSet << ('t' - 'a') |
        oneSet << ('s' - 'a') |
        oneSet << ('c' - 'a') |
        oneSet << ('h' - 'a');

const set vowelsLiters =
        oneSet << ('a' - 'a') |
        oneSet << ('e' - 'a') |
        oneSet << ('i' - 'a') |
        oneSet << ('o' - 'a') |
        oneSet << ('u' - 'a') |
        oneSet << ('y' - 'a');

int isSep(int x){
    return (x == ' ' || x == ',' || x == '\t' || x == '\n' || x == EOF);
}

int toLoverCase(int x){
    return x + ('a' - 'A')*(x >= 'A' && x <= 'Z');
}

int isLiter(int x){
    return (x >= 'a' && x <= 'z');
}

set withoutVowels(set s){
    return s - (s & vowelsLiters);
}

int isOnlyVowels(set s){
    return withoutVowels(s) == 0;
}

int isOnlyVoiceles(set s){
    return (withoutVowels(s) | voicelessLiters) == voicelessLiters;
}

int main(){
    int isVoiceless = 0;
    set liters = 0;
    int c;
    while ((c = getchar()) != EOF){
        if (isSep(c)){
            if (isOnlyVoiceles(liters) && !isOnlyVowels(liters))
                isVoiceless = 1;
            liters = 0;
        }
        else {
            if (isLiter(toLoverCase(c)))
                liters = liters | (oneSet << (toLoverCase(c) - 'a'));
        }
    }
    if (isOnlyVoiceles(liters) && !isOnlyVowels(liters))
                isVoiceless = 1;
    if (isVoiceless == 1){
        printf("Yes\n");

    } else {
        printf("No\n");
    }
    return 0;
}
