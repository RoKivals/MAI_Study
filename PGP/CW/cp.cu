#include <cmath>
#include <cstdio>
#include <cstdlib>
#include <cuda_device_runtime_api.h>
#include <cuda_runtime.h>
#include <fstream>
#include <iostream>
#include <string>

namespace tools {
__host__ __device__ float3 sum(const float3 &left, const float3 &right) {
  return {left.x + right.x, left.y + right.y, left.z + right.z};
}

__host__ __device__ float3 multiply(const float3 &vector, float digit) {
  return {vector.x * digit, vector.y * digit, vector.z * digit};
}

__host__ __device__ float3 minus(const float3 &left, const float3 &right) {
  return {left.x - right.x, left.y - right.y, left.z - right.z};
}

__host__ __device__ float scalProduct(const float3 &left, const float3 &right) {
  return left.x * right.x + left.y * right.y + left.z * right.z;
}

__host__ __device__ float3 vectorProduct(const float3 &left,
                                         const float3 &right) {
  return {left.y * right.z - left.z * right.y,
          left.z * right.x - left.x * right.z,
          left.x * right.y - left.y * right.x};
}

__host__ __device__ float vector_length(const float3 &vector) {
  return sqrt(scalProduct(vector, vector));
}

__host__ __device__ float3 normalize(const float3 &vector) {
  auto len = vector_length(vector);
  return {vector.x / len, vector.y / len, vector.z / len};
}

__host__ __device__ float3 toGlobalCoords(const float3 &base_x,
                                          const float3 &base_y,
                                          const float3 &base_z,
                                          const float3 &local) {
  return {base_x.x * local.x + base_y.x * local.y + base_z.x * local.z,
          base_x.y * local.x + base_y.y * local.y + base_z.y * local.z,
          base_x.z * local.x + base_y.z * local.y + base_z.z * local.z};
}

} // namespace tools

std::istream &operator>>(std::istream &in, float3 &vector) {
  in >> vector.x >> vector.y >> vector.z;
  return in;
}

std::ostream &operator<<(std::ostream &out, float3 &vector) {
  out << vector.x << " " << vector.y << " " << vector.z;
  return out;
}

struct Triangle {
  float3 p, q, r;
  uchar4 color;
  __host__ __device__ Triangle() {}
  __host__ __device__ Triangle(const float3 &a, const float3 &b,
                               const float3 &c, uchar4 color) {
    this->p = a;
    this->q = b;
    this->r = c;
    this->color = color;
  }
};

__host__ __device__ uchar4 ray(const float3 &origin, const float3 &dir,
                               const float3 &lightPos, uchar4 lightCol,
                               Triangle *triangles, int numTris) {
  int closest_triangle = -1;
  float tMin = 0.0;

  for (int i(0); i < numTris; ++i) {
    auto edge1 = tools::minus(triangles[i].q, triangles[i].p);
    auto edge2 = tools::minus(triangles[i].r, triangles[i].p);
    auto height = tools::vectorProduct(dir, edge2);
    auto a = tools::scalProduct(edge1, height);

    if (fabs(a) < 1e-10)
      continue;

    auto s = tools::minus(origin, triangles[i].p);
    auto u = tools::scalProduct(s, height) / a;

    if (u < 0.0 || u > 1.0)
      continue;

    auto vvec = tools::vectorProduct(s, edge1);
    auto v = tools::scalProduct(dir, vvec) / a;

    if (v < 0.0 || (u + v) > 1.0)
      continue;

    auto t = tools::scalProduct(edge2, vvec) / a;
    if (t < 0.0)
      continue;

    if (closest_triangle == -1 || t < tMin) {
      closest_triangle = i;
      tMin = t;
    }
  }

  if (closest_triangle == -1)
    return make_uchar4(0, 0, 0, 255);

  auto hitPoint = tools::sum(origin, tools::multiply(dir, tMin));
  auto toLight = tools::minus(lightPos, hitPoint);
  auto distToLight = sqrt(tools::scalProduct(toLight, toLight));

  toLight = tools::normalize(toLight);
  int i = 0;
  for (; i < numTris; ++i) {
    auto e1 = tools::minus(triangles[i].q, triangles[i].p);
    auto e2 = tools::minus(triangles[i].r, triangles[i].p);
    auto height = tools::vectorProduct(toLight, e2);
    auto det = tools::scalProduct(e1, height);

    if (fabs(det) < 1e-10)
      continue;

    auto s = tools::minus(hitPoint, triangles[i].p);
    auto u = tools::scalProduct(s, height) / det;

    if (u < 0.0 || u > 1.0)
      continue;

    auto q = tools::vectorProduct(s, e1);
    auto v = tools::scalProduct(toLight, q) / det;

    if (v < 0.0 || (u + v) > 1.0)
      continue;

    auto tShadow = tools::scalProduct(e2, q) / det;

    if (tShadow > 0.0 && tShadow < distToLight && i != closest_triangle)
      return make_uchar4(0, 0, 0, 255);
  }

  uchar4 base = triangles[closest_triangle].color;
  return make_uchar4(base.x * lightCol.x, base.y * lightCol.y,
                     base.z * lightCol.z, 255);
}

__host__ __device__ void cpuRender(uchar4 *img, const float3 &cameraPoint,
                                   const float3 &cameraDirection, int width,
                                   int height, float fov,
                                   const float3 &lightPos, uchar4 lightCol,
                                   Triangle *triangles, int triCount) {
  auto deltaX = 2.f / (width - 1);
  auto deltaY = 2.f / (height - 1);
  auto planeZ = 1.f / tanf(fov * M_PI / 360.);

  auto forward = tools::normalize(tools::minus(cameraDirection, cameraPoint));
  auto right = tools::normalize(tools::vectorProduct(forward, float3{0, 0, 1}));
  auto up = tools::normalize(tools::vectorProduct(right, forward));

  for (int i(0); i < width; ++i) {
    for (int j(0); j < height; ++j) {
      float3 screenPt{-1 + deltaX * i, (-1 + deltaY * j) * height / width,
                      planeZ};
      auto rayDirection = tools::toGlobalCoords(right, up, forward, screenPt);
      int index = (height - 1 - j) * width + i;
      auto direction = tools::normalize(rayDirection);

      img[index] =
          ray(cameraPoint, direction, lightPos, lightCol, triangles, triCount);
    }
  }
}

__global__ void gpuRender(uchar4 *img, float3 cameraPoint,
                          float3 cameraDirection, int width, int height,
                          float fov, float3 lightPos, uchar4 lightCol,
                          Triangle *triangles, int triCount) {
  int idx_x = blockDim.x * blockIdx.x + threadIdx.x;
  int idx_y = blockDim.y * blockIdx.y + threadIdx.y;
  int offset_x = blockDim.x * gridDim.x;
  int offset_y = blockDim.y * gridDim.y;

  auto deltaX = 2.f / (width - 1);
  auto deltaY = 2.f / (height - 1);
  auto planeZ = 1.f / tanf(fov * M_PI / 360.0);

  auto forward = tools::normalize(tools::minus(cameraDirection, cameraPoint));

  auto right = tools::normalize(tools::vectorProduct(forward, float3{0, 0, 1}));
  auto up = tools::normalize(tools::vectorProduct(right, forward));

  for (int i(idx_x); i < width; i += offset_x) {
    for (int j(idx_y); j < height; j += offset_y) {
      float3 screenPt{-1 + deltaX * i, (-1 + deltaY * j) * height / width,
                      planeZ};

      auto rayDirection = tools::toGlobalCoords(right, up, forward, screenPt);

      int idx = (height - 1 - j) * width + i;

      auto direction = tools::normalize(rayDirection);
      img[idx] =
          ray(cameraPoint, direction, lightPos, lightCol, triangles, triCount);
    }
  }
}

__host__ __device__ void cpuSmoothing(uchar4 *source, uchar4 *destination,
                                      int width, int height, int sppSide) {
  for (int x(0); x < width; ++x) {
    for (int y(0); y < height; ++y) {
      uint4 accumulator = uint4{0, 0, 0, 0};

      for (int i(0); i < sppSide; ++i)
        for (int j(0); j < sppSide; ++j) {
          int idx = width * sppSide * (y * sppSide + j) + (x * sppSide + i);
          uchar4 pix = source[idx];
          accumulator.x += pix.x;
          accumulator.y += pix.y;
          accumulator.z += pix.z;
        }
      int samples = sppSide * sppSide;
      destination[y * width + x] =
          make_uchar4(accumulator.x / samples, accumulator.y / samples,
                      accumulator.z / samples, 255);
    }
  }
}

__global__ void gpuSmoothing(uchar4 *source, uchar4 *destination, int width,
                             int height, int sppSide) {
  int idx_x = blockDim.x * blockIdx.x + threadIdx.x;
  int idx_y = blockDim.y * blockIdx.y + threadIdx.y;
  int offset_x = blockDim.x * gridDim.x;
  int offset_y = blockDim.y * gridDim.y;

  for (int x(idx_x); x < width; x += offset_x) {
    for (int y(idx_y); y < height; y += offset_y) {
      uint4 accumulator = uint4{0, 0, 0, 0};

      for (int i(0); i < sppSide; ++i)
        for (int j(0); j < sppSide; ++j) {
          int idx = width * sppSide * (y * sppSide + j) + (x * sppSide + i);
          uchar4 pixel = source[idx];
          accumulator.x += pixel.x;
          accumulator.y += pixel.y;
          accumulator.z += pixel.z;
        }
      int totSamples = sppSide * sppSide;
      destination[y * width + x] =
          make_uchar4(accumulator.x / totSamples, accumulator.y / totSamples,
                      accumulator.z / totSamples, 255);
    }
  }
}

namespace configs {

struct FrameCfg {
  int cntFrames;
  std::string filePattern;
  int width, height;
  float viewFov;
};

struct CameraCfg {
  float posA, posB, posC, arc, offset, freqA, freqB, phaseA, phaseB, phaseC;
  float targetA, targetB, targetC, targetArc, targetOffset, targetFreqA,
      targetFreqB, targetPhaseA, targetPhaseB, targetPhaseC;
};

struct ShapeCfg {
  float3 center;
  uchar4 color;
  float size;
};

struct FloorCfg {
  float3 v1, v2, v3, v4;
  uchar4 color;
};

struct LightCfg {
  float3 pos;
  uchar4 color;
  int sppSide;
};

std::istream &operator>>(std::istream &in, FrameCfg &config) {
  in >> config.cntFrames >> config.filePattern >> config.width >>
      config.height >> config.viewFov;
  return in;
}

std::istream &operator>>(std::istream &in, CameraCfg &config) {
  in >> config.posA >> config.posB >> config.posC >> config.arc >>
      config.offset >> config.freqA >> config.freqB >> config.phaseA >>
      config.phaseB >> config.phaseC;
  in >> config.targetA >> config.targetB >> config.targetC >>
      config.targetArc >> config.targetOffset >> config.targetFreqA >>
      config.targetFreqB >> config.targetPhaseA >> config.targetPhaseB >>
      config.targetPhaseC;
  return in;
}

std::istream &operator>>(std::istream &in, ShapeCfg &config) {
  float r, g, b;
  in >> config.center;
  in >> r >> g >> b;
  config.color = make_uchar4(r * 255, g * 255, b * 255, 255);
  in >> config.size;
  return in;
}

std::istream &operator>>(std::istream &in, FloorCfg &config) {
  float r, g, b;
  in >> config.v1 >> config.v2 >> config.v3 >> config.v4;
  in >> r >> g >> b;
  config.color = make_uchar4(r * 255, g * 255, b * 255, 255);
  return in;
}

std::istream &operator>>(std::istream &in, LightCfg &config) {
  float r, g, b;
  in >> config.pos;
  in >> r >> g >> b;
  config.color = make_uchar4(r * 255, g * 255, b * 255, 255);
  in >> config.sppSide;
  return in;
}
} // namespace configs

class SceneRenderer {
  bool useGPU;
  // Общее число треугольников: 2 (пол) + 4 (тетраэдер) + 12 (Гексаэдр) + 16
  // (Икосаэдр) = 38
  const int totalTris = 38;
  int gridDimX = 32, blockDimX = 32, gridDimY = 32, blockDimY = 32;

  configs::FrameCfg frameCfg;
  configs::CameraCfg camCfg;
  // Порядок: Тетраэдр, Гексаэдр, Икосаэдр
  configs::ShapeCfg tetraidCfg, geksaidrCfg, icosaidrCfg;
  configs::FloorCfg floorCfg;
  configs::LightCfg lightCfg;

  void buildFloor(Triangle *triList) {
    triList[0] =
        Triangle(floorCfg.v1, floorCfg.v2, floorCfg.v3, floorCfg.color);
    triList[1] =
        Triangle(floorCfg.v1, floorCfg.v3, floorCfg.v4, floorCfg.color);
  }

  // Создание Тэтраэдра
  void buildTetraid(Triangle *triList) {
    float L = tetraidCfg.size / sqrt(3.0);
    float3 c = tetraidCfg.center;
    float3 v0{c.x + L, c.y + L, c.z + L};
    float3 v1{c.x + L, c.y - L, c.z - L};
    float3 v2{c.x - L, c.y + L, c.z + L};
    float3 v3{c.x - L, c.y - L, c.z + L};

    int idx = 2;
    triList[idx++] = Triangle(v0, v1, v3, tetraidCfg.color);
    triList[idx++] = Triangle(v0, v2, v3, tetraidCfg.color);
    triList[idx++] = Triangle(v1, v2, v3, tetraidCfg.color);
    triList[idx++] = Triangle(v0, v1, v2, tetraidCfg.color);
  }

  // Создание Гексаэдра
  void buildGeksaidr(Triangle *triList) {
    float R = geksaidrCfg.size / sqrt(3.0);

    float3 c = geksaidrCfg.center;
    float3 v1{c.x - R, c.y - R, c.z - R};
    float3 v2{c.x - R, c.y - R, c.z + R};
    float3 v3{c.x - R, c.y + R, c.z - R};
    float3 v4{c.x - R, c.y + R, c.z + R};
    float3 v5{c.x + R, c.y - R, c.z - R};
    float3 v6{c.x + R, c.y - R, c.z + R};
    float3 v7{c.x + R, c.y + R, c.z - R};
    float3 v8{c.x + R, c.y + R, c.z + R};
    int idx = 6;
    triList[idx++] = Triangle(v1, v2, v4, geksaidrCfg.color);
    triList[idx++] = Triangle(v1, v3, v4, geksaidrCfg.color);
    triList[idx++] = Triangle(v2, v6, v8, geksaidrCfg.color);
    triList[idx++] = Triangle(v2, v4, v8, geksaidrCfg.color);
    triList[idx++] = Triangle(v5, v6, v8, geksaidrCfg.color);
    triList[idx++] = Triangle(v5, v7, v8, geksaidrCfg.color);
    triList[idx++] = Triangle(v1, v5, v7, geksaidrCfg.color);
    triList[idx++] = Triangle(v1, v3, v7, geksaidrCfg.color);
    triList[idx++] = Triangle(v1, v2, v6, geksaidrCfg.color);
    triList[idx++] = Triangle(v1, v5, v6, geksaidrCfg.color);
    triList[idx++] = Triangle(v3, v4, v8, geksaidrCfg.color);
    triList[idx++] = Triangle(v3, v7, v8, geksaidrCfg.color);
  }

  // Создание Икосаэдр
  void buildIcosaidr(Triangle *triList) {
    float phi = (1.0 + sqrt(5.0)) / 2.0;
    float R = icosaidrCfg.size / sqrt(3.0);
    float3 c = icosaidrCfg.center;
    float3 v1{c.x, c.y - R, c.z + R * phi};
    float3 v2{c.x, c.y + R, c.z + R * phi};
    float3 v3{c.x - R * phi, c.y, c.z + R};
    float3 v4{c.x + R * phi, c.y, c.z + R};
    float3 v5{c.x - R * phi, c.y + R * phi, c.z};
    float3 v6{c.x + R, c.y + R * phi, c.z};
    float3 v7{c.x + R, c.y - R * phi, c.z};
    float3 v8{c.x - R, c.y - R * phi, c.z};
    float3 v9{c.x - R * phi, c.y, c.z - R};
    float3 v10{c.x + R * phi, c.y, c.z - R};
    float3 v11{c.x, c.y - R, c.z - R * phi};
    float3 v12{c.x, c.y + R, c.z - R * phi};

    int idx = 18;
    triList[idx++] = Triangle(v1, v2, v3, icosaidrCfg.color);
    triList[idx++] = Triangle(v2, v1, v4, icosaidrCfg.color);
    triList[idx++] = Triangle(v1, v3, v8, icosaidrCfg.color);
    triList[idx++] = Triangle(v3, v2, v5, icosaidrCfg.color);
    triList[idx++] = Triangle(v5, v2, v6, icosaidrCfg.color);
    triList[idx++] = Triangle(v7, v1, v8, icosaidrCfg.color);
    triList[idx++] = Triangle(v4, v1, v7, icosaidrCfg.color);
    triList[idx++] = Triangle(v2, v4, v6, icosaidrCfg.color);
    triList[idx++] = Triangle(v5, v6, v12, icosaidrCfg.color);
    triList[idx++] = Triangle(v7, v8, v11, icosaidrCfg.color);
    triList[idx++] = Triangle(v4, v7, v10, icosaidrCfg.color);
    triList[idx++] = Triangle(v6, v4, v10, icosaidrCfg.color);
    triList[idx++] = Triangle(v8, v3, v9, icosaidrCfg.color);
    triList[idx++] = Triangle(v3, v5, v9, icosaidrCfg.color);
    triList[idx++] = Triangle(v10, v11, v12, icosaidrCfg.color);
    triList[idx++] = Triangle(v11, v9, v12, icosaidrCfg.color);
    triList[idx++] = Triangle(v6, v10, v12, icosaidrCfg.color);
    triList[idx++] = Triangle(v10, v7, v11, icosaidrCfg.color);
    triList[idx++] = Triangle(v8, v9, v11, icosaidrCfg.color);
    triList[idx++] = Triangle(v9, v5, v12, icosaidrCfg.color);
  }

public:
  SceneRenderer(bool gpuOn) {
    useGPU = gpuOn;
    frameCfg = {360, "res/%d.data", 800, 800, 90.0};
    camCfg = {7, 3, 0, 2, 1, 2, 6, 1, 0, 5, 2, 0, 3, 0.5, 0.1, 2, 4, 1, 0, 0};

    tetraidCfg = {float3{0, -2, 0}, make_uchar4(1, 1, 100, 255), 1};
    geksaidrCfg = {float3{0, 0, 0}, make_uchar4(255, 255, 0, 255), 1};
    icosaidrCfg = {float3{0, 2, 0}, make_uchar4(255, 255, 255, 255), 1};

    floorCfg = {float3{-5, -5, -1}, float3{-5, 5, -1}, float3{5, 5, -1},
                float3{5, -5, -1}, make_uchar4(255, 0, 0, 255)};
    lightCfg = {float3{10, 0, 15}, make_uchar4(75, 50, 25, 255), 4};
  }

  SceneRenderer(bool gpuOn, const configs::FrameCfg &frame_config,
                const configs::CameraCfg &cam_config,
                const configs::ShapeCfg &tetraid,
                const configs::ShapeCfg &geksaidr,
                const configs::ShapeCfg &icosaidr, const configs::FloorCfg &fl,
                const configs::LightCfg &light_config)
      : useGPU(gpuOn), frameCfg(frame_config), camCfg(cam_config),
        tetraidCfg(tetraid), geksaidrCfg(geksaidr), icosaidrCfg(icosaidr),
        floorCfg(fl), lightCfg(light_config) {}

  void render() {
    int spp = lightCfg.sppSide * lightCfg.sppSide;
    int totalPix = frameCfg.width * frameCfg.height * spp;
    uchar4 *rawImg = (uchar4 *)malloc(sizeof(uchar4) * totalPix);
    uchar4 *finalImg =
        (uchar4 *)malloc(sizeof(uchar4) * frameCfg.width * frameCfg.height);
    uchar4 *d_raw = nullptr, *d_final = nullptr;
    Triangle sceneTris[totalTris];
    Triangle *d_scene = nullptr;

    buildFloor(sceneTris);
    buildTetraid(sceneTris);
    buildGeksaidr(sceneTris);
    buildIcosaidr(sceneTris);

    if (useGPU) {
      cudaMalloc(&d_raw, sizeof(uchar4) * totalPix);
      cudaMalloc(&d_final, sizeof(uchar4) * frameCfg.width * frameCfg.height);
      cudaMalloc(&d_scene, sizeof(Triangle) * totalTris);
      cudaMemcpy(d_scene, sceneTris, sizeof(Triangle) * totalTris,
                 cudaMemcpyHostToDevice);
    }

    for (int frame(0); frame < frameCfg.cntFrames; ++frame) {
      auto t = 2 * M_PI * frame / frameCfg.cntFrames;
      float3 cameraPoint{
          (camCfg.posA + camCfg.arc * sinf(camCfg.freqA * t + camCfg.phaseA)) *
              cosf(camCfg.posC + camCfg.phaseC * t),
          (camCfg.posA + camCfg.arc * sinf(camCfg.freqA * t + camCfg.phaseA)) *
              sinf(camCfg.posC + camCfg.phaseC * t),
          camCfg.posB + camCfg.offset * sinf(camCfg.freqB * t + camCfg.phaseB),
      };

      float3 camTarget{
          (camCfg.targetA + camCfg.targetArc * sinf(camCfg.targetFreqA * t +
                                                    camCfg.targetPhaseA)) *
              cosf(camCfg.targetC + camCfg.targetPhaseC * t),
          (camCfg.targetA + camCfg.targetArc * sinf(camCfg.targetFreqA * t +
                                                    camCfg.targetPhaseA)) *
              sinf(camCfg.targetC + camCfg.targetPhaseC * t),
          camCfg.targetB + camCfg.targetOffset * sinf(camCfg.targetFreqB * t +
                                                      camCfg.targetPhaseB),
      };

      cudaEvent_t startEvent, endEvent;
      cudaEventCreate(&startEvent);
      cudaEventCreate(&endEvent);
      cudaEventRecord(startEvent);

      if (useGPU) {
        dim3 grid(gridDimX, blockDimX);
        dim3 block(gridDimY, blockDimY);
        gpuRender<<<grid, block>>>(
            d_raw, cameraPoint, camTarget, frameCfg.width * lightCfg.sppSide,
            frameCfg.height * lightCfg.sppSide, frameCfg.viewFov, lightCfg.pos,
            lightCfg.color, d_scene, totalTris);

        gpuSmoothing<<<grid, block>>>(d_raw, d_final, frameCfg.width,
                                      frameCfg.height, lightCfg.sppSide);
        cudaMemcpy(finalImg, d_final,
                   sizeof(uchar4) * frameCfg.width * frameCfg.height,
                   cudaMemcpyDeviceToHost);
      } else {
        cpuRender(rawImg, cameraPoint, camTarget,
                  frameCfg.width * lightCfg.sppSide,
                  frameCfg.height * lightCfg.sppSide, frameCfg.viewFov,
                  lightCfg.pos, lightCfg.color, sceneTris, totalTris);
        cpuSmoothing(rawImg, finalImg, frameCfg.width, frameCfg.height,
                     lightCfg.sppSide);
      }

      cudaEventRecord(endEvent);
      cudaEventSynchronize(endEvent);
      cudaEventDestroy(startEvent);
      cudaEventDestroy(endEvent);

      char outName[frameCfg.filePattern.length() +
                   std::to_string(frame).length() - 2];
      sprintf(outName, frameCfg.filePattern.c_str(), frame);

      std::ofstream fsOut(outName, std::ios::out | std::ios::binary);
      fsOut.write(reinterpret_cast<char *>(&frameCfg.width), sizeof(int));
      fsOut.write(reinterpret_cast<char *>(&frameCfg.height), sizeof(int));
      fsOut.write(reinterpret_cast<char *>(finalImg),
                  frameCfg.height * frameCfg.width * sizeof(uchar4));
      fsOut.close();
      std::cout << "MADE FRAME: " << frame << std::endl;
      // FILE *fOut = fopen(outName, "wb");
      // fwrite(&frameCfg.width, sizeof(int), 1, fOut);
      // fwrite(&frameCfg.height, sizeof(int), 1, fOut);
      // fwrite(finalImg, sizeof(uchar4), frameCfg.width * frameCfg.height, fOut);
      // fclose(fOut);
    }

    free(rawImg);
    free(finalImg);
    if (useGPU) {
      cudaFree(d_raw);
      cudaFree(d_final);
      cudaFree(d_scene);
    }
  }

  friend std::ostream &operator<<(std::ostream &os, SceneRenderer &sr) {
    os << sr.frameCfg.cntFrames << "\n"
       << sr.frameCfg.filePattern << "\n"
       << sr.frameCfg.width << " " << sr.frameCfg.height << " "
       << sr.frameCfg.viewFov << "\n";
    os << sr.camCfg.posA << " " << sr.camCfg.posB << " " << sr.camCfg.posC
       << " " << sr.camCfg.arc << " " << sr.camCfg.offset << " "
       << sr.camCfg.freqA << " " << sr.camCfg.freqB << " " << sr.camCfg.phaseA
       << " " << sr.camCfg.phaseB << " " << sr.camCfg.phaseC << "\n";
    os << sr.camCfg.targetA << " " << sr.camCfg.targetB << " "
       << sr.camCfg.targetC << " " << sr.camCfg.targetArc << " "
       << sr.camCfg.targetOffset << " " << sr.camCfg.targetFreqA << " "
       << sr.camCfg.targetFreqB << " " << sr.camCfg.targetPhaseA << " "
       << sr.camCfg.targetPhaseB << " " << sr.camCfg.targetPhaseC << "\n";

    os << sr.tetraidCfg.center << " " << int(sr.tetraidCfg.color.x) / 255.0
       << " " << int(sr.tetraidCfg.color.y) / 255.0 << " "
       << int(sr.tetraidCfg.color.z) / 255.0 << " " << sr.tetraidCfg.size
       << "\n";
    os << sr.geksaidrCfg.center << " " << int(sr.geksaidrCfg.color.x) / 255.0
       << " " << int(sr.geksaidrCfg.color.y) / 255.0 << " "
       << int(sr.geksaidrCfg.color.z) / 255.0 << " " << sr.geksaidrCfg.size
       << "\n";
    os << sr.icosaidrCfg.center << " " << int(sr.icosaidrCfg.color.x) / 255.0
       << " " << int(sr.icosaidrCfg.color.y) / 255.0 << " "
       << int(sr.icosaidrCfg.color.z) / 255.0 << " " << sr.icosaidrCfg.size
       << "\n";
    os << sr.floorCfg.v1 << " " << sr.floorCfg.v2 << " " << sr.floorCfg.v3
       << " " << sr.floorCfg.v4 << " " << int(sr.floorCfg.color.x) / 255.0
       << " " << int(sr.floorCfg.color.y) / 255.0 << " "
       << int(sr.floorCfg.color.z) / 255.0 << "\n";
    os << sr.lightCfg.pos << " " << int(sr.lightCfg.color.x) / 255.0 << " "
       << int(sr.lightCfg.color.y) / 255.0 << " "
       << int(sr.lightCfg.color.z) / 255.0 << " " << sr.lightCfg.sppSide;
    return os;
  }
};

int main(int argc, char **argv) {
  bool cpuMode = false, gpuMode = false, defMode = false;
  for (int i = 1; i < argc; ++i) {
    std::string arg = argv[i];
    if (arg == "--cpu")
      cpuMode = true;
    else if (arg == "--gpu")
      gpuMode = true;
    else if (arg == "--default")
      defMode = true;
  }
  bool gpuEnabled = true;
  if (cpuMode && !gpuMode)
    gpuEnabled = false;

  if (defMode) {
    SceneRenderer renderer(gpuEnabled);
    std::cout << renderer << std::endl;
  } else {
    configs::FrameCfg frameCfg;
    configs::CameraCfg cameraCfg;
    configs::ShapeCfg tetraidCfg, geksaidrCfg, icosaidrCfg;
    configs::FloorCfg floorCfg;
    configs::LightCfg lightCfg;

    std::cin >> frameCfg;
    std::cin >> cameraCfg;
    std::cin >> tetraidCfg;
    std::cin >> geksaidrCfg;
    std::cin >> icosaidrCfg;
    std::cin >> floorCfg;
    std::cin >> lightCfg;

    SceneRenderer renderer(gpuEnabled, frameCfg, cameraCfg, tetraidCfg,
                           geksaidrCfg, icosaidrCfg, floorCfg, lightCfg);
    renderer.render();
  }
  return 0;
}
