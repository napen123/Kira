cmake_minimum_required(VERSION 3.8)
project(KiraVM)

set(CMAKE_CXX_STANDARD 14)

set(SOURCE_FILES main.cpp vm.h vm.cpp utf8.h utf8.cpp stack.h stack.cpp error.h error.cpp internal.h internal.cpp)
add_executable(KiraVM ${SOURCE_FILES})
