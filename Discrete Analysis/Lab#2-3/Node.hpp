#pragma once

#include <iostream>

// T обычно располагается в диапазоне от 50 до 2К => все числа, работающие с индексами можно взять за uint16_t

// h ≤ logT((n+1) / 2)
// min grade factor
// T - 1 <= len(keys) <= 2T - 1
const int T = 3;

struct Node {
  int keyCount;
  bool isLeaf;
  std::string keys[2 * T - 1];
  uint64_t values[2 * T - 1];
  Node* children[2 * T];

  Node() {
    keyCount = 0;
    isLeaf = false;
  }
};