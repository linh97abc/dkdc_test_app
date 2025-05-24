#pragma once
#include <thread>
#include <cstdint>

class NamePipeServerHandler
{

public:
    virtual void OnDataReceived(const void *data, size_t size) = 0;
    virtual ~NamePipeServerHandler() = default;
};

class NamePipeServer
{
    NamePipeServerHandler *dataReceivedCallback{nullptr};
    void *hPipe{nullptr};
    std::thread *recvThread{nullptr};

    void TaskRecv();

public:
    NamePipeServer();
    ~NamePipeServer();
    void Start();
    void Stop();
    void Send(const void *data, uint32_t size);

    void RegisterDataReceivedCallback(NamePipeServerHandler *handler)
    {
        this->dataReceivedCallback = handler;
    }
};