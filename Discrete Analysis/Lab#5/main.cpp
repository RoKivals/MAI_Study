#include <iostream>
#include <string>
#include <unordered_map>

const char SENTINEL = '$';

struct Vertex {
  size_t start;
  int *end, suffixNumber;
  Vertex *suffixLink;
  std::unordered_map<char, Vertex *> child;

  Vertex(size_t start, int *end, int suffixNumber, Vertex *suffixLink)
          : start(start), end(end), suffixNumber(suffixNumber), suffixLink(suffixLink) {}
};

class SuffixTree {
/*
 * Правила вставки буквы в дерево
 * 1) Попал в лист - дописать букву на ребро
 * 2) Пути к букве нет - создать новый лист
 * 3) Строка с такой буквой (суффиксом) уже существует - ничего не делаем
 * Если строка есть в суффиксном дереве, то все её суффиксы так же в нём находятся - их обрабатывать не надо
 *
 * Случаи суффиксных ссылок (префикс без первого символа)
 * 1) Существует вн вершина в дереве
 * 2) Существует ребро без вн вершины - надо добавить вн вершину и разделить ребро
*/

public:
  explicit SuffixTree(std::string &text);

  int FindMinimumLineSectionSuffixNumber(int n);

private:
  void Build();

  int DFS(Vertex *node, int n);

private:
  std::string text;
  Vertex *root = nullptr;
};


SuffixTree::SuffixTree(std::string &text) {
  text = text + SENTINEL;
  this->text = text;
  this->root = new Vertex(0, new int(-1), -1, nullptr);
  this->Build();
}

// Построение СД с алг Укконена O(n) //
void SuffixTree::Build() {
  Vertex *currentNode = root;
  size_t suffixNum = 0, currentTextIndex = 0, currentEdgeLength = 0;
  // Индекс правой границы диапазона листьев (кол-во обработанных на данный момент символов)
  int *GlobalLeafIdx = new int(-1);

  for (size_t prefixNum(0); prefixNum < text.length(); ++prefixNum) {
    Vertex *lastCreatedVertex = nullptr;
    (*GlobalLeafIdx)++;

    for (; suffixNum <= prefixNum;) {
      if (currentEdgeLength == 0) {
        currentTextIndex = prefixNum;
      }
      // Если в ребре не хранится text[currentTextIndex]
      if (!currentNode->child.contains(text[currentTextIndex])) {
        currentNode->child[text[currentTextIndex]] = new Vertex(prefixNum, GlobalLeafIdx, suffixNum, root);

        if (lastCreatedVertex) {
          lastCreatedVertex->suffixLink = currentNode;
          lastCreatedVertex = nullptr;
        }
      } else { // Если в дереве уже находится text[currentTextIndex]
        Vertex *nextNode = currentNode->child[text[currentTextIndex]];
        size_t nextEdgeLength = *(nextNode->end) - (nextNode->start) + 1;

        // Пропускаем ребро, если кол-во букв в рассматриваемой строке больше, чем на ребре
        if (currentEdgeLength >= nextEdgeLength) {
          currentTextIndex += nextEdgeLength;
          currentEdgeLength -= nextEdgeLength;
          currentNode = nextNode;
          continue;
        }

        // Если символ находится на ребре
        if (text[nextNode->start + currentEdgeLength] == text[prefixNum]) {
          ++currentEdgeLength;
          if (lastCreatedVertex and currentNode != root) {
            lastCreatedVertex->suffixLink = currentNode;
          }
          break;
        }

        // Создание внутренней вершины в ребре
        Vertex *newNode = new Vertex(nextNode->start, new int(nextNode->start + currentEdgeLength - 1), -1, root);
        currentNode->child[text[currentTextIndex]] = newNode;
        nextNode->start += currentEdgeLength;
        newNode->child[text[nextNode->start]] = nextNode;
        newNode->child[text[prefixNum]] = new Vertex(prefixNum, GlobalLeafIdx, suffixNum, root);
        if (lastCreatedVertex) {
          lastCreatedVertex->suffixLink = newNode;
        }
        lastCreatedVertex = newNode;
      }

      if (currentNode == root) {
        if (currentEdgeLength) {
          ++currentTextIndex;
          --currentEdgeLength;
        }
      } else {
        currentNode = currentNode->suffixLink;
      }
      ++suffixNum;
    }
  }
}

int SuffixTree::DFS(Vertex *node, int n) {
  if (!node) {
    return -1;
  }

  if ((*node->end) > n and node->suffixNumber != -1) {
    return node->suffixNumber;
  }

  if (node->child.empty()) {
    return -1;
  }

  char minChar = 'z' + 1;
  for (auto &key: node->child) {
    if (key.first != '$' and key.first < minChar) {
      minChar = key.first;
    }
  }

  if (minChar == 'z' + 1) {
    minChar = '$';
  }

  return DFS(node->child[minChar], n);
}

int SuffixTree::FindMinimumLineSectionSuffixNumber(int n) {
  return DFS(root, n);
}

int main() {
  std::ios_base::sync_with_stdio(false);
  std::cin.tie(nullptr);

  std::string s;
  std::cin >> s;

  size_t n = s.size();
  s = s + s;
  SuffixTree st(s);
  int suf = st.FindMinimumLineSectionSuffixNumber(n);
  std::cout << s.substr(suf, n) << '\n';
  return 0;
}