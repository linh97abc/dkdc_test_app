using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dkdc_test.Components.layout
{
    enum ButtonID {
        ReadPID = 1,
        SavePid = 2,
        
    }

    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : UserControl
    {
        public ControlPanel()
        {
            InitializeComponent();
            this.DataContext = Services.ServiceProvider.Inst;
        }

        private async void  Button_ReadPID_Click(object sender, RoutedEventArgs e)
        {
            await Services.ServiceProvider.Inst.IpcService.NotifyEventBtnClick((uint)ButtonID.ReadPID);
        }

        private async void Button_SavePid_Click(object sender, RoutedEventArgs e)
        {
            await Services.ServiceProvider.Inst.IpcService.NotifyEventBtnClick((uint)ButtonID.SavePid);
        }
    }
}
