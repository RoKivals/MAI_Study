#include <stdio.h>
#include <stdbool.h>
#include "parsing.h"
#include "tree.h"
//Подсчитать количество уровней дерева выражений

int main() {

  queue q;
  QCreate(&q);

  res_read_expression res = read_expression(&q);
  switch (res) {
	case READ_EXPRESSION_SUCCESS: break;
	case READ_EXPRESSION_INVALID_INPUT: printf("Invalid input\n\n");
	  exit(0);
	case READ_EXPRESSION_UNEXPECTED_INPUT: printf("Unexpected input\n\n");
	  exit(1);
	case READ_EXPRESSION_UNEXPECTED_LBR: printf("Проверьте правильность расставленных открывающих скобок!\n\n");
	  exit(2);
	case READ_EXPRESSION_UNEXPECTED_RBR: printf("Проверьте правильность расставленных закрывающих скобок!\n\n");
	  exit(3);
  }

  tree asd;
  asd = BuildTokTree(&q);

  printf("Our cool expression ↑:\n\n\n");

  printf("Our Tree expression:\n");
  //simple_print(asd);

  PrintTokTreeDfs(asd, 0);
  printf("\n\n\n");

  printf("Our Great Tree:\n");
  PrintTokTree(asd, 0, false);

  printf("\nDepth of our tree:\n");
  TokDepth(asd);

  QDestroy(&q);

  DestroyTokTree(asd);

  return 0;
}

