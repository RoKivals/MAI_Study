#include <iostream>
#include <fstream>

__global__ void kernel(cudaTextureObject_t tex, uchar4 *dst, int width, int height, int widthRatio, int heightRatio)
{
  /*
    threadIdx.x, threadIdx.y, threadIdx.z
    blockIdx.x, blockIdx.y, blockIdx.z
    blockDim.x, blockDim.y, blockDim.z
    gridDim.x, gridDim.y, gridDim.z
  */
    int width_idx = blockDim.x * blockIdx.x + threadIdx.x;
    int height_idx = blockDim.y * blockIdx.y + threadIdx.y;
    int offset_x = blockDim.x * gridDim.x;
    int offset_y = blockDim.y * gridDim.y;
    int cntSamples = widthRatio * heightRatio;

    for (int x = width_idx; x < width; x += offset_x) {
        for (int y = height_idx; y < height; y += offset_y) {
            int3 samples;
            samples.x = 0;
            samples.y = 0;
            samples.z = 0;
            for (int i = 0; i < widthRatio; ++i) {
                for (int j = 0; j < heightRatio; ++j) {
                    uchar4 p = tex2D<uchar4>(tex, x * widthRatio + i, y * heightRatio + j);
                    samples.x += p.x;
                    samples.y += p.y;
                    samples.z += p.z;
                }
            }
            samples.x /= cntSamples;
            samples.y /= cntSamples;
            samples.z /= cntSamples;
            dst[x + y * width] = make_uchar4(samples.x, samples.y, samples.z, 0);
        }
    }
}

const int X_BLOCKS = 16;
const int X_THREADS = 16;
const int Y_BLOCKS = 16;
const int Y_THREADS = 16;

int main()
{
    std::string in_file, out_file;
    std::cin >> in_file >> out_file;
    
    int width, height, resWidth, resHeight;
    std::cin >> resWidth >> resHeight;

  /* 
  Считываем данные из .data:
  Первая строка - Ширина и высота
  Далее - значения пикселей в RGBA
  */
    std::ifstream file_stream_in(in_file, std::ios::in | std::ios::binary);
    file_stream_in.read(reinterpret_cast<char*>(&width), sizeof(int));
    file_stream_in.read(reinterpret_cast<char*>(&height), sizeof(int));

    uchar4 *rgba_data = new uchar4[width * height];
    file_stream_in.read(reinterpret_cast<char*>(rgba_data), width * height * sizeof(uchar4));
    file_stream_in.close();

    int widthRatio = width / resWidth;
    int heightRatio = height / resHeight;

    cudaArray *arr;
    cudaChannelFormatDesc channel = cudaCreateChannelDesc<uchar4>();
    cudaMallocArray(&arr, &channel, width, height);
    cudaMemcpy2DToArray(arr, 0, 0, rgba_data, width * sizeof(uchar4), width * sizeof(uchar4), height, cudaMemcpyHostToDevice);

    struct cudaResourceDesc resDesc = {};
    resDesc.resType = cudaResourceTypeArray;
    resDesc.res.array.array = arr;

    struct cudaTextureDesc texDesc = {};
    // Использование крайнего пикселя из-за усреднения
    texDesc.addressMode[0] = cudaAddressModeClamp;
    texDesc.addressMode[1] = cudaAddressModeClamp;
    texDesc.filterMode = cudaFilterModePoint;
    texDesc.readMode = cudaReadModeElementType;
    // Абсолютные значения координат для пикселей
    texDesc.normalizedCoords = false;

    cudaTextureObject_t tex = {};
    cudaCreateTextureObject(&tex, &resDesc, &texDesc, nullptr);

    uchar4 *dev_data;
    cudaMalloc(&dev_data, resWidth * resHeight * sizeof(uchar4));
    kernel<<<dim3(X_BLOCKS, X_THREADS), dim3(Y_BLOCKS, Y_THREADS)>>>(tex, dev_data, resWidth, resHeight, widthRatio, heightRatio);
    cudaDeviceSynchronize();

    cudaMemcpy(rgba_data, dev_data, sizeof(uchar4) * resWidth * resHeight, cudaMemcpyDeviceToHost);

    std::ofstream fsOut(out_file, std::ios::out | std::ios::binary);
    fsOut.write(reinterpret_cast<char*>(&resWidth), sizeof(int));
    fsOut.write(reinterpret_cast<char*>(&resHeight), sizeof(int));
    fsOut.write(reinterpret_cast<char*>(rgba_data), resWidth * resHeight * sizeof(uchar4));
    fsOut.close();

    cudaDestroyTextureObject(tex);
    cudaFreeArray(arr);
    cudaFree(dev_data);
    delete[] rgba_data;
    return 0;
}