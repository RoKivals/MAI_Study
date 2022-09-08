#ifndef VECTOR_H
#define VECTOR_H

#include <stdlib.h>

// Структура вектора на integer
typedef struct _Vector {
  int *_data;
  int _size;
  int _capacity;
} Vector;

Vector *VectorCreate(const int size);
int VectorEmpty(const Vector *v);
int VectorSize(const Vector *v);
int VectorCapacity(const Vector *v);
int VectorIndex(const Vector *v, const int index);
int VectorPushBack(Vector *v, const int value);
void VectorResize(Vector *v, const int size);
int VectorEqual(const Vector *v1, const Vector *v2);
void VectorDestroy(Vector *v);
void VectorPrint(Vector *v, const int size);

#endif
