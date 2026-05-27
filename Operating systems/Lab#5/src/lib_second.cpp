#include "lib.h"
#include <cmath>
#include <bits/stdc++.h>
#include <stdio.h>
#include <stdlib.h>
#include <malloc.h>


int PrimeCount(int start, int finish) {
  printf("Count prime digits by sieve of Eratosthenes\n");

  int curent_number;
  int *prime_number_list;
  int is_prime;
  finish = finish - 1;
  prime_number_list = (int *) malloc(finish * sizeof(int));

  for (int index = 0; index < finish; ++index) {
	prime_number_list[index] = index + 2;
  }

  for (int index = 0; index < finish; ++index) {
	curent_number = prime_number_list[index];
	is_prime = 0;

	for (int second_index = index + 1; second_index < finish; ++second_index) {
	  if (!(prime_number_list[second_index] % curent_number)) {
		for (int third_index = second_index; third_index < finish - 1; ++third_index) {
		  prime_number_list[third_index] = prime_number_list[third_index + 1];
		}

		is_prime = 1;
		--finish;
		--second_index;
	  }
	}

	if (is_prime == 0) {
	  break;
	}
  }

  int prime_count = 0;
  for (int index = 0; index < finish; ++index) {
	if (prime_number_list[index] >= start) {
	  ++prime_count;
	}
  }

  free(prime_number_list);

  return prime_count;
}

float E(int argument) {
  printf("Calculating a E by Taylor series\n");

  if (argument <= 0) {
	return -1;
  }

  float e = 1.0;
  float term = 1.0;

  for (int index = 1; index <= argument; ++index) {
	term /= static_cast<float>(index);
	e += term;
  }

  return e;
}
