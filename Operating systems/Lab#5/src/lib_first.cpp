#include "lib.h"
#include <cmath>
#include <bits/stdc++.h>
#include <stdio.h>
#include <stdlib.h>

// Проверяем, является ли число простым
bool IsPrime(int digit) {
  for (int curr_div(2); curr_div <= sqrt(digit); ++curr_div) {
	if (digit % curr_div == 0) return false;
  }
  return true;
}

int PrimeCount(int start, int finish) {
  printf("Count prime digits by native algorithm\n");

  // Единица - не простое число, скипаем его
  if (start == 1) ++start;

  // Подсчёт результата (кол-ва простых чисел)
  int result = 0;

  for (; start <= finish; ++start) {
	if (IsPrime(start)) ++result;
  }
  return result;
}

// Вычисление значения экспоненты через 2-ой замечательный предел
float E(int argument) {
  printf("Calculating the E by second wonderful limit\n");

  if (argument <= 0) return -1;

  float e = 1.0;

  for (int index = 0; index < argument; ++index) {
	e *= 1 + 1 / static_cast<float>(argument);
  }

  return e;
}
