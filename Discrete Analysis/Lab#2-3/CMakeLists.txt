cmake_minimum_required(VERSION 3.24)
project(DiskretAnalysis)


set(GCC_COVERAGE_COMPILE_FLAGS "-fexceptions -fstack-protector-strong –param=ssp-buffer-size=4")
set(CMAKE_CXX_STANDARD 20)

add_executable(DiskretAnalysis
        ../KP/main.cpp)
