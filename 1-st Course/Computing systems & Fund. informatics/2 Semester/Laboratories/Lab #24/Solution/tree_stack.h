#ifndef _TREE_STACK_H
#define _TREE_STACK_H
#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>
#include "main.h"
#include "tree.h"

typedef struct {
  int size;
  int cap;
  tree *buf;
} stack_dbl;

void tree_stok_create(stack_dbl *s);

void tree_stok_destroy(stack_dbl *s);

bool tree_stok_is_empty(stack_dbl *s);

bool tree_stok_push(stack_dbl *s, tree el);

tree tree_stok_pop(stack_dbl *s);

#endif

