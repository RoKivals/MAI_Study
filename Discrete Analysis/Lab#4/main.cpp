#include <map>
#include <sstream>
#include <vector>
#include <iostream>
#include <string>
#include <queue>


struct Vertex {
    std::map<std::string, Vertex *> links;
    Vertex *sufflink = nullptr;
    std::vector<int> success; //pattern id

    bool CheckKey(const std::string &key) {
        return links.find(key) != links.end();
    }
};

using TrieNode = std::pair<std::string, Vertex *>;

class AhoTrie {
    Vertex *root;
    int patternSize;
    int pieces;

public:
    std::vector<int> pieceIndex;

    AhoTrie() : patternSize(0), pieces(0) {
        root = new Vertex;
        root->sufflink = root;
    }

    Vertex *Move(Vertex *item, std::string &letter) {
        if (item->CheckKey(letter)) item = item->links.at(letter);
        else {
            if (item == root)
                item = root;
            else
                item = Move(item->sufflink, letter);
        }
        return item;
    }

    static Vertex *GetNextNode(Vertex *item, std::string letter) {
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
            for (auto &ChildPair: node->links) {
                child = ChildPair.second;
                queue.push(child);
                child->sufflink = FindSuffixLinks(child, node, ChildPair.first);

                child->success.insert(child->success.end(),
                                      child->sufflink->success.begin(),
                                      child->sufflink->success.end());
                child->success.shrink_to_fit();
            }
        }
    }

    Vertex *FindSuffixLinks(Vertex *child, Vertex *parent, const std::string &letter) {
        Vertex *linkup = parent->sufflink, *check;

        while (true) {
            check = GetNextNode(linkup, letter);
            if (check) return (check != child) ? check : root;
            if (linkup == root) return root;

            linkup = linkup->sufflink;
        }
    }

    void AddString(const std::vector<std::string> &str) {

        Vertex *bohr = root, *next;
        for (auto &word: str) {
            next = GetNextNode(bohr, word);
            if (!next) {
                next = new Vertex;
                next->sufflink = root;
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

        for (wordNumber = 0; wordNumber < textLen; ++wordNumber) {

            for (auto &i: node->success) {
                successIdx = wordNumber - pieceIndex[i];
                std::cout << placeInfo[successIdx].first << ", "
                          << placeInfo[successIdx].second << ", "
                          << i + 1 << '\n';

            }
//
//            for (size_t i = 0; i < node->success.size(); ++i) {
//                successIdx = wordNumber - pieceIndex[node->success[i]];
//                std::cout << placeInfo[successIdx].first << ", "
//                          << placeInfo[successIdx].second << ", "
//                          << node->success[i] + 1 << '\n';
//            }

            node = Move(node, text[wordNumber]);

        }

        for (size_t i = 0; i < node->success.size(); ++i) {
            successIdx = wordNumber - pieceIndex[node->success[i]];
            std::cout << placeInfo[successIdx].first << ", "
                      << placeInfo[successIdx].second << ", "
                      << node->success[i] + 1 << '\n';
        }
    }

};

int main() {
    std::vector<std::pair<int, int> > grid;
    std::vector<std::string> input;
    std::vector<std::string> patterns;
    std::string line, word;
    std::string pattern;
    AhoTrie b;
    int index = 0;
    while (std::getline(std::cin, pattern) && !pattern.empty()) {
        std::stringstream AlineStream(pattern);
        while (AlineStream >> word) {
            for (unsigned int i = 0; i < word.length(); ++i) {
                word[i] = tolower(word[i]);
            }

            patterns.push_back(word);
            index++;
        }

        b.AddString(patterns);
        b.pieceIndex.push_back(index);

        index = 0;
        pattern.clear();
        patterns.clear();

    }

    int lineIndex = 1, wordIndex = 1;


    while (std::getline(std::cin, line)) {
        std::stringstream lineStream(line);
        while (lineStream >> word) {
            for (unsigned int i = 0; i < word.length(); ++i) {
                word[i] = tolower(word[i]);
            }
            input.push_back(word);
            grid.push_back(std::make_pair(lineIndex, wordIndex));
            wordIndex++;
        }

        wordIndex = 1;
        lineIndex++;

    }
    input.shrink_to_fit();
    grid.shrink_to_fit();
    b.Search(input, grid);
    return 0;
}
