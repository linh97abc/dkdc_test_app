#include <iostream>
#include <memory>
#include <ipc/IpcServer.hpp>

#include <thread>

class TestApplication
{
    IpcServer ipcServer;
    std::thread *logUpdateThread;
    std::thread *chartUpdateThread;

    void OnButtonClick(const dkdc_ipc_ButtonClick &buttonClick)
    {
        std::cout << "Button clicked: " << buttonClick.buttonId << std::endl;
        // Handle button click logic here

        switch (buttonClick.buttonId)
        {
            case 1:
                std::cout << "Button ReadPID clicked. Performing action for button 1." << std::endl;
                // Perform action for button 1
                this->ipcServer.UpdateInputField(0, 1.0); // POS Kp
                this->ipcServer.UpdateInputField(1, 2.0); // VEL Kp
                this->ipcServer.UpdateInputField(2, 3.0); // VEL Ki
                break;
                
                case 2:
                std::cout << "Button SavePid clicked. Performing action for button 2." << std::endl;
                this->ipcServer.StartQueryInputData(0); // POS Kp
                this->ipcServer.StartQueryInputData(1); // VEL Kp
                this->ipcServer.StartQueryInputData(2); // VEL Ki
                // wait for the input data to be received
                // save the PID values
                break;
        }
    }

    void OnFrontendMessageRecv(const dkdc_ipc_FrontendToBackendMessage &message)
    {
        // std::cout << "Received message from frontend." << std::endl;
        // Handle the message here

        switch (message.which_cmd)
        {
        case dkdc_ipc_FrontendToBackendMessage_openPort_tag:
            std::cout << "Open Port command received. port: "
                      << message.cmd.openPort.port
                      << std::endl;
            // Handle open port logic
            break;
        case dkdc_ipc_FrontendToBackendMessage_closePort_tag:
            std::cout << "Close Port command received." << std::endl;
            // Handle close port logic
            break;
        case dkdc_ipc_FrontendToBackendMessage_buttonClick_tag:
            this->OnButtonClick(message.cmd.buttonClick);
            break;
        case dkdc_ipc_FrontendToBackendMessage_inputData_tag:
            std::cout << "Input Data command received. id: "
                      << message.cmd.inputData.inputId
                      << "data: "
                      << message.cmd.inputData.value
                      << std::endl;
            // Handle input data logic
            break;
        default:
            std::cerr << "Unknown command received." << std::endl;
            break;
        }
    }

    void TaskUpdateLog()
    {
        // This function can be used to update logs periodically
        // For now, it does nothing

        int count = 0;
        std::string logMessage("Test log message from backend. Count: ");
        while (true)
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(200));
            // Here you can add logic to update logs
            ipcServer.PushLogData(logMessage + std::to_string(count++));
        }
    }

    void TaskUpdateChart()
    {
        // This function can be used to update charts periodically
        // For now, it does nothing

        ipcServer.PushChartData({0.1f, 0.2f, 0.3f, 0.4f, 0.5f});

        int count = 0;

        while (true)
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(20));
            // Here you can add logic to update charts
            ipcServer.PushChartData({0.1f + count * 0.1f, 0.2f + count * 0.1f, 0.3f + count * 0.1f});
            count++;
        }
    }

public:
    ~TestApplication()
    {
        if (this->logUpdateThread && this->logUpdateThread->joinable())
        {
            this->logUpdateThread->join();
            delete this->logUpdateThread;
        }

        if (this->chartUpdateThread && this->chartUpdateThread->joinable())
        {
            this->chartUpdateThread->join();
            delete this->chartUpdateThread;
        }

        std::cout << "Test application stopped." << std::endl;
    }

    void Start()
    {

        ipcServer.RegisterMessageHandler(
            [this](const dkdc_ipc_FrontendToBackendMessage &message)
            {
                this->OnFrontendMessageRecv(message);
            });

        ipcServer.Start();

        this->logUpdateThread = new std::thread(&TestApplication::TaskUpdateLog, this);
        this->chartUpdateThread = new std::thread(&TestApplication::TaskUpdateChart, this);

        std::cout << "Test application started." << std::endl;
    }
};

int main()
{
    std::cout << "Backend running..." << std::endl;

    auto app = std::make_unique<TestApplication>();

    app->Start();

    while (true)
    {
        std::this_thread::sleep_for(std::chrono::seconds(1));
    }

    return 0;
}