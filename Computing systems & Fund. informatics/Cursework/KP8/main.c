#include <stdio.h>
#include "func.h"

void printMenu() {
  printf("Menu:\n");
  printf("1) Print the table\n");
  printf("2) Find a table element by key\n");
  printf("3) Sort the table by keys\n");
  printf("4) Shuffle the data in the table\n");
  printf("5) Reverse the data in the table\n");
  printf("6) Finish the program\n");
}

int main() {
  const int N = 50;
  int cnt, action;
  long key;
  char ch;
  row arr[N];
  FILE *file = fopen("D:\\Slavik\\Coding\\C++\\Laboratory\\KP8\\input.txt", "r");
  if (file == NULL) {
	printf("Error opening file\n");
	return 0;
  }
  cnt = 0;
  while (cnt < N && fscanf(file, "%ld", &arr[cnt]._key) == 1) {
	fscanf(file, "%c", &ch);
	GetRow(file, arr[cnt]._str, sizeof(arr[cnt]._str));
	cnt++;
  }
  fclose(file);
  do {
	printMenu();
	scanf("%d", &action);
	switch (action) {
	  case 1: PrintTable(arr, cnt);
		break;
	  case 2:
		if (!IsSorted(arr, cnt)) {
		  printf("The table is not sorted\n");
		} else {
		  int i;
		  printf("Enter the key: ");
		  scanf("%ld", &key);
		  i = BinSearch(arr, cnt, key);
		  if (i > -1) {
			printf("Element by this key: %s\n", arr[i]._str);
		  } else printf("There is no element with this key in the table!\n");
		}
		break;
	  case 3: Sort(arr, cnt);
		break;
	  case 4: Scramble(arr, cnt);
		break;
	  case 5: Reverse(arr, cnt);
		break;
	  case 6: break;
	  default: printf("There is no such item in the menu! Try a different value\n");
		break;
	}
  } while (action != 6);
  return 0;
}
