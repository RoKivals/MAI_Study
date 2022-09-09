/*
						14 лаба (8 вариант - не знаю, зачем он тут)
 	Реализовать спиральный вывод матрицы в строку.
 	То есть, в одну строку вывести матрицу по спирали => направо, вниз, налево, вверх
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

void print_matrix_by_spiral(int count_of_rows, int count_of_columns, int matrix[][count_of_columns]) {
  int x = 0, y = 0; // координаты движения
  int left_border = 0, upper_border = 0; // левая и верхняя границы
  int count_of_steps = 0; // кол-во выведенных чисел в реальный момент
  while (count_of_steps < count_of_columns * count_of_rows) {
	count_of_steps++;
	printf("%d ", matrix[y][x]);
	if (y == upper_border + 1 && x == left_border) {
	  upper_border++;
	  left_border++;
	}
	if (y == upper_border && x < count_of_columns - left_border - 1) {
	  x++;
	  continue;
	}
	if (x == count_of_columns - left_border - 1 && y < count_of_rows - upper_border - 1) {
	  y++;
	  continue;
	}
	if (y == count_of_rows - upper_border - 1 && x > left_border) {
	  x--;
	  continue;
	}
	if (x == left_border && y > upper_border) {
	  y--;
	  continue;
	}
  }
}


int main() {
	int count_of_rows, count_of_columns;
  	scanf("%d %d", &count_of_rows, &count_of_columns);
	int matrix[count_of_rows][count_of_columns];
	read_matrix(count_of_rows, count_of_columns, matrix);
  	print_matrix(count_of_rows, count_of_columns, matrix);
    print_matrix_by_spiral(count_of_rows, count_of_columns, matrix);
}

