cmake_minimum_required(VERSION 3.27)

# Set the C++ standard.
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Compiler-specific options.
if(MSVC)
    add_compile_options(/bigobj)
elseif(CMAKE_COMPILER_IS_GNUCXX OR CMAKE_CXX_COMPILER_ID STREQUAL "Clang")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -static-libgcc -static-libstdc++")
endif()

# Include directories.
include_directories(.)

# Define the shared library.
add_library(WireSharkLib SHARED
    quick_queue.h
    wiring_wrapper.h
    wiring_wrapper.cpp
    common.h
    logic_gate.h
    pixel_box.h
    interop.h
    impl.cpp
    interop.cpp
)

# Rename the output DLL.
set_target_properties(WireSharkLib PROPERTIES OUTPUT_NAME "libWireSharkLib")

# Define the test executable and link it with WireSharkLib.
add_executable(WireSharkLibTest test.cpp)
target_link_libraries(WireSharkLibTest WireSharkLib)