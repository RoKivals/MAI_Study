#ifndef _PARSING_H
#define _PARSING_H

#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>
#include "stack.h"
#include "queue.h"


bool WordChar(int ch);

void SReverse (stack *s);

int SwitchOperatorPrior(char op);

read_tok_res read_token (token *out, tok_type prev);

bool push_token(stack *s, queue *q, token a);

res_read_expression read_expression(queue *q);


#endif

