#ifndef FUNC_H
#define FUNC_H

#include "table.h"

int BinSearch(const row *arr, const int size, const long int key); // Бинарный поиск в таблице по ключу
void Sort(row *arr, const int size); // Сортировка методом пузырька
void Scramble(row *arr, const int size); // Перемешивание данных в таблице
void Reverse(row *arr, const int size); // Реверс данных в таблице
int RandomAb(const int a, const int b); // Рандомное значение числа
int IsSorted(const row *arr, const int size); // Проверка на отсортированность таблицы


#endif
