#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>

int mod(int a, int b) {
  return (b + a % b) % b;
}

int sign(int a) {
  if (a == 0) {
	return 0;
  } else if (a < 0) {
	return -1;
  } else {
	return 1;
  }
}

int max(int a, int b) {
  if (a > b) {
	return a;
  } else {
	return b;
  }
}

int min(int a, int b) {
  if (a < b)
  {
	return a;
  }
  else
  {
	return b;
  }
}

bool f(long long x, long long y) {
  return (((((x - 20) * (x - 20))) + (4 * y * y)) <= 100) ? true : false;
}

int main() {
  int i = -29, j = 3, l = 9;
  bool isabet = false;
  int i_curr = i, j_curr = j, l_curr = l;
  int d;
  for (int k = 0; k <50; k++) {
	if (f(i, j)) {
	  isabet = true;
	  break;
	}
	i_curr = i;
	j_curr = j;
	l_curr = l;
	d = k;
	i = mod(i_curr * max(j_curr, l_curr), 20) +  mod(j_curr * min(i_curr,l_curr),30) + k;
	j = mod(abs(i_curr - j_curr + l_curr - k) * sign(k-10),20);
	l = mod(abs(i_curr - j_curr) * l_curr - abs(j_curr - l_curr)*i_curr + abs(i_curr - l_curr) * j_curr,20) - k;
  }
  printf("%s\n%d %d %d %d\n", ((isabet) ? "Yes" : "No"), i, j, l, d);
  return 0;
}
