#include "bits/stdc++.h"
#include "CountingSort.hpp"


using duration_t = std::chrono::microseconds;
using namespace std;

// Оператор ввода для пары
template<class T, class Y>
std::istream &operator>>(std::istream &in, std::pair<T, Y> &p) {
  in >> p.first >> p.second;
  return in;
}

bool comparator(const std::pair<uint16_t, string> &left, const std::pair<uint16_t, string> &right) {
  return left.first < right.first;
}

int main() {

  std::ios::sync_with_stdio(false);
  std::cin.tie(nullptr);

  vector<std::pair<uint16_t, string>> data;
  std::pair<uint16_t, string> temp;

  while (cin >> temp) {
    data.push_back(std::move(temp));
  }

  std::chrono::time_point<std::chrono::system_clock> start_time = std::chrono::system_clock::now();
  CountingSort(data);
  std::chrono::time_point<std::chrono::system_clock> end_time = std::chrono::system_clock::now();
  uint64_t counting_sort_time = std::chrono::duration_cast<duration_t>(end_time - start_time).count();

  start_time = std::chrono::system_clock::now();
  std::stable_sort(data.begin(), data.end(), comparator);
  end_time = std::chrono::system_clock::now();

  uint64_t std_time = std::chrono::duration_cast<duration_t>(end_time - start_time).count();

  std::cout << "Counting sort time: " << counting_sort_time << std::endl;
  std::cout << "STL stable sort time: " << std_time << std::endl;

  return 0;
}