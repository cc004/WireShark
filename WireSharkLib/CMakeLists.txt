cmake_minimum_required(VERSION 3.27)
project(WireSharkLib)

set(CMAKE_CXX_STANDARD 20)

# Detect compiler.
if (MSVC)
    message(STATUS "Using MSVC")
	add_compile_options(/bigobj)                  # Increases the number of sections.
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /MT") # Static runtime.
    set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS ON)      # Export all symbols in the DLL.
elseif (CMAKE_CXX_COMPILER_ID STREQUAL "GNU")
    message(STATUS "Using MinGW/GCC")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -static")
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

# Define the test executables libraries.
add_executable(WireSharkLibTest
    quick_queue.h
    wiring_wrapper.h
    wiring_wrapper.cpp
    common.h
    logic_gate.h
    pixel_box.h
    interop.h
    impl.cpp
    interop.cpp
    test.cpp
)

# Rename the output DLL.
if(MSVC)
    set_target_properties(WireSharkLib PROPERTIES OUTPUT_NAME "libWireSharkLib")
endif()

# Link the library correctly depending on the compiler.
if (MSVC)
    target_link_libraries(WireSharkLib)
elseif (CMAKE_CXX_COMPILER_ID STREQUAL "GNU")
    target_link_libraries(WireSharkLib -static-libgcc -static-libstdc++)
endif()
