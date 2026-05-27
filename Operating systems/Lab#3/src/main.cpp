#include <bits/stdc++.h>

pthread_mutex_t mutex; // Переменная для Mutex

// структура аргументов потока
struct threadArgs {
  std::string mainString;
  std::string pattern;
  int index;
  size_t size;
};

// Функция наивного поиска подстроки в строке
void NativeSearch(void *inArgs) {
  pthread_mutex_lock(&mutex);
  threadArgs args = *(threadArgs *) inArgs; // приводим принятые аргументы к типу структуры аргументов
  std::string mainString = args.mainString;
  std::string pattern = args.pattern;
  int index = args.index; // Номер текущего потока
  size_t size = args.size;
  size_t pattern_len = pattern.size(); // Длина подстроки

  // index * size; index * size + size - начало и конец поиска для потока
  for (int i(index * (int) size); i < (index * (int) size + (int) size); ++i) {
	int j(0);
	for (; j < pattern_len; ++j) {
	  if (mainString[i + j] != pattern[j]) {
		break; // если какой-то символ подстроки и строки не совпал, то переходим к следующему i
	  }
	}

	// Если подстрока полностью совпала, то выводим индекс вхождения
	if (j == pattern_len) {
	  std::cout << "Thread [" << index << "] " << "Pattern found at position: " << i << std::endl;
	}
  }
  free(inArgs);
  pthread_mutex_unlock(&mutex); // открываем мьютекс для других потоков
}

int main(int argc, char **argv) {
  size_t inThreads = 1; // переменная для введенного количества потоков

  // Кол-во потоков не указано, используем по умолчанию 1
  if (argc != 2) {
	std::cerr << "Incorrect num of arguments, expected thread count" << std::endl;
	std::cerr << "Setting thread count to 1" << std::endl;
  } else {
	inThreads = std::stoi(argv[1]); // преобразуем строку в число
  }

  // Основная строка
  std::string mainString;
  // Подстрока
  std::string pattern;
  std::cout << "Input main string: ";
  std::cin >> mainString;
  std::cout << "Input a pattern string to search in main string: ";
  std::cin >> pattern;

  // Кол-во потоков и размер области поиска для каждого
  size_t threadCount = inThreads, size;
  // Если кол-во потоков меньше кол-ва итераций по строке (n - m)
  if (inThreads <= mainString.size() - pattern.size() + 1) {
	size = mainString.size() / inThreads;
  } else {
	// Каждый поток обрабатывает ровно одну итерацию по строке
	threadCount = mainString.size() - pattern.size() + 1;
	// 1 поток - 1 вхождение
	size = 1;
  }

  pthread_t threads[threadCount];
  int i(0);
  pthread_mutex_init(&mutex, nullptr);
  for (; i < threadCount; i++) {
	pthread_mutex_lock(&mutex); // закрываем мьютекс
	auto *arguments = new threadArgs;
	arguments->mainString = mainString; // заполняем структуру данными
	arguments->pattern = pattern;
	arguments->index = i;
	arguments->size = size;
	// запускаем поток, кастуем функцию к общему указателю, который возвращает и принимает общий указатель (pretty!)
	if (pthread_create(&threads[i], nullptr, reinterpret_cast<void *(*)(void *)>(&NativeSearch), arguments) != 0) {
	  std::cerr << "Creating thread error" << std::endl;
	}
	pthread_mutex_unlock(&mutex);
  }

  for (i = 0; i < threadCount; i++) {
	if (pthread_join(threads[i], nullptr) != 0) {
	  // тут мы ждём, когда потоки завершатся. Если кто-то вернул код != 0 - выводим ошибку
	  std::cerr << "Thread " << i << " work confused" << std::endl;
	}
  }
  pthread_mutex_destroy(&mutex);
  return 0;
}