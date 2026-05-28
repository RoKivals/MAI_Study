#include <iostream>
#include <string>
#include <cstdint>


uint64_t container[100][100]; // запоминание прошлых вычислений (мемоизация)

uint64_t Palindromes(std::string &str, char begin, char end) {
  if (begin > end) {
    return 0;
  } else if (begin == end) {
    return 1;   // если один элемент, то он сам паллиндром, и больше нет
  } else {
    if (container[begin][end] != 0) {
      return container[begin][end];
    } else {
      if (str[begin] == str[end]) {
        container[begin][end] = Palindromes(str, begin + 1, end) + Palindromes(str, begin, end - 1) + 1;
      } else {
        container[begin][end] = Palindromes(str, begin + 1, end) + Palindromes(str, begin, end - 1) -
                                Palindromes(str, begin + 1, end - 1);
      }
    }
    return container[begin][end];
  }
}

int main() {
  std::string str;
  for (char i(0); i < str.size(); i++) {
    for (char j(0); j < str.size(); j++) {
      container[i][j] = 0;
    }
  }
  std::cin >> str;
  std::cout << Palindromes(str, 0, str.size() - 1) << "\n";
  return 0;
}