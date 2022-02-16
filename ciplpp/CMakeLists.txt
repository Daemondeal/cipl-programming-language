cmake_minimum_required(VERSION 3.21)
project(ciplpp)

add_compile_options(-fno-exceptions -fno-rtti)

set(CMAKE_CXX_STANDARD 17)

add_executable(ciplpp main.cpp utils/memory.h utils/common.h utils/vector.h chunk.cpp chunk.h value.cpp value.h debug.cpp debug.h vm.cpp vm.h)
