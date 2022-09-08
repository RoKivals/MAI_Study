#ifndef _MAIN_H
#define _MAIN_H
#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>

typedef enum {
  TOK_VAR,
  TOK_CONST,
  TOK_OP,
  TOK_LBR,
  TOK_RBR,
  FIRST,
} tok_type;

typedef struct {
  tok_type type;
  char var_name[20];
  double const_val;
  char op;
} token;

typedef enum {
  READ_TOK_SUCCESS = 0,
  READ_TOK_EOF = -1,
  READ_TOK_EOE = -2,
  READ_TOK_INVALID_CHAR = -3,
  READ_TOK_UNEXPECTED_TOKEN = -4,
  SPASE = -5,
} read_tok_res;

typedef enum {
  UNO = 0,
  STEP = 1,
  UMN = 2,
  DEL = 2,
  RAZN = 3,
  SUM = 3,
} op_prior;

typedef enum {
  READ_EXPRESSION_SUCCESS = 0,
  READ_EXPRESSION_INVALID_INPUT = -1,
  READ_EXPRESSION_UNEXPECTED_INPUT = -2,
  READ_EXPRESSION_UNEXPECTED_LBR = -3,
  READ_EXPRESSION_UNEXPECTED_RBR = -4
} res_read_expression;

#endif
