#include <bits/stdc++.h>

// Объединение множеств
std::queue<int> Intersection(std::queue<int> left, std::queue<int> right) {
  std::queue<int> result;

  while (!left.empty() and !right.empty()) {
    if (left.front() > right.front()) {
      right.pop();
    } else if (left.front() < right.front()) {
      left.pop();
    } else { // равенство элементов
      result.push(left.front());
      left.pop();
      right.pop();
    }
  }

  return result;
}

// Разделение строк по словам
std::vector<std::string> Separation(std::string &&text) {
  std::vector<std::string> res;
  std::istringstream iss(text);
  while (iss >> text) {
    res.push_back(text);
  }
  return res;
}

int main() {
  // оптимизация
  std::ios_base::sync_with_stdio(false);
  std::cin.tie(nullptr);

  int texts, requests;
  std::unordered_map<std::string, std::queue<int>> index_words; // Хеш таблица (слово - номера текстов)

  std::cin >> texts;
  std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');

  // Разделение текстов на слова и запоминание номеров, в которых содержится слово
  for (int i(0); i < texts; ++i) {
    std::string text;
    getline(std::cin, text);
    for (const auto &word: Separation(std::move(text))) {
      if (index_words[word].empty() || (index_words[word].back() != i)) {
        index_words[word].push(i);
      }
    }
  }

  std::cin >> requests;
  std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');

  for (int idx(0); idx < requests; ++idx) {
    std::string tmp;
    getline(std::cin, tmp);
    const auto words = Separation(std::move(tmp));
    std::queue<int> answer = index_words[words[0]];

    for (int i(1); i < words.size(); ++i) {
      answer = Intersection(answer, index_words[words[i]]);
      if (answer.empty()) { // пустое пересечение
        break;
      }
    }

    std::cout << answer.size() << ' ';

    while (!answer.empty()) {
      std::cout << answer.front() << ' ';
      answer.pop();
    }
    std::cout << '\n';
  }
  return 0;
}
