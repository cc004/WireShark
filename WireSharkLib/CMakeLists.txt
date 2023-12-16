cmake_minimum_required(VERSION 3.27)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -static")

include_directories(.)

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
target_link_libraries(WireSharkLib -static-libgcc -static-libstdc++)