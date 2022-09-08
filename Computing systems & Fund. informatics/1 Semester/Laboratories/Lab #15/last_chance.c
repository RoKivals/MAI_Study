/*
						15 лаба (8 вариант - СМЭРТЬ)
    Замена элементов побочной диагонали суммами элементов проходящих через них главной
    и других || диагоналей.
    Если я правильно понял задание, то работает примерно так:
    берём элемент побочной диагонали, смотрим есть ли для него диагональ параллельная главной (в том числе она сама)
    суммируем элементы этой диагонали и ими заменяем первончальный элемент.
    Если не так, то задания надо нормально писать....
 */

#include <stdio.h>

void read_matrix(int count_of_rows, int count_of_columns, int matrix[][count_of_columns]) {
  for (int x = 0; x < count_of_rows; x++) {
	for (int i = 0; i < count_of_columns; i++) {
	  scanf("%d", &matrix[x][i]);
	}
  }
}

void print_matrix(int count_of_rows, int count_of_columns, int matrix[][count_of_columns]) {
  for (int x = 0; x < count_of_rows; x++) {
	printf("\n");
	for (int i = 0; i < count_of_columns; i++) {
	  printf("%d ", matrix[x][i]);
	}
  }
  printf("\n");
}

void change_matrix (int matrix_order, int matrix[][matrix_order]) {
  for (int i = 1; i < (matrix_order + 1) / 2; i++) {
	int upper_sum = matrix[i] [matrix_order - i - 1],
	lower_sum = matrix[matrix_order - i - 1][i];

	for (int x = 1; x <= i; x++) {
	  upper_sum += matrix[i - x][matrix_order - i - 1 - x];
	  upper_sum += matrix[i + x][matrix_order - i - 1 + x];
	  lower_sum += matrix[matrix_order - i - 1 + x][i + x];
	  lower_sum += matrix[matrix_order - i - 1 - x][i - x];

	}
	matrix[i][matrix_order - i - 1] = upper_sum;
	matrix[matrix_order - i - 1][i] = lower_sum;
  }
}

int main() {
  int size_matrix;
  scanf("%d", &size_matrix);
  int matrix[size_matrix][size_matrix];
  read_matrix(size_matrix, size_matrix, matrix);
  change_matrix(size_matrix, matrix);
  print_matrix(size_matrix, size_matrix, matrix);
}

