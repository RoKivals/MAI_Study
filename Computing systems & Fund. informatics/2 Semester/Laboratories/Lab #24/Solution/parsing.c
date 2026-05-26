#include "parsing.h"

#include <stdio.h>
#include <stdbool.h>
#include <stdlib.h>
#include <string.h>

bool WordChar(int ch) { //  Валидность символа
  if (('A' <= ch && ch <= 'Z') || ('a' <= ch && ch <= 'z') || (ch == '_') || ('0' <= ch && ch <= '9')) {
	return true;
  } else {
	return false;
  }
}

void SReverse(stack *s) {
  token a;
  queue per;
  QCreate(&per);

  while (!SIsEmpty(s)) {
	a = SPop(s);
	QPushBack(&per, a);
  }

  while (!QIsEmpty(&per)) {
	a = QPopFront(&per);
	SPush(s, a);
  }
  QDestroy(&per);
}

int SwitchOperatorPrior(char op) {
  char sum = '+';
  char diff = '-';
  char umn = '*';
  char del = '/';
  char step = '^';
  char uno = '~';

  if (op == sum) {
	return SUM;
  } else if (op == diff) {
	return RAZN;
  } else if (op == umn) {
	return UMN;
  } else if (op == del) {
	return DEL;
  } else if (op == step) {
	return STEP;
  } else if (op == uno) {
	return UNO;
  }
  return 999;
}

read_tok_res read_token(token *out, tok_type prev) {
  int ch;
  ch = getc(stdin);

  if (ch == ' ' || ch == '\t') {
	//printf("' '  't'\n\n");////////////////////
	return SPASE;
  } else if (ch == EOF) {
	//printf("EOF\n\n");////////////////////
	return READ_TOK_EOF;
  } else if (ch == '\n') {
	//printf("'enter'\n\n");////////////////////
	return READ_TOK_EOE;
  } else if (ch == '(') {
	if (prev == TOK_CONST || prev == TOK_VAR || prev == TOK_RBR) {
	  printf("Проверьте символ перед открывающей скобкой '(' !\n");
	  return READ_TOK_UNEXPECTED_TOKEN;
	}

	out->type = TOK_LBR;
	strcpy(out->var_name, "\0");
	out->const_val = 0;
	out->op = '\0';
	//printf("ch == ( \n\n");////////////////////
	return READ_TOK_SUCCESS;
  } else if (ch == ')') {
	if (prev == TOK_OP || prev == TOK_LBR || prev == FIRST) {
	  printf("Проверьте символ перед закрывающей скобкой ')' !\n");
	  return READ_TOK_UNEXPECTED_TOKEN;
	}

	out->type = TOK_RBR;
	strcpy(out->var_name, "\0");
	out->const_val = 0;
	out->op = '\0';
	//printf("ch == ) \n\n");////////////////////
	return READ_TOK_SUCCESS;
  } else if (ch == '-') {
	out->type = TOK_OP;
	strcpy(out->var_name, "\0");
	out->const_val = 0;

	if (prev == FIRST || prev == TOK_OP || prev == TOK_LBR) {
	  out->op = '~';
	  //printf(" ~ \n\n");////////////////////
	} else {
	  out->op = '-';
	  //printf(" - \n\n");////////////////////
	}
	return READ_TOK_SUCCESS;
  } else if (ch == '+' || ch == '*' || ch == '/' || ch == '^') {
	if (prev == TOK_OP || prev == TOK_LBR || prev == FIRST) {
	  printf("Проверьте символ перед знаками '+' '*' '/' '^'!\n");
	  return READ_TOK_UNEXPECTED_TOKEN;
	}
	out->type = TOK_OP;
	strcpy(out->var_name, "\0");
	out->const_val = 0;
	out->op = ch;

	//printf("OP \n\n");////////////////////
	return READ_TOK_SUCCESS;
  } else if (('A' <= ch && ch <= 'Z') || ('a' <= ch && ch <= 'z') || ch == '_') {
	if (prev == TOK_CONST || prev == TOK_VAR || prev == TOK_RBR) {
	  printf("Проверьте символ перед переменной!\n");
	  return READ_TOK_UNEXPECTED_TOKEN;
	}

	strcpy(out->var_name, "\0");
	out->var_name[0] = ch;
	char c;
	int i = 1;

	while (WordChar(c = getchar())) {
	  out->var_name[i] = c;
	  i++;
	  if (i > 20) {
		printf("Некорректное имя переменной!\n");
		return READ_TOK_INVALID_CHAR;
	  }
	}
	ungetc(c, stdin);
	out->type = TOK_VAR;
	out->const_val = 0;
	out->op = '\0';

	//printf("ch == var \n\n");////////////////////
	return READ_TOK_SUCCESS;
  } else if ('0' <= ch && ch <= '9') {
	if (prev == TOK_CONST || prev == TOK_VAR || prev == TOK_RBR) {
	  printf("Проверьте символ перед константой!\n");
	  return READ_TOK_UNEXPECTED_TOKEN;
	}

	double a;
	ungetc(ch, stdin);
	scanf("%lf", &a);

	out->type = TOK_CONST;
	strcpy(out->var_name, "\0");
	out->const_val = a;
	out->op = '\0';

	//printf("%.2lf \n\n", a);////////////////////
	return READ_TOK_SUCCESS;
  }

  return READ_TOK_INVALID_CHAR;
}

// Добавляем токен
bool push_token(stack *s, queue *q, token a) {
  token per;
  if (a.type == TOK_VAR) {
	QPushBack(q, a);
  } else if (a.type == TOK_CONST) {
	QPushBack(q, a);
  } else if (a.type == TOK_LBR) {
	SPush(s, a);
  } else if (a.type == TOK_RBR) {
	bool lbr_is_in_stack = false;
	stack s1;
	SCreate(&s1);

	per = SPop(s);
	while (!SIsEmpty(s)) {
	  if (per.type != TOK_LBR) {
		SPush(&s1, per);
		per = SPop(s);
	  } else {
		lbr_is_in_stack = true;
		break;
	  }
	}

	if (!lbr_is_in_stack) {
	  return false;
	}

	stok_reverse(&s1);

	while (!SIsEmpty(&s1)) {
	  per = SPop(&s1);
	  QPushBack(q, per);
	}
  } else if (a.type == TOK_OP) {
	while (!SIsEmpty(s)) {
	  if (SwitchOperatorPrior(a.op) == UNO) { //|| switch_op_prior(a.op) == STEP){
		per = SPop(s);
		if (SwitchOperatorPrior(a.op) >= SwitchOperatorPrior(per.op)) {
		  QPushBack(q, per);
		} else {
		  SPush(s, per);
		  break;
		}
	  } else {
		per = SPop(s);
		if (switch_op_prior(a.op) > switch_op_prior(per.op)) {
		  QPushBack(q, per);
		} else {
		  SPush(s, per);
		  break;
		}
	  }
	}
	SPush(s, a);
  }
  return true;
}

res_read_expression read_expression(queue *q) {
  stack s;
  SCreate(&s);

  tok_type prev = FIRST;
  read_tok_res status;
  while (status != READ_TOK_EOF && status != READ_TOK_EOE) {
	token a;
	status = read_token(&a, prev);
	if (status == READ_TOK_SUCCESS) {
	  if (push_token(&s, q, a)) {
		prev = a.type;
	  } else {
		return READ_EXPRESSION_UNEXPECTED_RBR;
	  }
	} else if (status == SPASE) {
	  continue;
	} else if (status == READ_TOK_INVALID_CHAR) {
	  return READ_EXPRESSION_INVALID_INPUT;
	} else if (status == READ_TOK_UNEXPECTED_TOKEN) {
	  return READ_EXPRESSION_UNEXPECTED_INPUT;
	}
  }
  if (prev == TOK_OP || SIsEmpty(&s)) {
	return READ_EXPRESSION_INVALID_INPUT;
  } else {
	while (!SIsEmpty(&s)) {
	  token a = SPop(&s);
	  if (a.type == TOK_LBR) {
		return READ_EXPRESSION_UNEXPECTED_LBR;
	  }
	  QPushBack(q, a);
	}
  }
  SDestroy(&s);
  return READ_EXPRESSION_SUCCESS;
}

