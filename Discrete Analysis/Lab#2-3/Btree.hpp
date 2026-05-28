#pragma once
#include <fstream>
#include "Node.hpp"

struct Btree {
 private:
  static std::string GetKey(Node *node, int i) {
    return node->keys[i];
  }

  static uint64_t GetValue(Node *node, int i) {
    return node->values[i];
  }

  static Node *GetChild(Node *node, int i) {
    return node->children[i];
  }

  static void SetKeyValue(Node *node, int i, std::string &key, uint64_t val) {
    node->keys[i] = std::move(key);
    node->values[i] = val;
  }

  static void SetChild(Node *node, int i, Node *child) {
    node->children[i] = child;
  }

  bool NodeIsFull(Node *node) {
    return node->keyCount == 2 * T - 1;
  }

  std::string MaxKey(Node *currNode) {
    Node *tmp = currNode;
    while (!tmp->isLeaf) {
      tmp = GetChild(tmp, tmp->keyCount);
    }
    return tmp->keys[tmp->keyCount - 1];
  }

  std::string MinKey(Node *currNode) {
    Node *tmp = currNode;
    while (!tmp->isLeaf) {
      tmp = GetChild(tmp, 0);
    }
    return tmp->keys[0];
  }

 public:
  Node *root;

  Btree() {
    Node *newNode;
    newNode = new Node;
    newNode->keyCount = 0;
    newNode->isLeaf = true;
    this->root = newNode;
  }

  // Check if the key in Tree
  bool PairInTree(Node *currNode, std::string &key, uint64_t &val) {
    if (currNode == nullptr) {
      return false;
    } else if (currNode->isLeaf) {
      for (int i(0); i < currNode->keyCount; ++i) {
        if (key == GetKey(currNode, i)) {
          val = GetValue(currNode, i);
          return true;
        }
      }
      return false;
    } else {
      if (GetKey(currNode, 0) > key) {
        return PairInTree(GetChild(currNode, 0), key, val);
      } else if (key > GetKey(currNode, currNode->keyCount - 1)) {
        return PairInTree(GetChild(currNode, currNode->keyCount), key, val);
      } else {
        for (int i = 0; i < currNode->keyCount; i++) {
          if (key == GetKey(currNode, i)) {
            val = GetValue(currNode, i);
            return true;
          }

          if (currNode->keyCount > 1 and key > GetKey(currNode, i) and GetKey(currNode, i + 1) > key) {
            return PairInTree(GetChild(currNode, i + 1), key, val);
          }
        }
      }
    }
  }

  void BTreeSplitChild(Node *parent, int child_idx) {
    Node *leftChild = GetChild(parent, child_idx);
    Node *rightChild = new Node;

    rightChild->isLeaf = leftChild->isLeaf;
    rightChild->keyCount = T - 1;

    // Перемещаем T - 1 ключ в правого ребёнка
    for (int i(0); i < T - 1; ++i) {
      auto key = GetKey(leftChild, i + T);
      SetKeyValue(rightChild, i, key, GetValue(leftChild, i + T));
    }

    if (!leftChild->isLeaf) {
      for (int i(0); i < T; ++i) {
        SetChild(rightChild, i, GetChild(leftChild, i + T));
      }
    }

    // Добавление ребёнка к родителю
    for (int i(parent->keyCount + 1); i > child_idx + 1; --i) {
      SetChild(parent, i, GetChild(parent, i - 1));
    }
    SetChild(parent, child_idx + 1, rightChild);

    // Добавление нового ключа родителю
    for (int i(parent->keyCount); i > child_idx; i--) {
      auto key = GetKey(parent, i - 1);
      auto value = GetValue(parent, i - 1);
      SetKeyValue(parent, i, key, value);
    }
    auto key = GetKey(leftChild, T - 1);
    auto value = GetValue(parent, T - 1);
    SetKeyValue(parent, child_idx, key, value);

    leftChild->keyCount = T - 1;
    parent->keyCount += 1;
  }

  void TreeInsertNonFullNode(Node *currNode, std::string &key, uint64_t &value) {
    int idx = currNode->keyCount - 1;

    if (currNode->isLeaf) {
      while (idx >= 0 and GetKey(currNode, idx) > key) {
        auto temp_key = GetKey(currNode, idx);
        auto temp_val = GetValue(currNode, idx);
        SetKeyValue(currNode, idx + 1, temp_key, temp_val);
        idx -= 1;
      }

      SetKeyValue(currNode, idx + 1, key, value);
      currNode->keyCount += 1;
    } else {
      while (idx >= 0 and GetKey(currNode, idx) > key) {
        idx -= 1;
      }

      idx += 1;
      if (NodeIsFull(GetChild(currNode, idx))) {
        BTreeSplitChild(currNode, idx);
        if (key > GetKey(currNode, idx)) {
          idx += 1;
        }
      }

      TreeInsertNonFullNode(GetChild(currNode, idx), key, value);
    }
  }

  void TreeInsert(Btree *tree, std::string &key, uint64_t &value) {
    Node *node = tree->root;
    if (!NodeIsFull(node)) {
      TreeInsertNonFullNode(node, key, value);
    } else {
      #TODO: убрать лишние поля
      Node *newRoot = new Node;
      newRoot->isLeaf = false;
      newRoot->keyCount = 0;
      SetChild(newRoot, 0, node);
      tree->root = newRoot;
      BTreeSplitChild(tree->root, 0);
      TreeInsertNonFullNode(tree->root, key, value);
    }
  }

  void MergeChildsToFull(Node *currNode, int pos) {
    Node *leftChild = GetChild(currNode, pos);
    Node *rightChild = GetChild(currNode, pos + 1);

    auto temp_key = GetKey(currNode, pos);
    auto temp_val = GetValue(currNode, pos);
    SetKeyValue(leftChild, T - 1, temp_key, temp_val);

    // Перемещаем пары из правого сына в левого
    for (int i(T); i < 2 * T - 1; ++i) {
      temp_key = GetKey(rightChild, i - T);
      temp_val = GetValue(rightChild, i - T);
      SetKeyValue(leftChild, i, temp_key, temp_val);
    }

    // Перемещаем детей правого сына в левого сына
    if (!leftChild->isLeaf) {
      for (int i(0); i < rightChild->keyCount + 1; ++i) {
        SetChild(leftChild, i + T, GetChild(rightChild, i));
      }
    }
    delete rightChild;
    currNode->keyCount--;

    // Сдвигаем ключи
    for (int i(pos); i < currNode->keyCount; ++i) {
      temp_key = GetKey(currNode, i + 1);
      temp_val = GetValue(currNode, i + 1);
      SetKeyValue(currNode, i, temp_key, temp_val);
    }

    // Сдвигаем детей
    for (int i(pos + 1); i < currNode->keyCount + 1; ++i) {
      SetChild(currNode, i, GetChild(currNode, i + 1));
    }
    leftChild->keyCount = 2 * T - 1;
  }

  void TreeDeleteFromNode(Node *currNode, std::string &key) {
    int idx(0);
    while (idx < currNode->keyCount and key > GetKey(currNode, idx)) {
      idx++;
    }

    if (idx < currNode->keyCount and GetKey(currNode, idx) == key) {
      if (currNode->isLeaf) {
        for (int i(idx); i < currNode->keyCount; ++i) {
          auto temp_key = GetKey(currNode, i + 1);
          auto temp_val = GetValue(currNode, i + 1);
          SetKeyValue(currNode, i, temp_key, temp_val);
        }
        currNode->keyCount--;
      } else {
        StealFromChildren(currNode, idx);
      }
    } else {
      if (GetChild(currNode, idx)->keyCount == T - 1) {
        if (idx != 0 and GetChild(currNode, idx - 1)->keyCount > T - 1) {
          DeleteFromLeftSon(currNode, idx);
        } else if (idx != currNode->keyCount and GetChild(currNode, idx + 1)->keyCount > T - 1) {
          DeleteFromRightSon(currNode, idx);
        } else {
          if (idx != currNode->keyCount) {
            MergeChildsToFull(currNode, idx);
          } else {
            MergeChildsToFull(currNode, idx - 1);
          }
        }
      }
      if (idx > currNode->keyCount) {
        TreeDeleteFromNode(GetChild(currNode, idx - 1), key);
      } else {
        TreeDeleteFromNode(GetChild(currNode, idx), key);
      }
    }
  }

  void StealFromChildren(Node *currNode, int pos) {
    auto key = GetKey(currNode, pos);
    auto child = GetChild(currNode, pos);
    auto right_child = GetChild(currNode, pos + 1);
    uint64_t val;

    if (child->keyCount > T - 1) {
      auto maxKey = MaxKey(child);
      PairInTree(currNode, maxKey, val);
      SetKeyValue(currNode, pos, maxKey, val);
      TreeDeleteFromNode(child, maxKey);
    } else if (right_child->keyCount > T - 1) {
      auto minKey = MinKey(right_child);
      PairInTree(currNode, minKey, val);
      SetKeyValue(currNode, pos, minKey, val);
      TreeDeleteFromNode(GetChild(currNode, pos + 1), minKey);
    } else {
      MergeChildsToFull(currNode, pos);
      TreeDeleteFromNode(GetChild(currNode, pos), key);
    }
  }

  void DeleteFromLeftSon(Node *currNode, int pos) {
    Node *destination = GetChild(currNode, pos);
    Node *sender = GetChild(currNode, pos - 1);

    for (int i(destination->keyCount); i >= 1; --i) {
      auto temp_key = GetKey(destination, i - 1);
      auto temp_val = GetValue(destination, i - 1);
      SetKeyValue(destination, i, temp_key, temp_val);
    }
    if (!destination->isLeaf) {
      for (int i(destination->keyCount + 1); i >= 1; --i) {
        SetChild(destination, i, GetChild(destination, i - 1));
      }
    }

    auto temp_key = GetKey(currNode, pos - 1);
    auto temp_val = GetValue(currNode, pos - 1);
    SetKeyValue(destination, 0, temp_key, temp_val);
    if (!destination->isLeaf) {
      SetChild(destination, 0, GetChild(sender, sender->keyCount));
    }

    temp_key = GetKey(sender, sender->keyCount - 1);
    temp_val = GetValue(sender, sender->keyCount - 1);
    SetKeyValue(currNode, pos - 1, temp_key, temp_val);

    destination->keyCount += 1;
    sender->keyCount -= 1;
  }

  void DeleteFromRightSon(Node *curNode, int pos) {
    Node *destination = GetChild(curNode, pos);
    Node *sender = GetChild(curNode, pos + 1);

    auto temp_key = GetKey(destination, pos);
    auto temp_val = GetValue(destination, pos);

    SetKeyValue(destination, destination->keyCount, temp_key, temp_val);

    if (!destination->isLeaf) {
      SetChild(destination, destination->keyCount + 1, GetChild(sender, 0));
    }

    temp_key = GetKey(sender, 0);
    temp_val = GetValue(sender, 0);
    SetKeyValue(curNode, pos, temp_key, temp_val);

    for (int i = 0; i < sender->keyCount - 1; i++) {
      temp_key = GetKey(sender, i + 1);
      temp_val = GetValue(sender, i + 1);
      SetKeyValue(sender, i, temp_key, temp_val);
    }
    if (!sender->isLeaf) {
      for (int i = 0; i < sender->keyCount + 1; i++) {
        SetChild(sender, i, GetChild(sender, i + 1));
      }
    }

    sender->keyCount -= 1;
    destination->keyCount += 1;
  }

  void ClearTree(Btree *tree, std::string &key) {
    if (tree->root != nullptr) {
      TreeDeleteFromNode(tree->root, key);
      if (tree->root->keyCount == 0) {
        if (tree->root->isLeaf) {
          delete tree->root;
          tree->root = nullptr;
        } else {
          Node *newRoot = GetChild(tree->root, 0);
          delete tree->root;
          tree->root = newRoot;
        }
      }
    }
  }

  void Save(Node *currNode, std::ofstream &stream) {
    if (currNode != nullptr) {
      for (int i = 0; i < currNode->keyCount; i++) {
        uint64_t val = GetValue(currNode, i);
        stream.write((char *) &val, sizeof(uint64_t));
        int strSize = currNode->keys[i].size();
        stream.write((char *) &strSize, sizeof(strSize));
        stream.write(GetKey(currNode, i).c_str(), sizeof(char) * strSize);
      }
      if (!currNode->isLeaf) {
        for (int i = 0; i <= currNode->keyCount; i++) {
          Save(GetChild(currNode, i), stream);
        }
      }
    }
  }

  void Load(Btree *tree, std::ifstream &stream) {
    uint64_t val;
    int strSize;
    std::string key;
    Node *node;
    while (stream.read((char *) &val, sizeof(uint64_t))) {
      stream.read((char *) &strSize, sizeof(strSize));
      key.resize(strSize);
      stream.read((char *) key.data(), strSize);
      TreeInsert(tree, key, val);
    }
  }

  void Destroy(Node *tree) {
    if (tree != nullptr) {
      if (!tree->isLeaf) {
        for (int i = 0; i <= tree->keyCount; i++) {
          Destroy(GetChild(tree, i));
        }
      }
      delete tree;
    }
  }
};


