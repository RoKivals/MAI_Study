#include <stdio.h>
#include <string.h>
#include <thrust/extrema.h>
#include <thrust/device_vector.h>
#include <thrust/device_ptr.h>
#include <math.h>
#include <float.h>

const int THREADS_PER_BLOCK = 256;
const int BLOCKS_PER_GRID = 128;

cudaEvent_t benchmarkStart, benchmarkStop;
void startBenchmark() {
    cudaEventCreate(&benchmarkStart);
    cudaEventCreate(&benchmarkStop);
    cudaEventRecord(benchmarkStart);
}

void stopBenchmark() {
    cudaEventRecord(benchmarkStop);
    cudaEventSynchronize(benchmarkStop);
    float time;
    cudaEventElapsedTime(&time, benchmarkStart, benchmarkStop);
    cudaEventDestroy(benchmarkStart);
    cudaEventDestroy(benchmarkStop);
    std::cout << "time = " << time << " ms\n";
}

__host__ void gpu_print_matrix(double* matrix, int size) {
	for (int i(0); i < size; ++i) {
		for (int j(0); j < size; ++j) {
			printf("%.1f ", matrix[i * size + j]);
		}
		printf("\n");
	}
}

__global__ void gpu_transpose(double* matrix, int size) {
	int idx = blockIdx.x * blockDim.x + threadIdx.x;
	int offsetx = gridDim.x * blockDim.x;
    double temp;
    int curr_row;
    int curr_col;

    while(idx < size * size) {
        curr_row = idx / size;
        curr_col = idx % size;
        if(curr_col > curr_row) {
            temp = matrix[curr_row * size + curr_col];
            matrix[curr_row * size + curr_col] = matrix[curr_col * size + curr_row];
            matrix[curr_col * size + curr_row] = temp;
        }
	    idx += offsetx;
	}
}

__global__ void gpu_swap(double* matrix, int size, int row_from, int row_to) {
	int idx = blockIdx.x * blockDim.x + threadIdx.x;
	int offsetx = gridDim.x * blockDim.x;
	double tmp;
	for (int i = idx; i < size; i += offsetx) {
		tmp = matrix[(i * size) + row_from];
		matrix[(i * size) + row_from] = matrix[(i * size) + row_to];
		matrix[(i * size) + row_to] = tmp;
	}
}

__global__ void gpu_compute_L(double* matrix, double* L, int size, int curr_row) {
	int idx = blockIdx.x * blockDim.x + threadIdx.x;
	int offsetx = gridDim.x * blockDim.x;

	for(; idx < size; idx += offsetx) {
		if(idx < curr_row)
			continue;

		if(idx == curr_row ) L[curr_row * size + curr_row] = 1.0;

		else if(fabs(matrix[curr_row * size + curr_row]) > 10e-7) L[curr_row * size + idx] = matrix[curr_row * size + idx] / matrix[curr_row * size + curr_row];
	}
}

__global__ void gpu_modify_matrix(double* matrix, double* L, int size, int max_col) {
	int idx = blockIdx.x * blockDim.x + threadIdx.x;
	int offsetx = gridDim.x * blockDim.x;
	int curr_row;
	int curr_col;
	for(; idx < size * size; idx += offsetx)
	{
		curr_row = idx / size;
		curr_col = idx % size;
		if(curr_col == max_col)
			continue;
		else
		{
			matrix[curr_row * size + curr_col] -= L[max_col * size + curr_col] * matrix[curr_row * size + max_col];
		}
	}
}

struct comparator
{
	__host__ __device__ bool operator()(double lhs, double rhs)
	{
		return fabs(lhs) < fabs(rhs);
	} 
};

long double calculateDet(double* matrix, double* L, int dim, int sign) {
    long double res = 1;
	for(int i = 0; i < dim; ++i) {
		res *= matrix[i * dim + i] * L[i * dim + i];
	}
	if(fabs(res) <= 10e-7)
		return res;
	else
		return res * sign;
}

int main()
{
	int n;
	scanf("%d", &n);
	double* matrix = (double*)malloc(sizeof(double) * n * n);
	for (int i = 0; i < n * n; ++i) {
		scanf("%lf", &matrix[i]);
	}

	double* matrix_dev;
	cudaMalloc(&matrix_dev, sizeof(double) * n * n);
	cudaMemcpy(matrix_dev, matrix, sizeof(double) * n * n, cudaMemcpyHostToDevice);

	double* L = (double*) calloc(n * n, sizeof(double));
	double* L_dev;
	cudaMalloc(&L_dev, sizeof(double) * n * n);
	cudaMemcpy(L_dev, L, sizeof(double) * n * n, cudaMemcpyHostToDevice);
	
	int pos_of_max;
	int sign = 1;
	comparator comp;
	auto p_matrix = thrust::device_pointer_cast(matrix_dev);
	
	startBenchmark();
	gpu_transpose <<<BLOCKS_PER_GRID, THREADS_PER_BLOCK >>>(matrix_dev, n);

	for (int row = 0; row < n; ++row) {
		auto max_elem = thrust::max_element(p_matrix + (row * n) + row, p_matrix + ((row + 1) * n), comp);
		pos_of_max = (int)(max_elem - p_matrix) % n;

		if(row != pos_of_max) {
			sign *= -1;
			gpu_swap<<<BLOCKS_PER_GRID, THREADS_PER_BLOCK>>>(matrix_dev, n, row, pos_of_max);
		}

		gpu_compute_L << <BLOCKS_PER_GRID, THREADS_PER_BLOCK >> > (matrix_dev, L_dev, n, row);
		gpu_modify_matrix <<< BLOCKS_PER_GRID, THREADS_PER_BLOCK >>> (matrix_dev, L_dev, n, row);
	}
	
	stopBenchmark();
	cudaMemcpy(L, L_dev, sizeof(double) * n * n, cudaMemcpyDeviceToHost);
	cudaMemcpy(matrix, matrix_dev, sizeof(double) * n * n, cudaMemcpyDeviceToHost);

    auto result = calculateDet(matrix, L, n, sign);
    printf("%.10Lf\n", result);

	free(matrix);
	free(L);
	cudaFree(matrix_dev);	
	cudaFree(L_dev);
}