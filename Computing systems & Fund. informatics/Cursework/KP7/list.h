#ifndef LIST_H
#define LIST_H

typedef char item;
typedef struct _list_node list_node;

// Структура, отвечающая за элемент списка
typedef struct _list_node {
  item data;
  list_node *next;
} list_node;

// Сам список, в котором есть указатель на голову (главный элемент)
typedef struct {
  list_node *head;
} list;

list *ListCreate();
void ListPrint(list *l);
void ListInsert(list *lst, item data);
void ListRemove(list *lst, item data);
int ListLen(list *lst);
void ListReverse(list_node *lstFromHead, list_node *lstFromTail, int i, list *lst, int len);

#endif
