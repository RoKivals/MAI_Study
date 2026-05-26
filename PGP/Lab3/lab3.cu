#include <iostream>
#include <vector>
#include <cmath>
#include <fstream>

__constant__ double AVG[32][3];
__constant__ double INVERSE_COV[32][3][3];
__constant__ double DET[32];

__device__ double calculate_truth(uchar4 pixel, int sample_id)
{
    double res = 0.0;
    double diff[3] = {};
    double transposed[3] = {};

    diff[0] = pixel.x - AVG[sample_id][0];
    diff[1] = pixel.y - AVG[sample_id][1];
    diff[2] = pixel.z - AVG[sample_id][2];
    for (int i = 0; i < 3; ++i)
    {
        for (int j = 0; j < 3; ++j) {
            transposed[i] += -diff[j] * INVERSE_COV[sample_id][j][i];
        }  
        res += transposed[i] * diff[i];
    }
    res -= std::log(std::abs(DET[sample_id]));
    return res;
}

__device__ int classify(uchar4 pixel, int classes_cnt)
{
    int num_class = 0;
    double max_elem = calculate_truth(pixel, num_class);
    for (int i = 1; i < classes_cnt; ++i)
    {
        double elem = calculate_truth(pixel, i);
        if (elem > max_elem)
        {
            max_elem = elem;
            num_class = i;
        }
    }
    return num_class;
}

__global__ void kernel(uchar4 *ker_data, int width, int height, int classes_cnt)
{
    int idx = blockIdx.x * blockDim.x + threadIdx.x;
	int offset = blockDim.x * gridDim.x;

    while (idx < width * height)
    {
        ker_data[idx].w = classify(ker_data[idx], classes_cnt);
        idx += offset;
    }
}

const int BLOCKS = 32;
const int THREADS = 32;
// const int MATRIX_SIZE = 3;

int main()
{
    std::string in_file, out_file;
    std::cin >> in_file >> out_file;

    int width, height, classes_cnt;
    std::cin >> classes_cnt;

    std::vector<std::vector<int2>> samples_coords;
    samples_coords.resize(classes_cnt);
    
    std::ifstream file_stream_in(in_file, std::ios::in | std::ios::binary);
    file_stream_in.read(reinterpret_cast<char*>(&width), sizeof(int));
    file_stream_in.read(reinterpret_cast<char*>(&height), sizeof(int));

    uchar4 *rgba_data = new uchar4[width * height];
    file_stream_in.read(reinterpret_cast<char*>(rgba_data), width * height * sizeof(uchar4));
    file_stream_in.close();

    for (int i(0); i < classes_cnt; ++i) {
        int pixels_cnt;
        std::cin >> pixels_cnt;
        samples_coords[i].resize(pixels_cnt);
        for (int j(0); j < pixels_cnt; ++j)
            std::cin >> samples_coords[i][j].x >> samples_coords[i][j].y;
    }

    double avg[32][3] = {};
    double cov[32][3][3] = {};
    double inverse_cov[32][3][3] = {};
    
    for (int i(0); i < classes_cnt; ++i) {
        int pixels_cnt = samples_coords[i].size();
        for (int j(0); j < pixels_cnt; ++j) {
            int x = samples_coords[i][j].x;
            int y = samples_coords[i][j].y;
            uchar4 curr_pixel = rgba_data[x + y * width];
            avg[i][0] += curr_pixel.x;
            avg[i][1] += curr_pixel.y;
            avg[i][2] += curr_pixel.z;
        }
        for (int k(0); k < 3; ++k) avg[i][k] /= pixels_cnt;      
    }

    for (int i(0); i < classes_cnt; ++i) {
        int pixels_cnt =  samples_coords[i].size();
        for (int j = 0; j < pixels_cnt; ++j) {
            double diff[3];
            int x = samples_coords[i][j].x;
            int y = samples_coords[i][j].y;
            uchar4 curr_pixel = rgba_data[x + y * width];
            diff[0] = curr_pixel.x - avg[i][0];
            diff[1] = curr_pixel.y - avg[i][1];
            diff[2] = curr_pixel.z - avg[i][2];

            for (int k(0); k < 3; ++k) {
                for (int m(0); m < 3; ++m) {
                    cov[i][k][m] += diff[k] * diff[m];
                }
            }
        }

        for (int j(0); j < 3; ++j) {
            for (int k(0); k < 3; ++k) {
                cov[i][j][k] /= pixels_cnt - 1;
            }
        }
    }

    double Det[32];
    for (int i = 0; i < classes_cnt; ++i)
    {
        double det = 0;
        for (int j = 0; j < 3; ++j)
            det += cov[i][0][j] * (cov[i][1][(j + 1) % 3] * cov[i][2][(j + 2) % 3] - cov[i][1][(j + 2) % 3] * cov[i][2][(j + 1) % 3]);
        inverse_cov[i][0][0] = (cov[i][1][1] * cov[i][2][2] - cov[i][2][1] * cov[i][1][2]) / det;
        inverse_cov[i][0][1] = (cov[i][0][2] * cov[i][2][1] - cov[i][0][1] * cov[i][2][2]) / det;
        inverse_cov[i][0][2] = (cov[i][0][1] * cov[i][1][2] - cov[i][0][2] * cov[i][1][1]) / det;
        inverse_cov[i][1][0] = (cov[i][1][2] * cov[i][2][0] - cov[i][1][0] * cov[i][2][2]) / det;
        inverse_cov[i][1][1] = (cov[i][0][0] * cov[i][2][2] - cov[i][0][2] * cov[i][2][0]) / det;
        inverse_cov[i][1][2] = (cov[i][1][0] * cov[i][0][2] - cov[i][0][0] * cov[i][1][2]) / det;
        inverse_cov[i][2][0] = (cov[i][1][0] * cov[i][2][1] - cov[i][2][0] * cov[i][1][1]) / det;
        inverse_cov[i][2][1] = (cov[i][2][0] * cov[i][0][1] - cov[i][0][0] * cov[i][2][1]) / det;
        inverse_cov[i][2][2] = (cov[i][0][0] * cov[i][1][1] - cov[i][1][0] * cov[i][0][1]) / det;
        Det[i] = det;
    }

    cudaMemcpyToSymbol(AVG, avg, sizeof(double) * 32 * 3);
    cudaMemcpyToSymbol(INVERSE_COV, inverse_cov, sizeof(double) * 32 * 3 * 3);
    cudaMemcpyToSymbol(DET, Det, sizeof(double) * 32);

    uchar4 *ker_data;
    cudaMalloc(&ker_data, sizeof(uchar4) * height * width);
    cudaMemcpy(ker_data, rgba_data, sizeof(uchar4) * height * width, cudaMemcpyHostToDevice);
    kernel<<<BLOCKS, THREADS>>>(ker_data, width, height, classes_cnt);
    cudaMemcpy(rgba_data, ker_data, sizeof(uchar4) * height * width, cudaMemcpyDeviceToHost);

    std::ofstream file_stream_out(out_file, std::ios::out | std::ios::binary);
    file_stream_out.write(reinterpret_cast<char*>(&width), sizeof(int));
    file_stream_out.write(reinterpret_cast<char*>(&height), sizeof(int));
    file_stream_out.write(reinterpret_cast<char*>(rgba_data), width * height * sizeof(uchar4));
    file_stream_out.close();

    cudaFree(ker_data);
    delete[] rgba_data;
    return 0;
}