/* 
Римским числом считается число состоящие из I, V, X, L, C, D, M и составленное по следующим правилам
1. После любого знака может стоять меньшее число (MI - допустимая конструкция)
2. Перед знаками I, X, C, M могут стоять лишь два ближайших в бОльшую сторону числа (т.е. все возможные комбинации с меньшим знаком слева
выглядят так: IV, IX, XL, XC, CD, CM)
3. Знаки I, X, C, M: один и тот же знак подряд, но не более 3 раз (IIII - не допустимо)
4. V, L, D:  один и тот же знак не идет друг за другом (VV - не допустимо)

Максимально возможным числом при соблюдении данных правил будет 3999, т.е. MMMIM
*/

#include <stdio.h>
#include <stdbool.h>

typedef enum {
    OUT_OF_WORD,
    IN_WORD,
    I, V, X, L, C, D, M, 
    END_GOOD_WORD,
    END_BAD_WORD
} state;

// Разделитель или нет
bool is_sep(int c){
    return (c == ' ') || (c == '\n') || (c == EOF);
}

// Принадлежит ли знак римской СС
bool is_rome(int c){
    return (c == 'I' || c == 'V' || c == 'X' || c == 'L' || c == 'C' || c == 'D' || c == 'M');
}


int main(){
    state s = OUT_OF_WORD;
    int n = 0; // кол-во подходящих чисел
    int c = 0; // текущий символ
    int i = 0; // сколько одинаковых чисел идет подряд
    
    while((c = getchar()) != EOF){
        if (s == END_GOOD_WORD) { 
            n++; 
            s = OUT_OF_WORD;
            i = 0;
            //printf("%d\n", n); }
        }
        // char x = c;
        // printf("%c %d;", x, s);
        if (i > 3) { i = 0; s = END_BAD_WORD; }
        if (s == OUT_OF_WORD){
            i = 0;
            if(is_sep(c)) { s = OUT_OF_WORD; }
            else { s = IN_WORD; }
        }
        // Определяем, что за знак перед нами
        if(s == IN_WORD){
            if(!is_rome(c)){ s = END_BAD_WORD; }
            else if(c == 'I') { i++; s = I; }
            else if(c == 'V') { s = V; }
            else if(c == 'X') { i++; s = X; }
            else if(c == 'L') { s = L; }
            else if(c == 'C') { i++; s = C; }
            else if(c == 'D') { s = D; }
            else if(c == 'M') { i++; s = M; }
            else { s = OUT_OF_WORD; }
        }
        
        else if (s == I){
            if (is_sep(c)){ s = END_GOOD_WORD; }
            else if (c == 'I') { i ++; s = I; }
            else if (c == 'V') { i = 0; s = V; }
            else if (c == 'X') { i = 0; s = X; }
            else { s = END_BAD_WORD; }
        }
        
        else if (s == V){
            if (is_sep(c)) { s = END_GOOD_WORD; }
            else if (c == 'I') { s = I; }
            else { s = END_BAD_WORD; }
        }
        
        else if (s == X){
            if (is_sep(c)) { s = END_GOOD_WORD; }
            else if (c == 'X') { i++; s = X; }
            else if (c == 'L') { i = 0; s = L; }
            else if (c == 'C') { i = 0; s = C; }
            else { s = END_BAD_WORD; }
        }
         
        else if (s == L){
            if (is_sep(c)) { s = END_GOOD_WORD; }
            else if (c == 'I') { s = I; }
            else if (c == 'V') { s = V; }
            else if (c == 'X') { s = X; }
            else { s = END_BAD_WORD; }
        }
        
        else if (s == C){
            if (is_sep(c)) { s = END_GOOD_WORD; }
            else if (c == 'I') { i = 0; s = I; }
            else if (c == 'V') { i = 0; s = V; }
            else if (c == 'X') { i = 0; s = X; }
            else if (c == 'L') { i = 0; s = L; }
            else if (c == 'C') { i++; s = C; }
            else if (c == 'D') { i = 0; s = D; }
            else if (c == 'M') { i = 0; s = M; }
            else { s = END_BAD_WORD; }
        }
        
        else if (s == D){
            if (is_sep(c)) { s = END_GOOD_WORD; }
            else if (c == 'I') { s = I; }
            else if (c == 'V') { s = V; }
            else if (c == 'X') { s = X; }
            else if (c == 'L') { s = L; }
            else if (c == 'C') { s = C; }
            else { s = END_BAD_WORD; }
        }
        
        else if (s == M){
            if (is_sep(c)) { s = END_GOOD_WORD; }
            else if (c == 'I') { i = 0; s = I; }
            else if (c == 'V') { i = 0; s = V; }
            else if (c == 'X') { i = 0; s = X; }
            else if (c == 'L') { i = 0; s = L; }
            else if (c == 'C') { i = 0; s = C; }
            else if (c == 'D') { i = 0; s = D; }
            else if (c == 'M') { i++; s = M; }
            else { s = END_BAD_WORD; }
        }
        
        else if (s == END_BAD_WORD){
            if(is_sep(c)){ s = OUT_OF_WORD; }
            else{ s = END_BAD_WORD; } 
        }
        // printf("%d\n", s);
    }
    
    if (s != END_BAD_WORD && s!= OUT_OF_WORD) { n++; }
    printf("%d\n", n);
}


