#include <stdio.h>
//Выбрать идущие подряд тройки цифр, упорядоченных по возрастанию.
//На вход программе даётся число, на выходе все возрастающие тройки чисел, идущих подряд

long aabs(long num){
    if(num < 0){
        return -num;
    }
    return num;
}

int isIncreasing(long tr){
    if((tr % 10 > tr % 100 / 10) && (tr % 100 / 10 > tr % 1000 / 100)){
        return 1;
    }
    return 0;
}

void addZeros(long tr){
    if(tr < 100 && tr >= 10){
        printf("0");
    }
}

void triples(long num){
    while((num >= 100 && num > 0) || (num <= -100 && num < 0)){
        long tr = aabs(num % 1000);
        if(isIncreasing(tr)){
            addZeros(tr);
            printf("%ld\n", tr);
        }
        num /= 10;
    }
}

int main() {
    long num;
    while (scanf("%ld", &num) == 1) {
        triples(num);
        num = 0;
    }
    triples(aabs(num));
    return 0;
}
