#include <stdio.h>
#include <stdlib.h>
#include "list.h"
#include "iterator.h"

// Создание списка
list *ListCreate() {
  list * lst = (list *)malloc(sizeof(list));
  lst->head = (list_node *)malloc(sizeof(list_node));
  lst->head->next = NULL;
  lst->head->data = ' ';
  return lst;
}

// Вывод списка, барьер в виде NULL
void ListPrint(list *l) {
  listIterator *it = IteratorCreate(l);
  if (it->node) {
	while (it->node != NULL) {
	  if (it->node->data != ' ') printf("%c --> ", it->node->data);
	  it->node = it->node->next;
	}
	printf("NULL");
	printf("\n");
  }
}

// Добавление элемента в список
void ListInsert(list *lst, item data) {
  listIterator *it = IteratorCreate(lst);
  if (it->node) {
	while (it->node->next) // next item is NULL!
	{
	  IteratorNext(it);
	}
	list_node *tail = (list_node *)malloc(sizeof(list_node));
	tail->next = NULL;
	tail->data = ' ';
	it->node->data = data;
	it->node->next = tail;
  } else printf("The list doesn't exist!\n");
  free(it);
}

// Удаление всего списка
void ListRemove(list *lst, item data) {
  int flag = 0;
  listIterator *it = IteratorCreate(lst);
  list_node *prew;
  if (it->node) {
	if (it->node->data == data) {  // case, then out node is head
	  lst->head = it->node->next;
	  free(it->node);
	  it->node = NULL;
	} else {
	  while (it->node->next) {
		if (it->node->next->data == data) // case, then our node between two elements or is tail
		{
		  prew = it->node;
		  IteratorNext(it);
		  flag = 1;
		  if (it->node->next != NULL) {
			prew->next = it->node->next;
		  }
		  free(it->node);
		  it->node = NULL;
		  break;
		}
		IteratorNext(it);
	  }
	  if (!flag) printf("There is no such element\n");
	}
  } else printf("The list doesn't exist!\n");
  free(it);
}

// Длина списка
int ListLen(list *lst) {
  int count = 0;
  listIterator *it = IteratorCreate(lst);
  IteratorSet(lst->head, it);
  if (it->node) {
	while (it->node->next && it->node->next->data != ' ') {
	  count++;
	  IteratorNext(it);
	}
  } else printf("The list doesn't exist!\n");
  free(it);
  count++;
  return count;
}

// Переворот списка (нестандартное действие)
void ListReverse(list_node *lstFromHead, list_node *lstFromTail, int i, list *lst, int len) {
  if (i - 1 == len / 2) return;
  list_node *lstFromHeadNext = lstFromHead->next;
  list_node *lstFromTailPrew; // prew from lstFromTail
  list_node *lstFromHeadPrew; // prew from lstFromHead
  listIterator *it = IteratorCreate(lst);
  IteratorSet(lstFromHead, it);
  for (int j = 0; j < len - 2 * i; j++) {
	IteratorNext(it);
  }
  lstFromTailPrew = it->node;
  IteratorSet(lst->head, it);
  for (int j = 0; j < i - 2; j++) {
	IteratorNext(it);
  }
  lstFromHeadPrew = it->node;
  if (i == 1) {
	lst->head = lstFromTail;
	lstFromTail->next = lstFromHeadNext;
	lstFromHead->next = NULL;
	lstFromTailPrew->next = lstFromHead;
  } else {
	lstFromTailPrew->next = lstFromHead;
	lstFromHead->next = lstFromTail->next;
	if (2 * i != len) {
	  lstFromTail->next = lstFromHeadNext;
	} else {
	  lstFromTail->next = lstFromHead;
	}
	lstFromHeadPrew->next = lstFromTail;
  }
  i++;
  free(it);
  ListReverse(lstFromHeadNext, lstFromTailPrew, i, lst, len);
}

