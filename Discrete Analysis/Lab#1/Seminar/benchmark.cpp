#include "sort.hpp"
#include "bits/stdc++.h"

using duration_t = std::chrono::microseconds;

int main() {
    KV kv;
    std::vector<KV> array;
    int32_t max_key = 0;

    while (std::cin >> kv.key >> kv.value) {
        array.push_back(kv);
        max_key = std::max(max_key, kv.key);
    }
    auto array_stl = array;

    std::vector<KV> res;
    std::chrono::time_point<std::chrono::system_clock> start_time = std::chrono::system_clock::now();
    CountingSort(array, res, max_key);
    std::chrono::time_point<std::chrono::system_clock> end_time = std::chrono::system_clock::now();
    uint64_t counting_sort_time = std::chrono::duration_cast<duration_t>(end_time - start_time).count();

    start_time = std::chrono::system_clock::now();
    std::stable_sort(std::begin(array_stl), std::end(array_stl),
                     [](const KV &lhs, const KV &rhs) { return lhs.key < rhs.key; });
    end_time = std::chrono::system_clock::now();

    return 0;
}