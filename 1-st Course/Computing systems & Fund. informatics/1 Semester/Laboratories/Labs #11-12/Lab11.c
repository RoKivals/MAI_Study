/*
 * Подсчитать количество положительных десятичных чисел в строке, допустимых 16-бит процессорами (от -2^16 до 2^16)
 * */
#include <stdio.h>
#include <ctype.h>
#include <stdlib.h>
int processing_string() {
  int curr_alpha = 0, alpha_step = 0;
  // curr_alpha - текущее число, которое мы обрабатываем в строке
  // alpha_step - кол-во знаков в числе
  int final_ans = 0; // конечный ответ
  int sym;
  int sign = 0;
  sym = getchar();
  while (sym != EOF) {
	if (isdigit(sym) && sign == 0 && alpha_step >= 0) {
		curr_alpha = curr_alpha * 10 + (sym - '0');
		alpha_step++;
	} else {
	  if (sym == '\t' || sym == '\n' || sym == '\r' || sym == ' ') {
		if (curr_alpha > 0 && curr_alpha <= 32767 && alpha_step > 0 && sign == 0) {
		  final_ans++;
		}
		sign = 0;
		alpha_step = 0;
	  } else {
		alpha_step = -1;
		curr_alpha = 0;
	  }
	  if (sym == '-') {
		sign = 1;
	  }
	  if (sym == '+') {
		sign = 0;
		alpha_step = 0;
	  }
	}
	if ((sym = getchar()) == EOF) {
	  if (curr_alpha > 0 && curr_alpha <= 32767 && alpha_step > 0 && sign == 0) {
		final_ans++;
	  }
	  break;
	} else {
	  continue;
	}
  }
  return final_ans;
}
int main() {
  printf("%s:%d", "Result", processing_string());
  return 0;
}