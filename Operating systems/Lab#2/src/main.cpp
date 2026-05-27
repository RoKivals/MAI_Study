#include <bits/stdc++.h>
#include <unistd.h>
#include <fcntl.h>
#include <signal.h>
#include <sys/wait.h>

//#define READ 0
//#define WRITE 1


using namespace std;

void wrt_fd(int *fd1, int *fd2, string name_file) {
  ifstream file1(name_file);
  string line;
  if (file1.is_open()) {
	while (getline(file1, line)) {
	  line = line + "\n";
	  int lineSize = line.length();
	  if (lineSize > 10) {
		if (write(fd2[1], line.c_str(), lineSize * sizeof(char)) == -1) {
		  std::perror("Pipe2");
		  return;
		}
	  } else {
		if (write(fd1[1], line.c_str(), lineSize * sizeof(char)) == -1) {
		  std::perror("Pipe1");
		  return;
		}
	  }
	}
  }
  file1.close();
}

int main(int argc, char *argv[]) {
  // Массивы указывающие на концы каналов
  int fd1[2];
  int fd2[2];

  // Создаём 2 пайпа
  if (pipe(fd1) == -1) {
	std::perror("pipe1");
	return 1;
  }

  if (pipe(fd2) == -1) {
	std::perror("pipe2");
	return 2;
  }

  // Создаём 1-ый дочерний процесс
  int id = fork(), id2;
  int flag = 0;

  // Если мы в родительском процессе, то создаём второго ребёнка
  if (id > 0) {
	id2 = fork();
	flag = 1;
  }
  if (id == -1 || id2 == -1) {
	std::perror("fork");
	return 5;
  } else if (id2 != 0 && id != 0) {
	// Закрываем каналы для чтения
	close(fd1[0]);
	close(fd2[0]);
	wrt_fd(fd1, fd2, "./file1.txt");
	wrt_fd(fd1, fd2, "./file2.txt");
	close(fd1[1]);
	close(fd2[1]);
  } else if (flag) {
	close(fd1[1]);
	close(fd1[1]);
	close(fd2[0]);
	dup2(fd1[0], STDIN_FILENO);
	execlp("./slave1", "slave1", NULL);
	close(fd1[0]);
  } else {
	close(fd2[1]);
	close(fd1[0]);
	close(fd1[1]);
	dup2(fd2[0], STDIN_FILENO);
	execlp("./slave2", "slave2", NULL);
	close(fd2[0]);
  }
  return 0;
}