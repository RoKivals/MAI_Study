#include "duque.h"
#include "MergeSort.c"
#include <stdio.h>

int main()
{
  printf("Enter the deque size\n");
  int n;
  int a;
  deque d;
  deque_create(&d);
  scanf("%d", &n);
  printf("Enter deque elements\n");
  for (int i = 0; i < n; i++) {
	scanf("%d", &a);
	push_back(&d, a);
  }
  merge_sort(&d);
  printf("Sorted deque\n");
  while(!empty(&d)) {
	printf("%d ", first_front(&d));
	pop_front(&d);
  }
  printf("\n");
  return 0;
}