#include <bits/stdc++.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/mman.h>
#include <signal.h>
#include "sys/types.h"
#include "errno.h"
#include <sys/stat.h>
#include <semaphore.h>
using namespace std;

// Имена для разделяемой памяти
string backFile1 = "main1.back";
string backFile2 = "main2.back";

/*
 * S_IWUSR == write access
 * S_IRUSR == read access
 * S_IRGRP == read access for group
 * S_IROTH == read access for all
 * */
int accessPerm = S_IWUSR | S_IRUSR | S_IRGRP | S_IROTH;

int create_memory(string name, int flag, int permitions);

void wrt_fd(string &ans1, string &ans2, string name_file) {
  // Открываем поток на чтение
  ifstream file1(name_file);
  string line;
  // Если файл нормально подключился к потокуы
  if (file1.is_open()) {
	while (getline(file1, line)) {
	  line = line + "\n";
	  int lineSize = line.length();
	  if (lineSize > 10) {
		ans1 += line;
	  } else {
		ans2 += line;
	  }
	}
  }
  file1.close();
}

// Заготовка для создания + проверки fork (не используется)
int create_fork_and_check() {
  int result = fork();
  if (result == -1) {
	std::perror("fork");
	exit(EXIT_FAILURE);
  }
  return result;
}

int main(int argc, char *argv[]) {
  // Создаём 1-ый дочерний процесс
  int id = fork(), id2;
  int flag = 0;
  // Если в родительском процессе, то создаём 2-ой дочерний
  if (id > 0) {
	id2 = fork();
	flag = 1;
  }

  if (id == -1 || id2 == -1) {
	std::perror("fork");
	exit(EXIT_FAILURE);
  } else if (id2 != 0 && id != 0) { // Если находимся в родителе
	string ans1, ans2;
	wrt_fd(ans1, ans2, "./file1.txt");
	wrt_fd(ans1, ans2, "./file2.txt");

	// Получаем размеры двух файлов
	int mapSize1 = ans1.size(), mapSize2 = ans2.size();
	// Создаём объект разделяемой памяти POSIX
	// O_RDRW == read-write access; O_CREAT == create memory object
	int fd1 = shm_open(backFile1.c_str(), O_RDWR | O_CREAT, accessPerm);
	// Устанавливаемый для файла (памяти) необходимый размер
	ftruncate(fd1, mapSize1);

	int fd2 = shm_open(backFile2.c_str(), O_RDWR | O_CREAT, accessPerm);
	ftruncate(fd2, mapSize2);

	// Проверяем ошибки создания памяти
	if (fd1 == -1) {
	  perror("shm open 1:");
	  exit(EXIT_FAILURE);
	}
	if (fd2 == -1) {
	  perror("shm open 2:");
	  exit(EXIT_FAILURE);
	}


	// mmap(start, len, prot, flags, fd, offset=0) - отражает файл в память (ОЗУ)
	/* PROT_READ == may be read
	 * PROT_WRITE == may be written
	 * MAP_SHARED == let other processes to get info about updating map
	 * */
	char *mapped1 = (char *) mmap(NULL, mapSize1, PROT_READ | PROT_WRITE, MAP_SHARED, fd1, 0);
	char *mapped2 = (char *) mmap(NULL, mapSize2, PROT_READ | PROT_WRITE, MAP_SHARED, fd2, 0);
	if (mapped1 == MAP_FAILED || mapped2 == MAP_FAILED) {
	  perror("MMAP");
	  exit(EXIT_FAILURE);
	}

	// Заполняем выделенную память
	memset(mapped1, '\0', mapSize1);
	for (int i = 0; i < mapSize1; ++i) {
	  mapped1[i] = ans1[i];
	}
	memset(mapped2, '\0', mapSize2);
	for (int i = 0; i < mapSize2; ++i) {
	  mapped2[i] = ans2[i];
	}
  } else if (flag) { // Один из потомков
	// Создаём в нём указатель на тот же раздел памяти
	int fd1 = shm_open(backFile1.c_str(), O_RDWR, accessPerm);
	if (fd1 == -1) {
	  perror("shm open 1 by child");
	  exit(EXIT_FAILURE);
	}

	struct stat statBuf1;
	fstat(fd1, &statBuf1);
	int mapSize1 = statBuf1.st_size;
	char *mapped1 = (char *) mmap(NULL, mapSize1, PROT_READ, MAP_SHARED, fd1, 0);
	if (mapped1 == MAP_FAILED) {
	  perror("mmap1_child");
	  exit(EXIT_FAILURE);
	}

	// null-vector аргументов
	char *argv1[] = {NULL, NULL};
	// Выделяем память для нашей строки (аргумента)
	argv1[0] = (char *) malloc(mapSize1);
	// Копируем строку в аргумент
	strcpy(argv1[0], mapped1);
	execv("child1", argv1);
  } else {

	int fd2 = shm_open(backFile2.c_str(), O_RDWR, accessPerm);
	if (fd2 == -1) {
	  perror("shm open 2 by child");
	  exit(EXIT_FAILURE);
	}

	struct stat statBuf2;
	fstat(fd2, &statBuf2);
	int mapSize2 = statBuf2.st_size; // полный размер в байтах
	char *mapped2 = (char *) mmap(NULL, mapSize2, PROT_READ, MAP_SHARED, fd2, 0);
	if (mapped2 == MAP_FAILED) {
	  perror("mmap2_child");
	  exit(EXIT_FAILURE);
	}

	TODO:
	"Переписать под один массив argv";
	// char *argv2[] = {nullptr, nullptr};
	char *argv2[] = {(char *) "child2", nullptr, nullptr};
	argv2[0] = (char *) malloc(mapSize2);
	strcpy(argv2[0], mapped2);
	execv("child2", argv2);
  }
  return 0;
}