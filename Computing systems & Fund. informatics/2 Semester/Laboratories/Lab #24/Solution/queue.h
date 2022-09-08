#ifndef QUEUE_H
#define QUEUE_H

#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>
#include "main.h"

typedef struct {
  int size;
  int first;
  int capacity;
  token *buf;
} queue;

void QCreate(queue *q);

void QDestroy(queue *q);

bool QIsEmpty(queue *q);

bool QPushBack(queue *q, token val);

token QPopFront(queue *q);

#endif

