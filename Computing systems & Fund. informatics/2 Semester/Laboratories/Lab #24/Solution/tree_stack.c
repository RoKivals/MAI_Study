#include "tree_stack.h"

void tree_stok_create(stack_dbl *s) {
  s->size = 0;
  s->cap = 0;
  s->buf = NULL;
}

void tree_stok_destroy(stack_dbl *s) {
  s->size = 0;
  s->cap = 0;
  free(s->buf);
}

bool tree_stok_is_empty(stack_dbl *s) {
  return s->size == 0;
}

bool tree_stok_grow_buf(stack_dbl *s) {
  //printf("GROW\n");
  int new_cap = s->cap * (3 / 2);
  if (s->cap == 0) {
	new_cap = 10;
  }
  tree *n = realloc(s->buf, sizeof(tree) * (new_cap));
  if (n != NULL) {
	s->buf = n;
	s->cap = new_cap;
	return true;
  } else {
	return false;
  }
}

bool tree_stok_push(stack_dbl *s, tree el) {
  if (s->size >= s->cap) {
	if (!tree_stok_grow_buf(s)) {
	  return false;
	}
  }
  s->buf[s->size] = el;
  s->size++;
  return true;
}

bool tree_stok_shrink_buf(stack_dbl *s) {
  //printf("SHRINCK\n");
  int new_cap = s->cap * (2 / 3);
  tree *n = realloc(s->buf, sizeof(tree) * (new_cap));
  if (n != NULL) {
	s->buf = n;
	s->cap = new_cap;
	return true;
  } else {
	return false;
  }
}

tree tree_stok_pop(stack_dbl *s) {
  if (s->size <= (4 / 9) * s->cap) {
	if (!tree_stok_shrink_buf(s)) {
	  return false;
	}
  }
  tree temp;
  temp = s->buf[s->size - 1];
  s->size--;
  return temp;
}

