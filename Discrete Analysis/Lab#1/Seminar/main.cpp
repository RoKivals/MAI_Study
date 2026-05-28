#include "sort.hpp"

int main() {
    std::vector<KV> input;
    int32_t key, max_key = 0;
    char value;
    while (std::cin >> key >> value) {
        input.emplace_back(key, value);
        max_key = std::max(max_key, key);
    }

    std::vector<KV> result;
    CountingSort(input, result, max_key);
    for (auto elem: input) {
        std::cout << "Key: " << elem.key << " value: " << elem.value << '\n';
    }
    return 0;
}