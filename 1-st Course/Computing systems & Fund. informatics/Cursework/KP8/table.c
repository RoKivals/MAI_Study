#include "table.h"
//#include <stdio.h>

// Печатаем таблицу.
void PrintTable(row *arr, const int size) {
  printf("+---------+------------------------------------------------+\n");
  printf("|   Key   |                      Value                     |\n");
  printf("+---------+------------------------------------------------+\n");
  for (int i = 0; i < size; i++)
	printf("|%9ld|%48s|\n", arr[i]._key, arr[i]._str);

  printf("+---------+------------------------------------------------+\n");
}

// Получаем строку из файла
void GetRow(FILE *stream, char *str, const int size) {
  //счётчик, чтобы строка не была больше указанного кол-ва знаков (один символ уходит на обозначение конца строки).
  int cnt = 0;
  char ch;
  while ((ch = getc(stream)) != '\n' && cnt < size - 1)
	str[cnt++] = ch;
  str[cnt] = '\0';
}

// Смена мест между строками
void SwapRows(row *r1, row *r2) {
  row tmp;
  tmp = *r1;
  *r1 = *r2;
  *r2 = tmp;
}

