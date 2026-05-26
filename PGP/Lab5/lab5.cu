#include <iostream>
#include <climits>

const int MAX_SHARED_MEMORY_SIZE = 1024;

__device__ void B_step(int* arr, int len, int id_real, int id_global, int elements_dist, int curr_size) {
    if (id_global % elements_dist >= elements_dist / 2) {
        return;
    }
    bool is_ascending = ((id_global / curr_size) % 2 == 0);
    
    if ((is_ascending && arr[id_real] > arr[id_real + elements_dist / 2]) || (!is_ascending && arr[id_real] < arr[id_real + elements_dist / 2])) {
        int tmp = arr[id_real];
        arr[id_real] = arr[id_real + elements_dist / 2];
        arr[id_real + elements_dist / 2] = tmp;
    }
}

__global__ void kernel_shared(int* dev_nums, int len, int n_b_begin, int curr_size) {
    __shared__ int sh_mem[2048];

    int idx = blockIdx.x * MAX_SHARED_MEMORY_SIZE;
    int offset = MAX_SHARED_MEMORY_SIZE * gridDim.x;

    while (idx < len) {
        sh_mem[threadIdx.x] = dev_nums[idx + threadIdx.x];
        sh_mem[threadIdx.x + MAX_SHARED_MEMORY_SIZE / 2] = dev_nums[idx + threadIdx.x + MAX_SHARED_MEMORY_SIZE / 2];
        __syncthreads();

        for (int elements_dist = n_b_begin; elements_dist >= 2; elements_dist /= 2) {
            for (int step = elements_dist / 2; step > 0; step /= 2) {
                int pos = 2 * threadIdx.x - (threadIdx.x & (step - 1));
                B_step(sh_mem, len, pos, idx + pos, elements_dist, curr_size);
                __syncthreads();
            }
        }

        dev_nums[idx + threadIdx.x] = sh_mem[threadIdx.x];
        dev_nums[idx + threadIdx.x + MAX_SHARED_MEMORY_SIZE / 2] = sh_mem[threadIdx.x + MAX_SHARED_MEMORY_SIZE / 2];
        idx += offset;
    }
}

__global__ void kernel_global(int* dev_nums, int len, int elements_dist, int curr_size) {
    int idx = blockIdx.x * blockDim.x + threadIdx.x;
    int offset = blockDim.x * gridDim.x;

    while (idx < len) {
        B_step(dev_nums, len, idx, idx, elements_dist, curr_size);
        idx += offset;
    }
}


int main() {
    int cnt;
    std::cin.read(reinterpret_cast<char*>(&cnt), sizeof(int));
    
    int n_low_grade = 1;
    while (n_low_grade < cnt) n_low_grade *= 2;

    int* nums = new int[n_low_grade];
    std::cin.read(reinterpret_cast<char*>(nums), sizeof(int) * cnt);

    for (int idx(cnt); idx < n_low_grade; ++idx) nums[idx] = INT_MAX;

    int* dev_nums;
    cudaMalloc(&dev_nums, sizeof(int) * n_low_grade); 
    cudaMemcpy(dev_nums, nums, sizeof(int) * n_low_grade, cudaMemcpyHostToDevice);

    for (int curr_size(2); curr_size <= n_low_grade; curr_size *= 2) {
        for (int elements_dist(curr_size); elements_dist >= 2; elements_dist /= 2) {
            if (elements_dist == MAX_SHARED_MEMORY_SIZE) {
                kernel_shared<<<512, 512>>>(dev_nums, n_low_grade, elements_dist, curr_size);
                break;
            } else {
                kernel_global<<<512, 512>>>(dev_nums, n_low_grade, elements_dist, curr_size);
            }
        }
    }

    cudaMemcpy(nums, dev_nums, sizeof(int) * n_low_grade, cudaMemcpyDeviceToHost);
    std::cout.write(reinterpret_cast<const char*>(nums), sizeof(int) * cnt);
    
    cudaFree(dev_nums);
    delete[] nums;
    return 0;
}