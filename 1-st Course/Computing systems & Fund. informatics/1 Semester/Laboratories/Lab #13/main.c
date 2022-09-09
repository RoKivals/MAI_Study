/*
 * 				19 вариант (13 лаба)
 * Есть ли слово, содержащее одну гласную букву (возможно несколько раз).
 * Примеры: Альфа, лава, канал и т.д.
 *
 * Особенность в битовом сдвиге между GCC и MinGW
 * Как работает сдвиг на MinGW я понятия не имею, его делал какой-то пьяница.
 * (поэтому в Clion работает только версия для англ букв)
 */

#include <stdio.h>
#include "stdbool.h"

// после побитового умножения, мы получим, что нужное нам множество - степень двойки
// поэтому делаем функцию, на проверку числа, является ли оно степенью 2
bool is_degree_of_two(unsigned long long digit) {
  while (digit) {
	if (digit == 1) {
	  return true;
	}
	if (digit % 2 == 0) {
	  digit /= 2;
	} else {
	  break;
	}
  }
  return false;
}

/*
 * Если символ из ASCII, то возвращаем его номер => 1 байт.
 * Если символ многобайтовый (2-4), то
 * 1. определяем кол-во байт
 * 2. создаём переменную для значащих бит
 * 3. считываем все байты и заносим их в переменную
 */
int letter_in_utf8() {
  unsigned int sym_code = getchar();
  if (sym_code == EOF) {
	return -1;
  }
  if (sym_code <= 127) {
	return sym_code;
  } else {
	/* первый октет (2-4) байтного символа всегда имеет две единицы
	 * вычитаем их и дальше находим оставшееся кол-во единиц, который указывают нам на кол-во байтов
	 * переменная k указывает нам на кол-во НЕзначащих единиц в первом октате
	 * т.е. k = 3, первый октет выглядит 1110****
	*/
	unsigned int count_of_bytes = sym_code - 192;
	int k = 2;
	if (count_of_bytes & 100000) {
	  k++;
	  count_of_bytes-=32;
	  if (count_of_bytes & 10000) {
		k++;
	  }
	}
	/*
	 * count_of_bytes - урезанный первый октет до первых (k-1) единиц
	 * так как первый октет уже был считан, поэтому мы убираем одну единицу
	 * для выхода из цикла после считывания оставшихся октетов
	 */
	count_of_bytes = sym_code;
	count_of_bytes = count_of_bytes >> (8 - k + 1);
	// leftover_bytes - значащие биты
	unsigned int leftover_bytes;
	leftover_bytes = sym_code << (32 - (8 - (k + 1)));
	leftover_bytes = leftover_bytes >> (32 - (8 - (k + 1)));
	/*
	 * считываем новый октет
	 * убираем ведущие символы (10)
	 * прибавляем их к значащим битам
	 */
	while (count_of_bytes) {
	  sym_code = getchar();
	  sym_code = sym_code << 26;
	  sym_code = sym_code >> 26;
	  leftover_bytes = leftover_bytes << 6;
	  leftover_bytes = leftover_bytes + sym_code;
	  count_of_bytes = count_of_bytes >> 1;
	}
	return leftover_bytes;
  }
}

// функция, которая возвращает номер буквы в её алфавите (начиная с 0)
// Русские: 0-32, English: 33 - 58
// необходимо для заполнения множества букв в слове
int ord_num_alphabet(int alpha) {
  int big_A = 1040, big_E = 1045, big_YO = 1025, big_YA = 1071, little_a = 1072, little_e = 1077, little_yo = 1105,
	  little_ya = 1103;
  if (alpha >= big_A && alpha <= big_E) {
	return alpha - big_A;
  }

  if (alpha > big_E && alpha <= big_YA) {
	return alpha - big_A + 1;
  }

  if (alpha >= little_a && alpha <= little_e) {
	return alpha - little_a;
  }

  if (alpha > little_e && alpha <= little_ya) {
	return alpha - little_a + 1;
  }

  if (alpha == big_YO || alpha == little_yo) {
	return 6;
  }

  if (alpha >= 'A' && alpha <= 'Z') {
	return 33 + alpha - 'A';
  }

  if (alpha >= 'a' && alpha <= 'z') {
	return 33 + alpha - 'a';
  }
  return -1;
}

unsigned long long alpha_to_set(int letter) {
  unsigned long long set = 0;
  int index = ord_num_alphabet(letter);
  if (index == -1) {
	return 0;
  } else {
	set = 1;
	set = set << index;
  }
  return set;
}

int processing_string() {
  unsigned long long russian_vowels = alpha_to_set(1040) | alpha_to_set(1045) | alpha_to_set(1025) | alpha_to_set(1048)
	  | alpha_to_set(1054) | alpha_to_set(1059) | alpha_to_set(1067) | alpha_to_set(1069) | alpha_to_set(1070)
	  | alpha_to_set(1071);
  unsigned long long eng_vowels =
	  alpha_to_set(65) | alpha_to_set(69) | alpha_to_set(73) | alpha_to_set(79) | alpha_to_set(85) | alpha_to_set(89);
  unsigned long long word = 0;
  int final_ans = 0;
  while (true) {
	int c = letter_in_utf8();
	if (c != '\n' && c != ' ' && c != ',' && c != '.' && c != '\t' && c != '\r' && ord_num_alphabet(c) != -1 && c != -1) {
	  word = word | alpha_to_set(c);
	} else if (c == '\n' || c == ' ' || c == ',' || c == '.' || c == '\t' || c == '\r' || c == -1) {
	  if ((is_degree_of_two(word & russian_vowels)) != (is_degree_of_two(word & eng_vowels))) {
		final_ans++;
	  }
	  word = 0;
	} else {
	  word = 0;
	}
	if (c == -1) {
	  break;
	}
  }
  return final_ans;
}

int main() {
  //  printf("%d", ord_num_alphabet(letter_in_utf8()));
	printf("%s : %d", "Count of words", processing_string());
}