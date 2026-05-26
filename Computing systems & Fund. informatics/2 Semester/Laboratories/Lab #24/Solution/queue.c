#include "queue.h"

// Создаём очередь
void QCreate(queue *q) {
	q->size = 0;
	q->capacity = 0;
	q->buf = NULL;
	q->first = 0;
}

// Удаляем очередь
void QDestroy(queue *q) {
	q->size = 0;
	q->capacity = 0;
	q->first = 0;
	free(q->buf);
}

// Проверяем пустая ли очередь
bool QIsEmpty(queue *q) {
	return q->size == 0;
}

// Увеличиваем буфер для очереди
bool QGrowBuf(queue *q) {
	int temp = q->capacity * 3/2;
	if (q->capacity == 0){
		temp = 3;
	}
	token *n = realloc(q->buf, sizeof(token) * temp);
	if (n == NULL)
		return false;
	q->buf = n;
	for (int i = (q->capacity - q->first); i > 0; i--) {
		q->buf[(temp - 1 + i - (q->capacity - q->first)) % temp] = q->buf[(q->first - 1 + i) % temp];
	}
	q->first = temp - (q->capacity - q->first);
	if (q->capacity == 0){
		q->first = 0;
	}
	q->capacity = temp;
	return true;
}

// Добавляем элемент в очередь
bool QPushBack(queue *q, token val) {
	if (q->size >= q->capacity) {
		if(!QGrowBuf(q))
			return false;
	}
	q->buf[(q->size + q->first) % q->capacity] = val;
	q->size++;
	return true;
}

// Убираем элемент из очереди
token QPopFront(queue *q) {
	token tmp = q->buf[q->first];
	q->first = (q->first +1) % q->capacity;
	q->size--;
	return tmp;
}

