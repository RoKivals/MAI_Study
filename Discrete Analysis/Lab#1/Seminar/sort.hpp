#pragma once
#include <vector>
#include <iostream>

struct KV {
 public:
  int32_t key;
  char value;

  KV() = default;
  KV(int32_t key, char value) : key(key), value(value) {};
};

void CountingSort(const std::vector<KV> &, std::vector<KV> &, const int32_t &);