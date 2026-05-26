#include <iostream>
#include <iomanip>

int main() {
    int cnt;
    std::cin >> cnt;

    float* arr = (float*) malloc(cnt * sizeof(float));

    for (int i(0); i < cnt; ++i) std::cin >> arr[i];

    for (int i(0); i < cnt - 1; ++i) {
        for (int j(0); j < cnt - i - 1; j++){
            if (arr[j] > arr[j + 1]) {
                arr[j + 1] += arr[j];
                arr[j] = arr[j + 1] - arr[j];
                arr[j + 1] -= arr[j]; 
            }
        }
    }

    for (int i(0); i < cnt; ++i) std::cout << std::scientific << std::setprecision(6) << arr[i] << " ";
    return 0;
}