NANOPB_GENERATE_CPP(
    TARGET ipc_message  
    ${CMAKE_CURRENT_SOURCE_DIR}/dkdc_ipc.proto
)

target_sources(${CMAKE_PROJECT_NAME} PRIVATE ${CMAKE_CURRENT_SOURCE_DIR}/IpcServer.cpp)