#ifndef _TREE_H
#define _TREE_H

#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>
#include "main.h"
#include "queue.h"

typedef struct node node;
struct node {
  token data;
  struct node *left;
  struct node *right;
};

typedef struct node *tree;

bool IsBinary(token a);

tree AddList(token a);

tree AddBinOp(token op, tree left, tree right);

tree AddUnoOp(token op, tree right);

tree BuildTokTree(queue *q);

void TreeReadToken(token *tok);

void SimplePrint(tree t);

void PrintTokTreeDfs(tree t, int h);

void PrintTokTree(tree t, int level, bool isleft);

int TreeDepth(tree t);

void TokDepth(tree t);

void DestroyTokTree(tree t);

#endif

