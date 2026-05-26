#include <stdio.h>
#include <stdlib.h>
#include <assert.h>

__global__ void kernel(float *first_vec, float *second_vec, float *res_vec, int n) {
	int idx = blockIdx.x * blockDim.x + threadIdx.x;
	int offset = blockDim.x * gridDim.x;

	for (; idx < n; idx += offset) {
		if (first_vec[idx] > second_vec[idx])
		res_vec[idx] = first_vec[idx];
	else
		res_vec[idx] = second_vec[idx];
	}
}

int main() {
	int n;
    scanf("%d", &n);

	float *first_arr = (float *)malloc(sizeof(float) * n);
	float *second_arr = (float *)malloc(sizeof(float) * n);
    float *res_arr = (float *)malloc(sizeof(float) * n);

	for(int i(0); i < n; ++i) scanf("%f", &first_arr[i]);
	for(int i(0); i < n; ++i) scanf("%f", &second_arr[i]);
	for(int i(0); i < n; ++i) res_arr[i] = i;

	float *dev_arr1, *dev_arr2, *dev_arr3;
	cudaMalloc(&dev_arr1, sizeof(float) * n);
	cudaMalloc(&dev_arr2, sizeof(float) * n);
	cudaMalloc(&dev_arr3, sizeof(float) * n);

	cudaMemcpy(dev_arr1, first_arr, sizeof(float) * n, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_arr2, second_arr, sizeof(float) * n, cudaMemcpyHostToDevice);
	cudaMemcpy(dev_arr3, res_arr, sizeof(float) * n, cudaMemcpyHostToDevice);
	
	cudaEvent_t start, stop;
	cudaEventCreate(&start);
	cudaEventCreate(&stop);
	cudaEventRecord(start);
	
	kernel<<<1024, 1024>>>(dev_arr1, dev_arr2, dev_arr3, n);
	cudaDeviceSynchronize();
	
	cudaEventRecord(stop);
	cudaEventSynchronize(stop);
	float t;
	cudaEventElapsedTime(&t, start, stop);
	cudaEventDestroy(start);
	cudaEventDestroy(stop);
	printf("time = %f ms\n", t);
	
	cudaMemcpy(res_arr, dev_arr3, sizeof(float) * n, cudaMemcpyDeviceToHost);

	cudaFree(dev_arr1);
    cudaFree(dev_arr2);
    cudaFree(dev_arr3);
	free(first_arr);
    free(second_arr);
    free(res_arr);
	return 0;
}