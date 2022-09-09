#include "tree.h"
#include "tree_stack.h"
#include "queue.h"

bool IsBinary(token a) {
  if (a.op == '~') {
	return false;
  }
  return true;
}

tree AddList(token a) {
  tree t = malloc(sizeof(node));
  t->data = a;
  t->left = t->right = NULL;
  return t;
}

tree AddBinOp(token op, tree left, tree right) {
  tree t = malloc(sizeof(node));
  t->data = op;
  t->left = left;
  t->right = right;
  return t;
}

tree AddUnoOp(token op, tree right) {
  tree t = malloc(sizeof(node));
  t->data = op;
  t->right = right;
  return t;
}

//построить
tree BuildTokTree(queue *q) {
  if (QIsEmpty(q)) {
	printf("Введите выражение для постройки дерева!\n");
	exit(4);
  }

  stack_dbl s;
  tree_stok_create(&s);

  tree Tree;
  while (!QIsEmpty(q)) {
	token a;
	a = QPopFront(q);
	//printf ("var_name: %s\nconst_val: %lf\nop: %c\n\n", a.var_name, a.const_val, a.op);
	//если переменная      //если констаната
	if (a.type == TOK_CONST || a.type == TOK_VAR) {
	  tree_stok_push(&s, AddList(a));
	}
	  //если знак
	else if (a.type == TOK_OP) {
	  if (IsBinary(a)) {
		tree right = tree_stok_pop(&s);
		tree left = tree_stok_pop(&s);

		tree_stok_push(&s, AddBinOp(a, left, right));

		Tree = AddBinOp(a, left, right);
	  } else {
		tree uno = tree_stok_pop(&s);

		tree_stok_push(&s, AddUnoOp(a, uno));

		Tree = AddUnoOp(a, uno);
	  }
	}
  }
  return Tree;
}


void TreeReadToken(token *tok) {
  if (tok->type == TOK_OP) {
	printf("%c ", tok->op);
  } else if (tok->type == TOK_VAR) {
	printf("%s ", tok->var_name);
  } else if (tok->type == TOK_CONST) {
	printf("%.2lf ", tok->const_val);
  }
}

void SimplePrint(tree t) {
  if (t == NULL) {
	return;
  }

  SimplePrint(t->left);
  TreeReadToken(&t->data);
  SimplePrint(t->right);
}

int get_prior_op(token tok) {
  char sum = '+';
  char razn = '-';
  char umn = '*';
  char del = '/';
  char step = '^';
  char uno = '~';

  if (tok.op == sum) {
	return SUM;
  } else if (tok.op == razn) {
	return RAZN;
  } else if (tok.op == umn) {
	return UMN;
  } else if (tok.op == del) {
	return DEL;
  } else if (tok.op == step) {
	return STEP;
  } else if (tok.op == uno) {
	return UNO;
  }
  return 999;//тут мы поняли, что у нас LBR
}

void PrintTokTreeDfs(tree t, int h) {
  if (t == NULL)
	return;
  bool left_br = false, right_br = false;
  if (t->data.type == TOK_OP && t->data.op != '~' && t->left->data.type == TOK_OP
	  && get_prior_op(t->data) < get_prior_op(t->left->data)) {
	printf("( ");
	left_br = true;
  }
  PrintTokTreeDfs(t->left, h + 1);
  if (left_br) {
	printf(") ");
  }
  TreeReadToken(&t->data);
  if (t->data.type == TOK_OP && t->right->data.type == TOK_OP && get_prior_op(t->data) < get_prior_op(t->right->data)) {
	printf("( ");
	right_br = true;
  }
  PrintTokTreeDfs(t->right, h + 1);
  if (right_br) {
	printf(") ");
  }
}

void PrintTokTree(tree t, int level, bool isleft) {
  if (t == NULL) {
	return;
  }

  PrintTokTree(t->right, level + 1, false);
  for (int i = 0; i < level; i++) {
	printf("        ");
  }
  if (level != 0) {
	if (isleft) {
	  printf("╙");
	} else {
	  printf("╓");
	}
  } else {
	printf("╟");
  }

  printf("───────█");
  TreeReadToken(&t->data);
  printf("\n");

  PrintTokTree(t->left, level + 1, true);
}

bool is_op_umn(token tok) {
  if (tok.type == TOK_OP && tok.op == '*') {
	//printf("var_name: %s\nconst_val: %lf\nop: %c\ntype: %d\n", tok->var_name, tok->const_val, tok->op, tok->type);
	return true;
  }
  return false;
}

bool is_const_1(token tok) {
  if (tok.type == TOK_CONST && tok.const_val == 1) {
	//printf("var_name: %s\nconst_val: %lf\nop: %c\ntype: %d\n", tok->var_name, tok->const_val, tok->op, tok->type);
	return true;
  }
  return false;
}

int TreeDepth(tree t) {
  if (t == NULL) {
	return 0;
  }
  int left, right;
  if (t->left != NULL) {
	left = TreeDepth(t->left);
  } else {
	left = -1;
  }
  if (t->right != NULL) {
	right = TreeDepth(t->right);
  } else {
	right = -1;
  }
  int max;
  if (left > right) {
	max = left;
  } else {
	max = right;
  }
  return (max + 1);
}
void TokDepth(tree t) {
  printf("%d\n", TreeDepth(t));

  return;
}

void DestroyTokTree(tree t) {
  if (t == NULL) {
	return;
  }
  tree l = t->left;
  tree r = t->right;
  free(t);
  t = NULL;
  DestroyTokTree(l);
  DestroyTokTree(r);
}

