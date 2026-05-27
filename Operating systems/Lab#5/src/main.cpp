#include "lib.h"
#include <stdio.h>

int main() {
  int command = 0;
  printf("To compute count of prime numbers on [A, B] enter -- 1.Args: start finish\n");
  printf("To compute E enter -- 2.Args: argument\n");

  //std::cin >> command
  while (scanf("%d", &command) != EOF) {
	switch (command) {
	  case 1: {
		// Границы отрезка
		int start, finish;

		if (scanf("%d %d", &start, &finish) == 2) {
		  printf("Count: %d\n", PrimeCount(start, finish));
		}
		break;
	  }
	  case 2: {
		int argument;

		if (scanf("%d", &argument) == 1) {
		  printf("E is: %f\n", E(argument));
		}

		break;
	  }
	  default: {
		printf("This command is not supported, enter 1 or 2\n");
	  }
	}
  }

  return 0;
}
