cmake_minimum_required(VERSION 3.16)
project(dkdc_backend)


set(CMAKE_MODULE_PATH ${CMAKE_CURRENT_SOURCE_DIR}/lib/nanopb/extra)
find_package(Nanopb REQUIRED)

add_executable(${CMAKE_PROJECT_NAME})

add_subdirectory(src)

target_link_libraries(${CMAKE_PROJECT_NAME}
    PRIVATE
        ipc_message
)

# build msvc static runtime
if (MSVC)
    set_property(TARGET ${CMAKE_PROJECT_NAME} PROPERTY MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")
endif()
