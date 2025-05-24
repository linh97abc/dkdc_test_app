#pragma once

#include <pb.h>
#include <pb_encode.h>
#include <pb_decode.h>
#include <dkdc_ipc.pb.h>

#include <string>
#include <vector>
#include <cstdint>
#include <functional>

#include <drivers/NamePipeServer.hpp>

class IpcServer : NamePipeServerHandler
{
    std::function<void(const dkdc_ipc_FrontendToBackendMessage &)> messageHandler;
    NamePipeServer namePipeServer;

    virtual void OnDataReceived(const void *data, size_t size) override;
public:
    IpcServer();
    ~IpcServer();

    void Start();

    void SendMessage(const dkdc_ipc_BackendToFrontendMessage &message);

    void NotifySerialPortClosed();
    void StartQueryInputData(uint32_t inputId);
    void PushLogData(const std::string &logData);
    void PushChartData(const std::vector<float> &chartData);
    void UpdateInputField(uint32_t inputId, double value);


    void RegisterMessageHandler(std::function<void(const dkdc_ipc_FrontendToBackendMessage &)> handler)
    {
        this->messageHandler = handler;
    }
};
