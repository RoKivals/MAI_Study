#include "duque.h"

// Слияние двух деков
void merge(deque* res, deque* a, deque* b)
{
  deque c;
  deque_create(&c);
  while (!empty(a) && !empty(b)) {
	if (first_front(a) < first_front(b)) {
	  push_back(&c, first_front(a));
	  pop_front(a);
	} else {
	  push_back(&c, first_front(b));
	  pop_front(b);
	}
  }
  while (!empty(a)) {
	push_back(&c, first_front(a));
	pop_front(a);
  }
  while (!empty(b)) {
	push_back(&c, first_front(b));
	pop_front(b);
  }

  while (!empty(&c)) {
	push_front(res, first_back(&c));
	pop_back(&c);
  }
}

// Сортировка слиянием
void merge_sort(deque* a) {
  if (size(a) > 1) {
	deque b, c;
	deque_create(&b);
	deque_create(&c);
	while (!empty(a)) {
	  if (size(a) % 2 == 0) {
		push_front(&b, first_front(a));
		pop_front(a);
	  } else {
		push_front(&c, first_front(a));
		pop_front(a);
	  }
	}
	merge_sort(&b);
	merge_sort(&c);
	merge(a, &b, &c);
  }
}