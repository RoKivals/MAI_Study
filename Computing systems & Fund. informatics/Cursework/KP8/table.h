#ifndef TABLE_H
#define TABLE_H

#include <stdio.h>

typedef struct _row {
  long int _key;
  char _str[120];
} row;

void PrintTable(row *arr, const int size); // Печать таблицы
void GetRow(FILE *stream, char *str, const int size); // Получить одну строку из файла
void SwapRows(row *r1, row *r2); // Поменять две строки местами


#endif
