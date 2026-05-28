#include <iostream>
#include <vector>
#include <string>

using namespace std;

// Оператор ввода для пары
template<class T, class Y>
std::istream &operator>>(std::istream &in, std::pair<T, Y> &p) {
  in >> p.first >> p.second;
  return in;
}

// Оператор вывода для пары
template<class T, class Y>
std::ostream &operator<<(std::ostream &out, std::pair<T, Y> &p) {
  out << p.first << "\t" << p.second;
  return out;
}

// GetKey - Функция, которая вытаскивает ключ из пары
template<class T>
void CountingSort(vector<T> &data) {
  if (data.empty()) return;

  uint16_t max_key = 0;
  for (size_t i(0); i < data.size(); ++i) {
    max_key = std::max(max_key, data[i].first);
  }

  // Создаём вектор для хранения кол-ва ключей с одинаковым индексом
  vector<size_t> count_keys(max_key + 1);
  for (size_t i(0); i < data.size(); ++i) {
    ++count_keys[data[i].first];
  }

  // Создаём массив, в котором каждый элемент указывает на последний подходящий для этого ключа индекс
  for (size_t i(1); i < count_keys.size(); ++i) {
    count_keys[i] += count_keys[i - 1];
  }

  // Результирующий вектор
  vector<T> res(data.size());
  // Размещаем элементы с конца для сохранения порядка ввода
  for (int64_t i = data.size() - 1; i >= 0; --i) {
    res[--count_keys[data[i].first]] = data[i];
  }

  for (size_t i(0); i < res.size(); ++i) {
    cout << res[i] << '\n';
  }
}

int main() {
  std::ios::sync_with_stdio(false);
  std::cin.tie(nullptr);

  vector<std::pair<uint16_t, string>> data;
  std::pair<uint16_t, string> temp;

  while (cin >> temp) {
    data.push_back(std::move(temp));
  }

  CountingSort<std::pair<uint16_t, string>>(data);
}
