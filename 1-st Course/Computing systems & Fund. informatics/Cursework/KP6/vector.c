#include <stdio.h>
#include "vector.h"

// Создаём вектор
Vector *VectorCreate(const int size) {
  Vector *v;
  v = (Vector *)malloc(sizeof(Vector));
  if (size > 0) {
	v->_data = (int *)malloc(sizeof(int) * size);
	v->_capacity = size;
  } else {
	v->_data = (int *)malloc(sizeof(int));
	v->_capacity = 1;
  }
  v->_size = 0;
}

// Является ли вектор пустым
int VectorEmpty(const Vector *v) {
  return v->_size == 0;
}

// Размер вектора
int VectorSize(const Vector *v) {
  return v->_size;
}

int VectorCapacity(const Vector *v) {
  return v->_capacity;
}

//Получаем элемент вектора по индексу
int VectorIndex(const Vector *v, const int index) {
  return v->_data[index];
}

// Добавляем элемент к вектору с конца
int VectorPushBack(Vector *v, const int value) {
  int *ptr = NULL;
  if (v->_size == v->_capacity) {
	ptr = (int *)realloc(v->_data, sizeof(int) * v->_capacity * 2);
	if (ptr != NULL) {
	  v->_data = ptr;
	  v->_capacity *= 2;
	} else
	  return 0;
  }
  v->_data[v->_size++] = value;
  return 1;
}

// Изменяем размер вектора
void VectorResize(Vector *v, const int size) {
  int *ptr = NULL;
  if (size < 0)
	return;
  if (size == 0) {
	VectorDestroy(v);
	return;
  }
  ptr = (int *)realloc(v->_data, sizeof(int) * size);
  if (ptr != NULL) {
	v->_data = ptr;
	v->_size = size;
	v->_capacity = size;
  }
}

// Сравнение двух векторов
int VectorEqual(const Vector *v1, const Vector *v2) {
  int i;
  if (v1->_size != v2->_size)
	return 0;
  for (i = 0; i < v1->_size; i++)
	if (v1->_data[i] != v2->_data[i])
	  return 0;
  return 1;
}

// Уничтожение вектора
void VectorDestroy(Vector *v) {
  if (v->_data != NULL) {
	free(v->_data);

	v->_data = NULL;
  }
  v->_size = 0;
  v->_capacity = 0;
}

// Вывод вектора
void VectorPrint(Vector *v, const int size) {
  for (int i = 0; i < size; i++)
	printf("%d ", v->_data[i]);
  printf("\n");
}
