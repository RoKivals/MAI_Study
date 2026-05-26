#ifndef ITERATOR_H
#define ITERATOR_H

#include "list.h"

typedef struct _listIterator {
  list_node *node;
} listIterator;

listIterator *IteratorCreate(list *lst);
void IteratorNext(listIterator *it);
list_node *IteratorGet(listIterator *it);
void IteratorSet(list_node *l, listIterator *it);

#endif
