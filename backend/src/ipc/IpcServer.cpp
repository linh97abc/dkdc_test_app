#include "IpcServer.hpp"

#include <iostream>

IpcServer::IpcServer()
{
    this->namePipeServer.RegisterDataReceivedCallback(this);
}

void IpcServer::Start()
{
    try
    {
        this->namePipeServer.Start();
    }
    catch (const std::exception &e)
    {
        std::cerr << "Error starting IPC server: " << e.what() << std::endl;
    }
}

void IpcServer::OnDataReceived(const void *data, size_t size)
{
    dkdc_ipc_FrontendToBackendMessage message = dkdc_ipc_FrontendToBackendMessage_init_zero;
    pb_istream_t stream = pb_istream_from_buffer(static_cast<const uint8_t *>(data), size);

    if (!pb_decode(&stream, dkdc_ipc_FrontendToBackendMessage_fields, &message))
    {
        std::cerr << "Failed to decode message: " << PB_GET_ERROR(&stream) << std::endl;
        return;
    }

    if (this->messageHandler)
    {
        this->messageHandler(message);
    }
}

void IpcServer::SendMessage(const dkdc_ipc_BackendToFrontendMessage &message)
{
    pb_byte_t buffer[DKDC_IPC_DKDC_IPC_PB_H_MAX_SIZE];
    pb_ostream_t stream = pb_ostream_from_buffer(buffer, sizeof(buffer));

    if (!pb_encode(&stream, dkdc_ipc_BackendToFrontendMessage_fields, &message))
    {
        std::cerr << "Failed to encode message: " << PB_GET_ERROR(&stream) << std::endl;
        return;
    }

    this->namePipeServer.Send(buffer, stream.bytes_written);
}

void IpcServer::NotifySerialPortClosed()
{
    dkdc_ipc_BackendToFrontendMessage message = dkdc_ipc_BackendToFrontendMessage_init_zero;
    message.which_cmd = dkdc_ipc_BackendToFrontendMessage_closePort_tag;
    message.cmd.closePort = dkdc_ipc_ClosePort_init_zero;

    this->SendMessage(message);
}

void IpcServer::StartQueryInputData(uint32_t inputId)
{
    dkdc_ipc_BackendToFrontendMessage message = dkdc_ipc_BackendToFrontendMessage_init_zero;
    message.which_cmd = dkdc_ipc_BackendToFrontendMessage_queryInput_tag;
    message.cmd.queryInput.inputId = inputId;

    this->SendMessage(message);
}

void IpcServer::PushLogData(const std::string &logData)
{
    dkdc_ipc_BackendToFrontendMessage message = dkdc_ipc_BackendToFrontendMessage_init_zero;
    message.which_cmd = dkdc_ipc_BackendToFrontendMessage_logData_tag;

    memcpy_s(
        message.cmd.logData.log,
        sizeof(message.cmd.logData.log),
        logData.c_str(),
        std::min(logData.size(), sizeof(message.cmd.logData.log) - 1));

    this->SendMessage(message);
}

void IpcServer::PushChartData(const std::vector<float> &chartData)
{
    dkdc_ipc_BackendToFrontendMessage message = dkdc_ipc_BackendToFrontendMessage_init_zero;
    message.which_cmd = dkdc_ipc_BackendToFrontendMessage_chartData_tag;
    message.cmd.chartData.data_count = chartData.size();

    std::copy(chartData.begin(), chartData.end(), message.cmd.chartData.data);

    this->SendMessage(message);
}

void IpcServer::UpdateInputField(uint32_t inputId, double value)
{
    dkdc_ipc_BackendToFrontendMessage message = dkdc_ipc_BackendToFrontendMessage_init_zero;
    message.which_cmd = dkdc_ipc_BackendToFrontendMessage_inputData_tag;
    message.cmd.inputData.inputId = inputId;
    message.cmd.inputData.value = value;

    this->SendMessage(message);
}

IpcServer::~IpcServer()
{
    this->namePipeServer.Stop();
}