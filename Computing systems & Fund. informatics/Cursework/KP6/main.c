#include <stdio.h>
#include <stdlib.h>
#include "vector.h"

// Текстовое меню для пользователя
void print_menu() {
  printf("1) Print matrix\n");
  printf("2) Complete the task\n0 for exit\n");
}

// Заполняем вектор
void vectorFill(Vector **CIP, Vector **PI, Vector **YE, FILE *file, int rows, int col) {
  int num;
  if (file) {
	int count = 0;
	// Очищаем память, если был передан не пустой вектор
	if (*CIP) free(*CIP);
	if (*PI) free(*PI);
	if (*YE) free(*YE);
	// Заполняем векторы значениями по умолчанию
	*CIP = VectorCreate(1);
	*PI = VectorCreate(1);
	*YE = VectorCreate(1);
	// Идём по вектору CIP и заполняем у него значения для начала каждой строки
	for (int i = 1; i <= rows; i++) {
	  VectorPushBack(*CIP, count);
	  // Сразу заполняем векторы PI и YE номером столбца и значением ячейки соответственно
	  for (int j = 1; j <= col; j++) {
		fscanf(file, "%d", &num);
		if (num) {
		  VectorPushBack(*PI, j);
		  VectorPushBack(*YE, num);
		  count++;
		}
	  }
	}
	fclose(file);
  }
}

// Вывод матрицы
void matrixPrint(Vector *CIP, Vector *PI, Vector *YE, int rows, int col) {
  int PIi = 0;
  int CIPi = 1;
  if (CIP && PI && YE) {
	for (int i = 0; i < VectorSize(CIP); i++) {
	  for (int j = 1; j <= col; j++) {
		if (PI->_data[PIi] == j) {
		  printf("%d ", YE->_data[PIi]);
		  PIi++;
		} else printf("0 ");
		  if (CIP->_data[CIPi] == PIi) {
			CIPi++;
			for (int k = 0; k < col - j; k++)
			  printf("0 ");
			printf("\n");
			break;
		  }
	  }
	}
  }
  printf("\n");
}

int main(void) {
  char c;
  int rows, col;
  FILE *file;
  Vector *CIP = NULL, *PI = NULL, *YE = NULL;
  Vector *v1 = NULL;
  v1 = VectorCreate(1);
  print_menu();
  while ((c = getchar()) != EOF) {
	if (c == '\n') continue;
	file = fopen("D:\\Slavik\\Coding\\C++\\MAI\\KP6\\matrix.txt", "r");
	if (file) {
	  fscanf(file, "%d%d", &rows, &col);
	  vectorFill(&CIP, &PI, &YE, file, rows, col);
	  switch (c) {
		case '1': matrixPrint(CIP, PI, YE, rows, col);
		  printf("CIP: ");
		  VectorPrint(CIP, VectorSize(CIP));
		  printf("PI: ");
		  VectorPrint(PI, VectorSize(PI));
		  printf("YE: ");
		  VectorPrint(YE, VectorSize(YE));
		  break;
		case '2': VectorResize(v1, col + 1);
		  for (int i = 0; i < VectorSize(v1); i++)
			v1->_data[i] = 0;
		  for (int i = 0; i < VectorSize(PI); i++)
			v1->_data[PI->_data[i]]++;
		  int max = 0, max_i1 = 0;
		  int max_i2 = 0;
		  int flag = 0;
		  for (int i = 1; i < VectorSize(v1); i++)
			if (v1->_data[i] > max) {
			  max = v1->_data[i];
			  max_i1 = i;
			  flag = 0;
			} else if (v1->_data[i] == max) {
			  max_i2 = max_i1;
			  max_i1 = i;
			  flag = 1;
			}
		  if (flag) max_i1 = max_i2;
		  printf("Column with the maximum number of elements: %d\n", max_i1);
		  int result = 1;
		  for (int i = 0; i < VectorSize(PI); i++)
			if (PI->_data[i] == max_i1) result *= YE->_data[i];
		  printf("The product of the elements of this column: %d\n", result);
		  break;
		case '0': VectorDestroy(CIP);
		  VectorDestroy(PI);
		  VectorDestroy(YE);
		  VectorDestroy(v1);
		  return 0;
		default: printf("There is no such option! I guess you made a mistake : (\nGoodbye!\n");
		  return 0;
	  }
	  print_menu();
	} else printf("File isn't exist!\n");
  }
}

