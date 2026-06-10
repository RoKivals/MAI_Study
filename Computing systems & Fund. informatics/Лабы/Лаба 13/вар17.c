#include <stdio.h>
#include <string.h>
#include <stdbool.h>

#define UTF_CODE_a 1072
#define UTF_CODE_z 1103

typedef unsigned long long set;


bool is_sep(int c){
    return (c == ' ') || (c == '\n') || (c == EOF) || (c == ',');
}

// k раз считывает байт с ввода, переводит в число и прибавляет к res
int get_parsing_another_byte(int k, int res){
    for(int i = 0; i < k; i ++){
        int another_byte = getchar();
        res = res<<6;
        res += another_byte - 128; 
    }
    return res;
}

// k раз берет следующий байт из *a, переводит в число и прибавляет к res
int char_parsing_another_byte(int k, int res, char * a){
    for(int i = 1; i < k; i ++){
        int another_byte = a[i] + 256;  // так как a[i] начинается с 1 и переводится в доп код, и чтобы исправить это, прибавляем 256
        res = res<<6;
        res += another_byte - 128; 
    }
    return res;
}

// Получает первый байт буквы, переводит его в число и далее 
// в зависимости от флага посылает или в get_parsing_another_byte, или в char_parsing_another_byte
// flag = 1 если буква уже дана; flag = 2 если букву надо брать с ввода
int parsing_fst_byte(int fst_byte, int flag){
    int res = 0;
    if(fst_byte < 128 || is_sep(fst_byte)){ 
        return fst_byte;  
    }
    int k = 1;
    int p = 5;
    fst_byte -= 192;
    res += fst_byte;
    while((fst_byte - (2<<p)) >= 0){
        fst_byte -= (2<<p);
        k ++;
        p --;
    }
    
    if(flag == 2){
        return get_parsing_another_byte(k, res);
    }
    else{
        return res;
    }
}

int letter_to_code(char * a){
     // так как a[0] начинается с 1 и переводится в доп код, исправляем это, прибавляя 256
     int a_idx = parsing_fst_byte(a[0] + 256, 1); 
     a_idx = char_parsing_another_byte(2, a_idx, a);
     return a_idx;
}

// из кода utf-8 в индекс буквы в алфавите
int code_to_index(int c){
    if((c >= UTF_CODE_a) && (c <= UTF_CODE_z)){
        return c - UTF_CODE_a;
    }
    else { return -1; }
}

set code_to_set(int c){
    int idx = code_to_index(c);
    if(idx == -1){
        return 0;
    }
    return ((set)1)<<idx;
}

// из заданной буквы в set
set letter_to_set(char * c){
    return code_to_set(letter_to_code(c));
}

// берет с ввода и в результате получает код utf8
int get_utf8_char(){
    int fst_byte = getchar();
    int res = parsing_fst_byte(fst_byte, 2);
    return res;
}
int main(){
    bool flag = false;
    int c = 0;
    int k = 1;
    int state = 0;
    set s1 = 0;
    set s2 = 0;
    
    set vowels = letter_to_set("а") | letter_to_set("е") | letter_to_set("и") | letter_to_set("о") | letter_to_set("у");
    vowels = vowels | letter_to_set("э") | letter_to_set("ю") | letter_to_set("я") | letter_to_set("ы");
    do{
        c = get_utf8_char();
        set c_set = code_to_set(c);
        if(state == 0){
            if(is_sep(c)) { 
                state = 0;
            }
            else {
                state = k;
            }
        }
        
        if(state == 1){ 
            if(c_set & vowels){
                s1 = s1|c_set;
            }
            if(is_sep(c)){ 
                state = 0; 
                k = 2;
                s2 = 0;
            }
            else { state = 1; }
        }
        
        if(state == 2){
            if(c_set & vowels){
                s2 = s2|c_set;
            }
            if(is_sep(c)){
                state = 3;
            }
            else{ state = 2; }
        }
        
        if(state == 3){
            if(is_sep(c)){
                if(!(s1&s2)){ flag = true; }
                s1 = s2;
                s2 = 0;
                state = 0;
                k = 2;
            }
            else{
                state = 0;
                k = 2;
            }
        }
    } while(c != EOF);
    
    if(flag){ printf("\nТакие слова есть\n"); }
    else{ printf("\nНет\n"); }
    return 0;   
}
