#include <iostream>
#include <iomanip>
#include <thrust/extrema.h>
#include <thrust/device_vector.h>

#define CSC(call)       \
do {                    \
    cudaError_t status = call;          \
    if  (status != cudaSuccess) {       \
        fprintf(stderr, "ERROR in %s:%d. Message: %s\n", __FILE__, __LINE__, cudaGetErrorString(status));   \
        exit(0);                        \
    }                                   \
} while (0)

#define BENCHMARK

#ifdef BENCHMARK
cudaEvent_t benchmarkStart, benchmarkStop;

void startBenchmark() {
    CSC(cudaEventCreate(&benchmarkStart));
    CSC(cudaEventCreate(&benchmarkStop));
    CSC(cudaEventRecord(benchmarkStart));
}

void stopBenchmark() {
    CSC(cudaEventRecord(benchmarkStop));
    CSC(cudaEventSynchronize(benchmarkStop));
    float time;
    CSC(cudaEventElapsedTime(&time, benchmarkStart, benchmarkStop));
    CSC(cudaEventDestroy(benchmarkStart));
    CSC(cudaEventDestroy(benchmarkStop));
    std::cout << "time = " << time << " ms\n";
}
#endif /* BENCHMARK */

struct abs_comparator {
    __host__ __device__ bool operator()(double a, double b) {
        return abs(a) < abs(b);
    }
};

const int SWAP_ROWS_BLOCKS = 32;
const int SWAP_ROWS_THREADS = 32;
const int UPDATE_MATRIX_X_BLOCKS = 16;
const int UPDATE_MATRIX_X_THREADS = 16;
const int UPDATE_MATRIX_Y_BLOCKS = 32;
const int UPDATE_MATRIX_Y_THREADS = 32;

__global__ void swapRows(double* matrix, int n, int row1, int row2) {
    int id_x = blockDim.x * blockIdx.x + threadIdx.x;
    int offset_x = blockDim.x * gridDim.x;
    for (int x = id_x; x < n; x += offset_x) {
        double tmp = matrix[x*n + row1];
        matrix[x*n + row1] = matrix[x*n + row2];
        matrix[x*n + row2] = tmp;
    }
}

__global__ void updateMatrix(double* matrix, int n, int i) {
    int id_x = blockDim.x * blockIdx.x + threadIdx.x;
    int id_y = blockDim.y * blockIdx.y + threadIdx.y;
    int offset_x = blockDim.x * gridDim.x;
    int offset_y = blockDim.y * gridDim.y;
    for (int x = id_x+i+1; x < n; x += offset_x)
        for (int y = id_y+i+1; y < n; y += offset_y)
            matrix[y*n + x] += matrix[y*n + i] * (-matrix[i*n + x] / matrix[i*n + i]);
}


double calculateDet(double* matrix, int n, bool is_even_swaps) {
    double res = 1;
    for (int i = 0; i < n; ++i) {
        if (abs(matrix[i*n + i]) < 1e-7)
            return 0;
        res *= matrix[i*n + i];
    }
    if (is_even_swaps)
        res *= -1;
    return res;
}

int main() {
    int n;
    std::cin >> n;
    double* matrix = (double*)malloc(sizeof(double) * n*n);

    for (int i = 0; i < n; ++i)
        for (int j = 0; j < n; ++j)
            std::cin >> matrix[j*n + i];

    double* matrix_dev;
    CSC(cudaMalloc(&matrix_dev, sizeof(double) * n*n));
    CSC(cudaMemcpy(matrix_dev, matrix, sizeof(double) * n*n, cudaMemcpyHostToDevice));

    const abs_comparator comparator;
    bool is_even_swaps = false;

#ifdef BENCHMARK
    startBenchmark();
#endif /* BENCHMARK */

    for (int i = 0; i < n-1; ++i) {
        const thrust::device_ptr<double> ptr = thrust::device_pointer_cast(matrix_dev + i*n);
        const thrust::device_ptr<double> max_ptr = thrust::max_element(ptr + i,  ptr + n, comparator);
        int max_row = max_ptr - ptr;
        if (max_row != i) {
            swapRows<<<SWAP_ROWS_BLOCKS, SWAP_ROWS_THREADS>>>(matrix_dev, n, i, max_row);
            is_even_swaps ^= true;
        }

        updateMatrix<<<dim3(UPDATE_MATRIX_X_BLOCKS, UPDATE_MATRIX_X_THREADS), dim3(UPDATE_MATRIX_Y_BLOCKS, UPDATE_MATRIX_Y_THREADS)>>>(matrix_dev, n, i);
    }

    CSC(cudaDeviceSynchronize());
    CSC(cudaGetLastError());

#ifdef BENCHMARK
    stopBenchmark();
#endif /* BENCHMARK */

    CSC(cudaMemcpy(matrix, matrix_dev, sizeof(double) * n*n, cudaMemcpyDeviceToHost));
    CSC(cudaFree(matrix_dev));

    double result = calculateDet(matrix, n, is_even_swaps);
    std::cout<< std::scientific << std::setprecision(10) << result;

    free(matrix);
    return 0;
}