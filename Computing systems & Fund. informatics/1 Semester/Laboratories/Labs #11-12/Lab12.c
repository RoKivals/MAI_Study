/*
 * Задание:
 * Упорядочить цифры числа попарно по убыванию.
 */

#include <stdio.h>
#include <ctype.h>
#include <stdlib.h>

// знаю, что можно было без этого, так как есть alpha_step, но изначально делал без него и уже решил оставить
int count_of_digits(int a) {
  int num = 0;
  while (a) {
	num++;
	a /= 10;
  }
  return num;
}

int degree_of_ten(int degree) {
  int itog;
  if (degree == 0) {
	itog = 1;
  } else {
	itog = 10;
  }
  for (int x = 1; x < degree; x++) {
	itog *= 10;
  }
  return itog;
}
/*
 * функция, которая по количеству разрядов берёт попарно первый и второй элемент, сравнивает их и строит новое число
 * 1. берём самоую первую цифру (делим на количество чисел - 1) - первое число пары
 * 2. убираем первую цифру (остаток от деления на кол-во - 1)
 * 3. повторяем тоже самое с текущим ведущим элементом - второе число пары
 * 4. сравниваем
 * 5. строим новое число, домножая на 10 и прибавляя в порядке убывания
 */
int reorganization_of_digit(int a) {
  int num_of_digits = count_of_digits(a);
  int first_of_pair, second_of_pair;
  int reorganized_digit = 0;
  while (num_of_digits) {
	if (num_of_digits == 1) {
	  first_of_pair = a % 10;
	  reorganized_digit = reorganized_digit * 10 + first_of_pair;
	  num_of_digits--;
	}
	else {
	  first_of_pair = a / degree_of_ten(num_of_digits - 1);
	  a = a % degree_of_ten(num_of_digits - 1);
	  num_of_digits--;
	  second_of_pair = a / degree_of_ten(num_of_digits - 1);
	  a = a % degree_of_ten(num_of_digits - 1);
	  num_of_digits--;
	  if (first_of_pair < second_of_pair) {
		reorganized_digit = reorganized_digit * 10 + second_of_pair;
		reorganized_digit = reorganized_digit * 10 + first_of_pair;
	  } else {
		reorganized_digit = reorganized_digit * 10 + first_of_pair;
		reorganized_digit = reorganized_digit * 10 + second_of_pair;
	  }
	}
  }
  return reorganized_digit;
}

void processing_string() {
  int sym, ord_num = 0;
  while ((scanf("%d", &sym)) == EOF) {
		ord_num ++;
		printf("%d : %d\n", ord_num, reorganization_of_digit(sym));
	  }
  }
  
int main() {
  processing_string();
}

