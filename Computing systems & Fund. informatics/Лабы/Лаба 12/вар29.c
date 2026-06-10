#include <stdio.h>

int pow_int(int a, int*b){
    int base = a;
    a = 1;
    for(int i = 0; i < *b; i++){
        a *= base;
    }
    return a;
}

int pos_num_to_8(int a, int*k){
    int res = 0, b = *k;   
    while(a > 0){
        res += a % 8 * (pow_int(10, &b));
        a /= 8;
        b++;
    }
    *k = b;
    return res;
}

int invers(int a, int k){
    int full_7 = 7;
    for(int i = 0; i < k; i++){
        full_7 = full_7 * 10 + 7;
    }
    return full_7 - a + 1;
}

int main(){
    int num, digits = 0;
    scanf("%d", &num);
    if(num >= 0){
        printf("%d\n", pos_num_to_8(num, &digits));
    }
    else{
        num = pos_num_to_8(-num, &digits);
        printf("%d\n", invers(num, digits));
        
    }
    return 0;
}
