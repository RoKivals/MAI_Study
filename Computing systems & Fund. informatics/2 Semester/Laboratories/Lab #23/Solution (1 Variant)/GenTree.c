#include <stdio.h>
#include <stdlib.h>

#include "GenTree.h"



Tree *root_create(int value)
{
  Tree *tree = (Tree*)malloc(sizeof(Tree));
  tree->data = value;
  tree->left = NULL;
  tree->right = NULL;
  tree->parent = NULL;
  return tree;
}

Tree *TreeAddElement(Tree *parent, int value)
{
  if (parent == NULL) {
	printf("Done!\n");
	return root_create(value);
  }

  Tree *NewTree = (Tree*)malloc(sizeof(Tree));
  NewTree->data = value;
  Tree *tree1 = parent;
  Tree *tree2 = NULL;

  while (tree1 != NULL) {
	tree2 = tree1;
	if (value < tree1->data) {
	  tree1 = tree1->left;
	}
	else if (value > tree1->data) {
	  tree1 = tree1->right;
	}
	else {
	  printf("This value is already exist in tree! Use other values!\n");
	  return parent;
	}
  }
  NewTree->parent = tree2;
  NewTree->left = NULL;
  NewTree->right = NULL;
  if (value < tree2->data) {
	tree2->left = NewTree;
  } else {
	tree2->right = NewTree;
  }
  printf("Done!\n");
  return parent;
}

void TreePrint(Tree *t, int n)
{
  if (t != NULL) {
	TreePrint(t->right, n + 1);
	for (int i = 0; i < n; i++) printf("\t");
	printf("%d\n", t->data);
	TreePrint(t->left, n + 1);
  }
}

Tree *DeleteElement(Tree *parent, int value)
{
  Tree* tree1 = NULL, * tree2 = NULL, *tree3 = parent, *tree4 = NULL;

  if (parent == NULL) {
	printf("Delete error! Your tree isn't exist!\n");
	return parent;
  }
  tree1 = SearchInTree(tree3, value);
  if (tree1 == NULL) {
	printf("Your element already isn't exist!\n");
	return parent;
  }
  // First case. element == leaf
  if (tree1->left == NULL && tree1->right == NULL) {
	if (tree1->parent == NULL) { // If this is root
	  free(tree1);
	  tree1 = NULL;
	  printf("Successful deletion\n");
	  return NULL;
	}
	tree2 = tree1->parent;
	if (tree2->left == tree1) {
	  tree2->left = NULL;
	}
	else {
	  tree2->right = NULL;
	}
	free(tree1);
  }
	// Second case. one of subtrees are available
  else if (tree1->left != NULL && tree1->right == NULL) { // left subtrees is exist
	if (tree1->parent == NULL) { // If this is root
	  tree4 = tree1->left;
	  tree4->parent = NULL;
	  free(tree1);
	  printf("Successful deletion\n");
	  return tree4;
	}
	tree2 = tree1->parent;
	if (tree2->left == tree1) {
	  tree2->left = tree1->left;
	}
	else {
	  tree2->right = tree1->left;
	}
	free(tree1);
  }
  else if (tree1->left == NULL && tree1->right != NULL) { // right subtrees is exist
	if (tree1->parent == NULL) { // If this is root
	  tree4 = tree1->right;
	  tree4->parent = NULL;
	  free(tree1);
	  printf("Successful deletion\n");
	  return tree4;
	}
	tree2 = tree1->parent;
	if (tree2->left == tree1) {
	  tree2->left = tree1->right;
	}
	else {
	  tree2->right = tree1->right;
	}
	free(tree1);
  }
	// Third case. Both subtrees are available
  else if (tree1->left != NULL && tree1->right != NULL) {
	tree2 = Minimum(tree1->right);
	tree1->data = tree2->data;
	tree4 = tree2->parent;
	if (tree4->left == tree2) {
	  free(tree2);
	  tree4->left = NULL;
	}
	if (tree4->right == tree2) {
	  free(tree2);
	  tree4->right = NULL;
	}
  }
  printf("Successful deletion\n");
  return parent;
}

Tree *SearchInTree(Tree *t, int value)
{
  if (t == NULL) {
	return NULL;
  }
  if (t->data == value) {
	return t;
  }
  if (value > t->data) {
	return SearchInTree(t->right, value);
  } else {
	return SearchInTree(t->left, value);
  }
}

Tree *Minimum(Tree *t)
{
  if (t->left == NULL) {
	return t;
  }
  return Minimum(t->left);
}

Tree *Maximum(Tree *t) {
  if (t->right == NULL) {
	return t;
  }
  return Maximum(t->right);
}

int max_way(Tree *t, int len) {
  len++;
  if (t->right == NULL) {
	return len;
  }
  return max_way(t->right,len);
}

int min_way(Tree *t, int len)
{
  len++;
  if (t->left == NULL) {
	return len;
  }
  return min_way(t->left,len);
}

int IsAvl(Tree *t)
{
  if (abs(min_way(t, 0) - max_way(t, 0)) > 1)
	return 0;
  if (t->left != NULL)
	IsAvl(t->left);
  if (t->right != NULL)
	IsAvl(t->right);
  return 1;
}