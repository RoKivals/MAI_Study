#include <stdio.h>
#include <ctype.h>

typedef int bool;

void add_num16(int *num, char digit){
    int idig;
    if (digit >= 'A'){
        idig = digit - 'A' + 10;
    } else {
        idig = digit - '0';
    }
    *num *= 16;
    *num += idig;
}

void print16(int num, char prefix) {
    if (prefix != '\0') {
        putchar(prefix);
    }
    printf("%X", num);
    putchar('\n');
}


int main(){
//    int n=0xaA98fF1;

    char c, prefix = '\0', our_prefix = '\0', next_prefix = '\0';
    bool brand_new_word=1, is_num=0, brand_new_string=1, got1=0, got2=0;
    int our_num=0x0, num=0x0, next_num=0x0;
    while (scanf("%c", &c)!=EOF){
        if (c=='\n'){
            brand_new_string=1;
            brand_new_word=1;
            if (is_num){
                if (got2){
                    print16(next_num, next_prefix);
                } else if (got1){
                    print16(our_num, our_prefix);
                } else {
                    printf("No\n");
                }
            } else if (got2){
                print16(our_num, our_prefix);
            } else printf("No\n");
            is_num=0;
            got1=0;
            got2=0;
            num=0x0;
            our_prefix = '\0';
            next_prefix = '\0';
            prefix = '\0';
            continue;
            //TODO
            // Но может быть без \n последняя строка (echo -n)
        }
        brand_new_string=0;
        if ((c=='\t') || (c=='\r') || (c==' ') || (c==',')){
            if (is_num){
                if (got2){
                    our_num = next_num;
                    our_prefix = next_prefix;
                    next_num = num;
                    next_prefix = prefix;
                } else if (got1){
                    next_num = num;
                    next_prefix = prefix;
                    got2 = 1;
                } else {
                    our_num = num;
                    our_prefix = prefix;
                    got1 = 1;
                }
            }
            brand_new_word=1;
            prefix = '\0';
            is_num = 0;
            num=0x0;
            continue;
        }
        if (((c=='+') || (c=='-')) && brand_new_word) {
            prefix = c;
            is_num = 1;
            brand_new_word = 0;
            continue;
        }
        c=toupper(c);
        if (((c>='0' && c<='9') || (c>='A' && c<='F')) && (brand_new_word || is_num)){
            is_num = 1;
            brand_new_word = 0;
            add_num16(&num, c);
        } else {
            is_num = 0;
            num=0x0;
            brand_new_word = 0;
        }
    }
    if (!brand_new_string){
        if (is_num){
            if (got2){
                print16(next_num, next_prefix);
            } else if (got1){
                print16(our_num, our_prefix);
            } else {
                printf("No\n");
            }
        } else if (got2){
            print16(our_num, our_prefix);
        }
    }
    return 0;
}