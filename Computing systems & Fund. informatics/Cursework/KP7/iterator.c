#include <stdlib.h>
#include <stdio.h>
#include "iterator.h"

// Создаём итератор от головы списка
listIterator *IteratorCreate(list *lst) {
  if (lst != NULL) {
	listIterator *it = (listIterator *)malloc(sizeof(listIterator));
	it->node = lst->head;
	return it;
  } else return NULL;
}

// Переводим итератор на след элемент
void IteratorNext(listIterator *it) {
  if (it->node == NULL) {
	printf("You have met a barrier element! The list is finished\n");
  } else {
	it->node = it->node->next;
  }
}

// Возвращаем итератор
list_node *IteratorGet(listIterator *it) {
  return it->node;
}

// Устанавливаем новую связь для итератора
void IteratorSet(list_node *l, listIterator *it) {
  it->node = l;
}

