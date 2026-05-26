#ifndef STACK_H
#define STACK_H

#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>
#include "main.h"

typedef struct{
	int size;
	int cap;
	token *buf;
} stack;

void SCreate(stack *s);

void SDestroy(stack *s);

bool SIsEmpty(stack *s);

int SSize(stack *s);

bool SPush(stack *s, token el);

token SPop(stack *s);

#endif

















