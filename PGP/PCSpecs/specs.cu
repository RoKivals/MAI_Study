#include <stdio.h>

int main() {
	int deviceCount;
	cudaDeviceProp devProp;
	cudaGetDeviceCount(&deviceCount);
	printf("Found %d devices\n", deviceCount);
	for(int device = 0;device < deviceCount;device++) {
		cudaGetDeviceProperties(&devProp, device);
		printf("Device %d\n", device);
		printf("Compute capability      : %d.%d\n", devProp.major, devProp.minor);
		printf("Name                    : %s\n", devProp.name);
		printf("Total Global Memory     : %d\n", (int)devProp.totalGlobalMem);
		printf("Shared memory per block : %d\n", (int)devProp.sharedMemPerBlock);
		printf("Registers per block     : %d\n", devProp.regsPerBlock);
		printf("Warp size               : %d\n", devProp.warpSize);
		printf("Max threads per block   : (%d, %d, %d)\n", devProp.maxThreadsDim[0], devProp.maxThreadsDim[1], devProp.maxThreadsDim[2]);
		printf("Max block   : (%d, %d, %d)\n", devProp.maxGridSize[0], devProp.maxGridSize[1], devProp.maxGridSize[2]);
		printf("Total constant memory   : %d\n", (int)devProp.totalConstMem);
		printf("Multiprocessors count   : %d\n", devProp.multiProcessorCount);
	}
	return 0;
}