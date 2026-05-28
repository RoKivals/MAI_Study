#include <unordered_map>
#include <sstream>
#include <vector>
#include <iostream>
#include <string>
#include <queue>
#include <algorithm>

template<class T = std::string>
class AhoTrie {
 private:
  struct Vertex {
    std::unordered_map<T, Vertex *> links;
    Vertex *failLink = nullptr;
    std::vector<int> success;

    bool CheckKey(const T &key) {
      return links.find(key) != links.end();
    }
  };
  Vertex *root;
  int pieces;
  using TrieNode = std::pair<T, Vertex *>;

 public:
  std::vector<int> pieceIndex;

  AhoTrie() : pieces(0) {
    root = new Vertex;
    root->failLink = root;
  }

  Vertex *Move(Vertex *item, T &letter) {
    if (item->CheckKey(letter)) item = item->links.at(letter);
    else {
      if (item == root)
        item = root;
      else
        item = Move(item->failLink, letter);
    }
    return item;
  }

  static Vertex *GetNextNode(Vertex *item, const T &letter) {
    if (!item)
      return nullptr;
    if (item->CheckKey(letter)) item = item->links.at(letter);
    else item = nullptr;
    return item;
  }

  void Linkate() {
    Vertex *node, *child;
    std::queue<Vertex *> queue;
    queue.push(root);

    while (!queue.empty()) {
      node = queue.front();
      queue.pop();
      for (auto &ChildPair : node->links) {
        child = ChildPair.second;
        queue.push(child);
        child->failLink = FindSuffixLinks(child, node, ChildPair.first);

        child->success.insert(child->success.end(),
                              child->failLink->success.begin(),
                              child->failLink->success.end());
        child->success.shrink_to_fit();
      }
    }
  }

  Vertex *FindSuffixLinks(Vertex *child, Vertex *parent, const std::string &letter) {
    Vertex *linkup = parent->failLink, *check;

    while (true) {
      check = GetNextNode(linkup, letter);
      if (check) return (check != child) ? check : root;
      if (linkup == root) return root;

      linkup = linkup->failLink;
    }
  }

  void AddString(const std::vector<std::string> &str) {

    Vertex *bohr = root, *next;
    for (auto &word : str) {
      next = GetNextNode(bohr, word);
      if (!next) {
        next = new Vertex;
        next->failLink = root;
        bohr->links.insert(TrieNode(word, next));
      }
      bohr = next;
    }
    bohr->success.push_back(pieces);
    ++pieces;
  }

  void Search(std::vector<std::string> &text,
              std::vector<std::pair<int, int> > &placeInfo) {
    Linkate();

    size_t textLen = text.size();
    Vertex *node = root;
    int successIdx;
    int wordNumber;

    for (wordNumber = 0; wordNumber <= textLen; ++wordNumber) {

      for (auto &i : node->success) {
        successIdx = wordNumber - pieceIndex[i];
        std::cout << placeInfo[successIdx].first << ", "
                  << placeInfo[successIdx].second << ", "
                  << i + 1 << '\n';

      }
      if (wordNumber < textLen) node = Move(node, text[wordNumber]);
    }
  }

};

int main() {
  std::vector<std::pair<int, int>> grid;
  std::vector<std::string> input, patterns;

  std::string line, word;
  std::string pattern;
  AhoTrie<std::string> bohr;

  while (std::getline(std::cin, pattern) && !pattern.empty()) {
    std::stringstream AlineStream(pattern);
    int index = 0;
    while (AlineStream >> word) {
      std::transform(word.begin(), word.end(), word.begin(), ::tolower);
      patterns.push_back(word);
      index++;
    }

    bohr.AddString(patterns);
    bohr.pieceIndex.push_back(index);

    pattern.clear();
    patterns.clear();
  }

  int lineIndex = 1, wordIndex = 1;

  while (std::getline(std::cin, line)) {
    std::stringstream lineStream(line);

    while (lineStream >> word) {
      std::transform(word.begin(), word.end(), word.begin(), ::tolower);
      input.push_back(word);

      grid.emplace_back(lineIndex, wordIndex);
      wordIndex++;
    }

    wordIndex = 1;
    lineIndex++;
  }

  input.shrink_to_fit();
  grid.shrink_to_fit();
  bohr.Search(input, grid);
  return 0;
}
