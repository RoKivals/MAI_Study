#include <stdio.h>

int zo(int n){
    int zeroes=0, ones=0;
    if (n==0){
        return 0;
    }
    while (n!=0){
        if (n%2){
            ++ones;
        } else ++zeroes;
//        printf("%d",n%2);     //debug printf
        n/=2;
    }
//    printf("\n");     //debug printf

    return zeroes==ones;
}


int main() {
    int n;
    while (scanf("%d", &n)!=EOF){
        printf("%d\n",zo(n));
    }
    return 0;
}
