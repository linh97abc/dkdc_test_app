#include "NamePipeServer.hpp"

#include <windows.h>
#include <iostream>
#include <thread>

NamePipeServer::NamePipeServer()
{
}

NamePipeServer::~NamePipeServer()
{
    if (this->recvThread != nullptr)
    {
        this->recvThread->join();
        delete this->recvThread;
        this->recvThread = nullptr;
    }

    if (this->hPipe != nullptr)
    {
        CloseHandle(this->hPipe);
        this->hPipe = nullptr;
    }
}

void NamePipeServer::Start()
{

    this->hPipe = CreateNamedPipe(
        TEXT("\\\\.\\pipe\\vtacTestIPC"),                      // Pipe name
        PIPE_ACCESS_DUPLEX | FILE_FLAG_OVERLAPPED,             // Read/Write access
        PIPE_TYPE_MESSAGE | PIPE_READMODE_MESSAGE | PIPE_WAIT, // Message type pipe
        1,                                                     // Max instances
        1024,                                                  // Output buffer size
        1024,                                                  // Input buffer size
        0,                                                     // Default timeout
        nullptr                                                // Default security attributes
    );

    if (this->hPipe == INVALID_HANDLE_VALUE)
    {
        std::cerr << "Failed to create named pipe. Error: " << GetLastError() << std::endl;
        throw std::runtime_error("Failed to create named pipe");
    }

    std::cout << "Named pipe created successfully." << std::endl;

    // Wait for a client to connect
    std::cout << "Waiting for client to connect..." << std::endl;
    if (!ConnectNamedPipe(this->hPipe, nullptr))
    {
        std::cerr << "Failed to connect named pipe. Error: " << GetLastError() << std::endl;
        CloseHandle(this->hPipe);
        throw std::runtime_error("Failed to connect named pipe");
    }

    std::cout << "Client connected to named pipe." << std::endl;
    this->recvThread = new std::thread(&NamePipeServer::TaskRecv, this);
}

void NamePipeServer::Stop()
{
    if (this->hPipe != nullptr)
    {
        if (!DisconnectNamedPipe(this->hPipe))
        {
            std::cerr << "Failed to disconnect named pipe. Error: " << GetLastError() << std::endl;
        }
        CloseHandle(this->hPipe);
        this->hPipe = nullptr;
        std::cout << "Named pipe disconnected and closed." << std::endl;
    }
    else
    {
        // std::cerr << "No named pipe to close." << std::endl;
    }
}

void NamePipeServer::TaskRecv()
{
    if (this->hPipe == nullptr)
    {
        std::cerr << "Named pipe is not initialized." << std::endl;
        return;
    }

    char buffer[4096];
    DWORD bytesRead;

    while (true)
    {

        OVERLAPPED overlapped = {};
        overlapped.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

        BOOL result = ReadFile(
            this->hPipe,
            buffer,
            sizeof(buffer),
            &bytesRead,
            &overlapped);

        if (result || GetLastError() == ERROR_IO_PENDING)
        {
            // Wait for the read operation to complete
            if (WaitForSingleObject(overlapped.hEvent, INFINITE) == WAIT_OBJECT_0)
            {
                if (!GetOverlappedResult(this->hPipe, &overlapped, &bytesRead, FALSE))
                {
                    std::cerr << "Failed to get overlapped result. Error: " << GetLastError() << std::endl;
                    CloseHandle(overlapped.hEvent);
                    break;
                }
            }
        }
        else
        {
            std::cerr << "Failed to initiate read operation. Error: " << GetLastError() << std::endl;
            CloseHandle(overlapped.hEvent);
            break;
        }

        if (bytesRead > 0)
        {
            // Notify the callback if registered
            if (this->dataReceivedCallback)
            {
                this->dataReceivedCallback->OnDataReceived(buffer, bytesRead);
            }

            // std::cout << "Received data from named pipe: " << bytesRead << std::endl;
        }

        // Always close the event handle before next loop
        CloseHandle(overlapped.hEvent);
    }
}

void NamePipeServer::Send(const void *data, uint32_t size)
{
    if (this->hPipe == nullptr)
    {
        // std::cerr << "Named pipe is not initialized." << std::endl;
        return;
    }

    auto buff = new uint8_t[size+4];
    memcpy(buff, &size, 4);
    memcpy(buff + 4, data, size);

    OVERLAPPED overlapped = {};
    overlapped.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

    DWORD bytesWritten;

    // if (!WriteFile(this->hPipe, &size, 4, &bytesWritten, nullptr))
    // {
    //     std::cerr << "Failed to write frame size to named pipe. Error: " << GetLastError() << std::endl;
    //     return;
    // }

    BOOL result = WriteFile(
        this->hPipe,
        buff,
        size+4,
        &bytesWritten,
        &overlapped);

    if (!result && GetLastError() != ERROR_IO_PENDING)
    {
        std::cerr << "Failed to initiate write operation. Error: " << GetLastError() << std::endl;
        WaitForSingleObject(overlapped.hEvent, INFINITE);

        if (!GetOverlappedResult(hPipe, &overlapped, &bytesWritten, FALSE))
        {
            // If the write operation failed, log the error
        }
        else
        {
            // Successfully wrote data
        }
    }

    CloseHandle(overlapped.hEvent);
    delete[] buff;
    // if (!WriteFile(this->hPipe, data, static_cast<DWORD>(size), &bytesWritten, nullptr))
    // {
    //     std::cerr << "Failed to write data to named pipe. Error: " << GetLastError() << std::endl;
    //     return;
    // }
}