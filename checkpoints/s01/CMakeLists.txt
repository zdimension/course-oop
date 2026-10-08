cmake_minimum_required(VERSION 3.20)
project(checkpoint_s01 LANGUAGES CXX)

add_library(resistor src/resistor.cpp)
target_include_directories(resistor PUBLIC include)
target_compile_features(resistor PUBLIC cxx_std_20)

if(CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang")
    target_compile_options(resistor PRIVATE -Wall -Wextra -Wpedantic)
endif()

add_executable(s01_demo main.cpp)
target_link_libraries(s01_demo PRIVATE resistor)

enable_testing()
add_executable(s01_tests tests.cpp)
target_link_libraries(s01_tests PRIVATE resistor)
add_test(NAME s01_tests COMMAND s01_tests)
