#include "duque.h"
#include <stdlib.h>

// Создаём дек
void deque_create(deque *a)
{
  a->elements = malloc(sizeof(int));
  a->capacity = 1;
  a->number_of_elements = 0;
}

// Добавить элемент в начало дека
void push_front(deque *a, int elem)
{
  if(a->number_of_elements == a->capacity) {
	resize(a);
  }
  int tmp1, tmp2;
  tmp1 = elem;
  for(int i = 0; i < a->number_of_elements + 1; i++) {
	tmp2 = a->elements[i];
	a->elements[i] = tmp1;
	tmp1 = tmp2;
  }
  a->number_of_elements++;
}

// Вставить элемент в конец дека
void push_back(deque *a, int elem)
{
  if (a->number_of_elements == deque_size(a)) {
	resize(a);
  }
  a->elements[a->number_of_elements] = elem;
  a->number_of_elements++;
}

// Удалить элемент с конца
void pop_back(deque *a)
{
  if (a->number_of_elements > 0) {
	a->number_of_elements--;
  }
}

// Удалить элемент с начала
void pop_front(deque *a)
{
  if (a->number_of_elements > 0) {
	for(int i = 0; i < a->number_of_elements - 1; i++) {
	  a->elements[i] = a->elements[i + 1];
	}
	a->number_of_elements--;
  }
}

// Вывести первые элемент спереди
int first_front(deque* a)
{
  return a->elements[0];
}

// Вывести первые элемент сзади
int first_back(deque* a)
{
  return a->elements[a->number_of_elements - 1];
}

// Проверить, является ли дек пустым
int empty(deque *a)
{
  if (a->number_of_elements == 0) {
	return 1;
  } else {
	return 0;
  }
}

// Изменение размера дека (перераспределение памяти)
void resize(deque *a)
{
  a->capacity++;
  a->elements = realloc(a->elements, a->capacity * sizeof(int));
}

// Вернуть размер дека
int size(deque* a)
{
  return a->number_of_elements;
}

// Вернуть размер выделенной памяти в деке (бесполезно, но вдруг)
int deque_size(deque *a)
{
  return a->capacity;
}

// Перевернуть дек наоборот
deque* reverse(deque* a)
{
  deque* b;
  deque_create(b);
  while (!empty(a)) {
	push_front(b, first_front(a));
	pop_front(a);
  }
  return b;
}

