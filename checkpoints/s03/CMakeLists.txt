cmake_minimum_required(VERSION 3.20)
project(checkpoint_s03 LANGUAGES CXX)

add_library(components
    src/component.cpp
    src/resistor.cpp
    src/capacitor.cpp
    src/inductor.cpp
    src/series.cpp
    src/parallel.cpp
)
target_include_directories(components PUBLIC include)
target_compile_features(components PUBLIC cxx_std_20)

if(CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang")
    target_compile_options(components PRIVATE -Wall -Wextra -Wpedantic)
endif()

add_executable(s03_demo main.cpp)
target_link_libraries(s03_demo PRIVATE components)

enable_testing()
add_executable(s03_tests tests.cpp)
target_link_libraries(s03_tests PRIVATE components)
add_test(NAME s03_tests COMMAND s03_tests)
