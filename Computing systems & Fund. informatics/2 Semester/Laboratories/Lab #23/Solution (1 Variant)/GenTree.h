#ifndef _TREE_H_
#define _TREE_H_

#include <stdio.h>
#include <stdlib.h>

typedef struct _tree Tree;

typedef struct _tree {
  int data;
  struct Tree *left;
  struct Tree *right;
  struct Tree* parent;
} Tree;

Tree *TreeAddElement(Tree *parent, int value);
Tree *DeleteElement(Tree *parent, int value);
void TreePrint(Tree *t, int n);
int IsAvl(Tree *t);
Tree* Minimum(Tree *t);
Tree* Maximum(Tree *t);
Tree* SearchInTree(Tree *t, int value);


#endif //_TREE_H_

