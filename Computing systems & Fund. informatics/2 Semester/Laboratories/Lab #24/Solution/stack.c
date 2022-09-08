#include "stack.h"

// Создаём стек
void SCreate(stack *s){
	s->size = 0;
	s->cap = 0;
	s->buf = NULL;
}

// Создаём стек
void SDestroy(stack *s){
	s->size = 0;
	s->cap = 0;
	free(s->buf);
}

// Пустой ли стек
bool SIsEmpty(stack *s){
	return s->size == 0;
}

// Размер стека
int SSize(stack *s){
	return s->size;
}

// Увеличиваем буфер
bool SGrowBuf(stack *s){
	int new_cap = s->cap * (3/2);
	if (s->cap == 0){
		new_cap = 10;
	}
	token *n = realloc(s->buf, sizeof(token) * (new_cap));
	if (n != NULL){
		s->buf = n;
		s->cap = new_cap;
		return true;
	}
	else{
		return false;
	}
}

// Добавить элемент в стек
bool SPush(stack *s, token el){
    if (s->size >= s->cap){
        if (!SGrowBuf(s)){
            return false;
        }
    }
    s->buf[s->size] = el;
    s->size++;
    return true;
}

// Уменьшаем буфер
bool SShrinkBuf(stack *s){
	int new_cap = s->cap * (2/3);
	token *n = realloc(s->buf, sizeof(token) * (new_cap));
	if (n != NULL){
        	s->buf = n;
        	s->cap = new_cap;
        	return true;
    	}
    	else{
        	return false;
    }
}

// Удаляем элемент из стека
token SPop(stack *s){
	if (s->size<=(4/9) * s->cap){
		SShrinkBuf(s);
	}
	token temp;
	temp = s->buf[s->size - 1];
	s->size--;
	return temp;
}

