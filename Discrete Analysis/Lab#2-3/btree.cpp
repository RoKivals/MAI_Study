#include <iostream>
#include <string>
#include <fstream>
using namespace std;

const int T = 3;

struct Node {
  int n;
  bool isLeaf;
  std::string keys[2 * T - 1];
  uint64_t values[2 * T - 1];
  Node *children[2 * T];
};

struct Btree {
  Node *root;
};

std::string GetKey(Node *node, int i) {
  return node->keys[i];
}

uint64_t GetValue(Node *node, int i) {
  return node->values[i];
}

void SetKeyValue(Node *node, int i, std::string &key, uint64_t val) {
  node->keys[i] = key;
  node->values[i] = val;
}

Node *GetChild(Node *node, int i) {
  return node->children[i];
}

void SetChild(Node *node, int i, Node *child) {
  node->children[i] = child;
}

void TreeInit(Btree *tree) {
  Node *newNode;
  newNode = new(std::nothrow) Node;
  if (newNode == nullptr) {
    std::cout << "ERROR\n";
  }
  newNode->n = 0;
  newNode->isLeaf = true;
  tree->root = newNode;
}

bool TreeSearch(Node *curNode, std::string &key, uint64_t &val) {
  if (curNode == nullptr) {
    return false;
  } else if (curNode->isLeaf) {
    for (int i = 0; i < curNode->n; i++) {
      if (key == GetKey(curNode, i)) {
        val = GetValue(curNode, i);
        return true;
      }
    }
    return false;
  } else {
    if (GetKey(curNode, 0) > key) {
      return TreeSearch(GetChild(curNode, 0), key, val);
    } else if (key > GetKey(curNode, curNode->n - 1)) {
      return TreeSearch(GetChild(curNode, curNode->n), key, val);
    } else {
      for (int i = 0; i < curNode->n; i++) {
        if (key == GetKey(curNode, i)) {
          val = GetValue(curNode, i);
          return true;
        }
        if (curNode->n > 1 and key > GetKey(curNode, i) and GetKey(curNode, i + 1) > key) {
          return TreeSearch(GetChild(curNode, i + 1), key, val);
        }
      }
    }
  }
}

void TreeSplitChild(Node *parNode, int cind) {
  Node *leftChild = GetChild(parNode, cind);
  Node *rightChild;
  rightChild = new(std::nothrow) Node;
  if (rightChild == nullptr) {
    std::cout << "ERROR\n";
  }
  rightChild->isLeaf = leftChild->isLeaf;
  rightChild->n = T - 1;
  leftChild->n = T - 1;
  std::string key;
  for (int i = T; i < 2 * T - 1; i++) {
    key = GetKey(leftChild, i);
    SetKeyValue(rightChild, i - T, key, GetValue(leftChild, i));
  }
  if (!leftChild->isLeaf) {
    for (int i = T; i < 2 * T; i++) {
      SetChild(rightChild, i - T, GetChild(leftChild, i));
    }
  }

  parNode->n += 1;
  for (int i = (parNode->n + 1) - 1; i > cind; i--) {
    SetChild(parNode, i, GetChild(parNode, i - 1));
  }
  SetChild(parNode, cind + 1, rightChild);
  for (int i = parNode->n - 1; i > cind; i--) {
    key = GetKey(parNode, i - 1);
    SetKeyValue(parNode, i, key, GetValue(parNode, i - 1));
  }
  key = GetKey(leftChild, T - 1);
  SetKeyValue(parNode, cind, key, GetValue(leftChild, T - 1));
}

void TreeInsertToUnfullNode(Node *curNode, std::string &key, uint64_t value) {
  int pos = curNode->n - 1;
  std::string x;
  if (curNode->isLeaf) {
    while (pos >= 0 and GetKey(curNode, pos) > key) {
      x = GetKey(curNode, pos);
      SetKeyValue(curNode, pos + 1, x, GetValue(curNode, pos));
      pos -= 1;
    }
    SetKeyValue(curNode, pos + 1, key, value);
    curNode->n += 1;
  } else {
    while (pos >= 0 and GetKey(curNode, pos) > key) {
      pos -= 1;
    }
    pos += 1;
    if (GetChild(curNode, pos)->n == 2 * T - 1) {
      TreeSplitChild(curNode, pos);
      if (key > GetKey(curNode, pos)) {
        pos += 1;
      }
    }
    TreeInsertToUnfullNode(GetChild(curNode, pos), key, value);
  }
}

void TreeInsert(Btree *tree, std::string &key, uint64_t value) {
  if (tree->root == nullptr) {
    TreeInit(tree);
  }
  if (tree->root->n < 2 * T - 1) {
    TreeInsertToUnfullNode(tree->root, key, value);
  } else {
    Node *newRoot;
    newRoot = new(std::nothrow) Node;
    newRoot->isLeaf = false;
    newRoot->n = 0;
    SetChild(newRoot, 0, tree->root);
    tree->root = newRoot;
    TreeSplitChild(tree->root, 0);
    TreeInsertToUnfullNode(tree->root, key, value);
  }
}

std::string FindMaxKey(Node *curNode) {
  Node *tmp = curNode;
  while (!tmp->isLeaf) {
    tmp = GetChild(tmp, tmp->n);
  }
  return tmp->keys[tmp->n - 1];
}

std::string FindMinKey(Node *curNode) {
  Node *tmp = curNode;
  while (!tmp->isLeaf) {
    tmp = GetChild(tmp, 0);
  }
  return tmp->keys[0];
}

void TreeMerge(Node *curNode, int pos) {
  Node *leftChild = GetChild(curNode, pos);
  Node *rightChild = GetChild(curNode, pos + 1);
  std::string x;

  x = GetKey(curNode, pos);
  SetKeyValue(leftChild, T - 1, x, GetValue(curNode, pos));

  for (int i = T; i < 2 * T - 1; i++) {
    x = GetKey(rightChild, i - T);
    SetKeyValue(leftChild, i, x, GetValue(rightChild, i - T));
  }

  if (!leftChild->isLeaf) {
    for (int i = 0; i < rightChild->n + 1; i++) {
      SetChild(leftChild, i + T, GetChild(rightChild, i));
    }
  }
  delete rightChild;
  curNode->n--;
  for (int i = pos; i < curNode->n; i++) {
    x = GetKey(curNode, i + 1);
    SetKeyValue(curNode, i, x, GetValue(curNode, i + 1));
  }

  for (int i = pos + 1; i < curNode->n + 1; i++) {
    SetChild(curNode, i, GetChild(curNode, i + 1));
  }
  leftChild->n = 2 * T - 1;
}

void TreeDeleteFromNode(Node *root, std::string &key);

void StealFromChildren(Node *curNode, int pos) {
  std::string key = GetKey(curNode, pos);
  if (GetChild(curNode, pos)->n > T - 1) {
    std::string maxKey = FindMaxKey(GetChild(curNode, pos));
    uint64_t val;
    TreeSearch(curNode, maxKey, val);
    SetKeyValue(curNode, pos, maxKey, val);
    TreeDeleteFromNode(GetChild(curNode, pos), maxKey);
  } else if (GetChild(curNode, pos + 1)->n > T - 1) {
    std::string minKey = FindMinKey(GetChild(curNode, pos + 1));
    uint64_t val;
    TreeSearch(curNode, minKey, val);
    SetKeyValue(curNode, pos, minKey, val);
    TreeDeleteFromNode(GetChild(curNode, pos + 1), minKey);
  } else {
    TreeMerge(curNode, pos);
    TreeDeleteFromNode(GetChild(curNode, pos), key);
  }
}

void StealFromLeftBrother(Node *curNode, int pos) {
  Node *thief = GetChild(curNode, pos);
  Node *victim = GetChild(curNode, pos - 1);
  std::string x;
  for (int i = thief->n; i >= 1; i--) {
    x = GetKey(thief, i - 1);
    SetKeyValue(thief, i, x, GetValue(thief, i - 1));
  }
  if (!thief->isLeaf) {
    for (int i = thief->n + 1; i >= 1; i--) {
      SetChild(thief, i, GetChild(thief, i - 1));
    }
  }

  x = GetKey(curNode, pos - 1);
  SetKeyValue(thief, 0, x, GetValue(curNode, pos - 1));
  if (!thief->isLeaf) {
    SetChild(thief, 0, GetChild(victim, victim->n));
  }

  x = GetKey(victim, victim->n - 1);
  SetKeyValue(curNode, pos - 1, x, GetValue(victim, victim->n - 1));

  thief->n += 1;
  victim->n -= 1;
}

void StealFromRightBrother(Node *curNode, int pos) {
  Node *thief = GetChild(curNode, pos);
  Node *victim = GetChild(curNode, pos + 1);
  std::string x;

  x = GetKey(curNode, pos);
  SetKeyValue(thief, thief->n, x, GetValue(curNode, pos));

  if (!thief->isLeaf) {
    SetChild(thief, thief->n + 1, GetChild(victim, 0));
  }

  x = GetKey(victim, 0);
  SetKeyValue(curNode, pos, x, GetValue(victim, 0));

  for (int i = 0; i < victim->n - 1; i++) {
    x = GetKey(victim, i + 1);
    SetKeyValue(victim, i, x, GetValue(victim, i + 1));
  }
  if (!victim->isLeaf) {
    for (int i = 0; i < victim->n + 1; i++) {
      SetChild(victim, i, GetChild(victim, i + 1));
    }
  }

  victim->n -= 1;
  thief->n += 1;
}

void TreeDeleteFromNode(Node *curNode, std::string &key) {
  int pos = 0;
  while (pos < curNode->n and key > GetKey(curNode, pos)) {
    pos++;
  }

  if (pos < curNode->n and GetKey(curNode, pos) == key) {
    if (curNode->isLeaf) {
      for (int i = pos + 1; i < curNode->n; i++) {
        std::string x = GetKey(curNode, i);
        SetKeyValue(curNode, i - 1, x, GetValue(curNode, i));
      }
      curNode->n--;
    } else {
      StealFromChildren(curNode, pos);
    }
  } else {
    if (GetChild(curNode, pos)->n == T - 1) {
      if (pos != 0 and GetChild(curNode, pos - 1)->n > T - 1) {
        StealFromLeftBrother(curNode, pos);
      } else if (pos != curNode->n and GetChild(curNode, pos + 1)->n > T - 1) {
        StealFromRightBrother(curNode, pos);
      } else {
        if (pos != curNode->n) {
          TreeMerge(curNode, pos);
        } else {
          TreeMerge(curNode, pos - 1);
        }
      }
    }
    if (pos > curNode->n) {
      TreeDeleteFromNode(GetChild(curNode, pos - 1), key);
    } else {
      TreeDeleteFromNode(GetChild(curNode, pos), key);
    }
  }
}

void TreeDelete(Btree *tree, std::string &key) {
  if (tree->root != nullptr) {
    TreeDeleteFromNode(tree->root, key);
    if (tree->root->n == 0) {
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

void Save(Node *curNode, std::ofstream &stream) {
  if (curNode != nullptr) {
    for (int i = 0; i < curNode->n; i++) {
      uint64_t val = GetValue(curNode, i);
      stream.write((char *) &val, sizeof(uint64_t));
      int strSize = curNode->keys[i].size();
      stream.write((char *) &strSize, sizeof(strSize));
      stream.write(GetKey(curNode, i).c_str(), sizeof(char) * strSize);
    }
    if (!curNode->isLeaf) {
      for (int i = 0; i <= curNode->n; i++) {
        Save(GetChild(curNode, i), stream);
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

void Destroy(Node *root) {
  if (root != nullptr) {
    if (!root->isLeaf) {
      for (int i = 0; i <= root->n; i++) {
        Destroy(GetChild(root, i));
      }
    }
    delete root;
  }
}

void Parser(Btree *tree) {
  std::string cmd;
  while (std::cin >> cmd) {
    if (cmd == "+") {
      std::string key;
      uint64_t value;
      std::cin >> key >> value;
      for (int i = 0; i < key.size(); i++) {
        key[i] = tolower(key[i]);
      }
      uint64_t res;
      if (TreeSearch(tree->root, key, res)) {
        std::cout << "Exist\n";
      } else {
        TreeInsert(tree, key, value);
        std::cout << "OK\n";
      }
    } else if (cmd == "-") {
      std::string key;
      std::cin >> key;
      for (int i = 0; i < key.size(); i++) {
        key[i] = tolower(key[i]);
      }
      uint64_t res;
      if (!TreeSearch(tree->root, key, res)) {
        std::cout << "NoSuchWord\n";
      } else {
        TreeDelete(tree, key);
        std::cout << "OK\n";
      }
    } else if (cmd == "!") {
      std::string cmd, path;
      std::cin >> cmd;
      if (cmd == "Save") {
        std::cin >> path;
        std::ofstream ostream(path, std::ios::binary);
        Save(tree->root, ostream);
        ostream.close();
        std::cout << "OK\n";
      } else if (cmd == "Load") {
        std::cin >> path;
        std::ifstream istream(path, std::ios::binary);
        Destroy(tree->root);
        TreeInit(tree);
        Load(tree, istream);
        istream.close();
        std::cout << "OK\n";
      } else {
        std::cout << "ERROR\n";
      }
    } else {
      for (int i = 0; i < cmd.size(); i++) {
        cmd[i] = tolower(cmd[i]);
      }
      uint64_t val;
      if (!TreeSearch(tree->root, cmd, val)) {
        std::cout << "NoSuchWord\n";
      } else {
        std::cout << "OK: " << val << "\n";
      }
    }
  }
}

int main() {
  std::ios_base::sync_with_stdio(false);
  std::cin.tie(0);
  Btree *tree;
  tree = new(std::nothrow) Btree;
  TreeInit(tree);
  Parser(tree);
  Destroy(tree->root);
  delete tree;
  return 0;
}
