#include <stdio.h>

/*
 * 1 1 1
 * 0 1 1
 * 0 0 1
 * Верхнетреугольная матрица
 */

/*
 * 1 0 0
 * 1 1 0
 * 1 1 1
 * Нижнетреугольная матрица
 * В общем, главная диагональ будет входить в обе подматрицы
 */

void printMatrix(int m, int mx[m][m]){
    for(int i = 0; i < m; i++){
        for(int j = 0; j < m; j++){
            printf("%d ", mx[i][j]);
        }
        printf("\n");
    }
}

void scanMatrix(int m, int mx[m][m]){
    for(int i = 0; i < m; i++){
        for(int j = 0; j < m; j++){
            scanf("%d", &mx[i][j]);
        }
    }
}

int maxLowerTriangle(int m, int mx[m][m]){
    int loverMax = mx[0][0];
    for(int i = 0; i < m; i++){
        for(int j = 0; j < m; j++){
            if(i >= j && mx[i][j] > loverMax)
                loverMax = mx[i][j];
        }
    }
    return loverMax;
}

void multiplyUpperTriangle(int m, int loverMax, int mx[m][m]){
    for(int i = 0; i < m; i++){
        for(int j = 0; j < m; j++){
            if(i <= j)
                mx[i][j] *= loverMax;
        }
    }
}

int main(){
    int m;
    scanf("%d", &m);
    int mx[m][m];
    scanMatrix(m, mx);
    int loverMax = maxLowerTriangle(m, mx);
    multiplyUpperTriangle(m,loverMax, mx);
    printMatrix(m, mx);
    return 0;
}
