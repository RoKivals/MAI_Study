#ifndef DEQUE
#define DEQUE

#include <stdio.h>

typedef struct _deque deque;

struct _deque
{
  int *elements;
  int capacity;
  int number_of_elements;
};

void deque_create(deque* a);
void push_front(deque* a, int elem);
void push_back(deque* a, int elem);
int first_front(deque* a);
int first_back(deque* a);
int empty(deque* a);
void pop_front(deque* a);
void pop_back(deque* a);
void resize(deque* a);
int size(deque* a);
int deque_size(deque* a);

deque* reverse(deque* a);
#endif
