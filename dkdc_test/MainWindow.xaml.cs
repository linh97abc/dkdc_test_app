using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;

namespace dkdc_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<string> _serialPorts;
        public ObservableCollection<string> SerialPorts { get => _serialPorts; }

        string comport;
        public string ComPort
        {
            get => comport;
            set
            {
                comport = value;

                if (value == null || value == string.Empty)
                {
                    //AppService.GetInst().DisconnectServo();
                    //AppService.GetInst().RootDiaglog.Show("Lỗi", "Mất kết nối tới cổng COM");
                }

                if (this.btnConnect != null)
                {
                    this.btnConnect.IsEnabled = ((comport != null) && (comport != string.Empty));
                }
            }
        }

        public MainWindow()
        {
            this._serialPorts = new ObservableCollection<string>();
            this.DataContext = this;
            InitializeComponent();

            Services.ServiceProvider.Inst.GUIService.RootDialog = this.RootDialog;
            Services.ServiceProvider.Inst.GUIService.RootSnackBar = this.RootSnackbar;

            Wpf.Ui.Appearance.Watcher.Watch(this);
        }

        private void RootDialog_ButtonLeftClick(object sender, RoutedEventArgs e)
        {
            (sender as Wpf.Ui.Controls.Dialog).Hide();
        }

        private void NavigationButtonTheme_OnClick(object sender, RoutedEventArgs e)
        {
            var theme = Wpf.Ui.Appearance.Theme.GetAppTheme();


            if (theme == ThemeType.Dark)
            {
                //AppService.GetInst().IsDarkMode = false;
                Wpf.Ui.Appearance.Theme.Apply(ThemeType.Light);
            }
            else
            {
                //AppService.GetInst().IsDarkMode = true;
                Wpf.Ui.Appearance.Theme.Apply(ThemeType.Dark);
            }
        }

        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            On_UsbChange();

            Services.ServiceProvider.Inst.AppBusiness.PropertyChanged += (object s, PropertyChangedEventArgs ev) =>
            {
                if (ev.PropertyName == "IsSerialOpen")
                {
                    var _appbusiness_stt = s as Store.AppBusiness;
                    if (_appbusiness_stt.IsSerialOpen)
                    {
                        this.btnConnect.IsChecked = false;
                    }
                    else
                    {
                        this.btnConnect.IsChecked = true;
                    }
                }
            };

            Services.ServiceProvider.Inst.IpcService.Start();


        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WmDevicechange = 0x0219;
            const int DBT_DEVNODES_CHANGED = 7;

            if (msg == WmDevicechange)
            {
                switch (wParam.ToInt32())
                {
                    case DBT_DEVNODES_CHANGED:
                        this.On_UsbChange();
                        break;
                    default:
                        break;
                }
            }
            return IntPtr.Zero;
        }

        private void On_UsbChange()
        {
            var listPort = System.IO.Ports.SerialPort.GetPortNames();

            //this._serialPorts.Clear();

            List<string> removePort = new List<string>();

            foreach (var port in this._serialPorts)
            {
                if (!listPort.Contains(port))
                {
                    removePort.Add(port);
                }
            }

            foreach (var port in removePort)
            {
                this._serialPorts.Remove(port);

            }


            foreach (var port in listPort)
            {
                if (!this._serialPorts.Contains(port))
                {
                    this._serialPorts.Add(port);
                }

                //this._serialPorts.Add(port);

            }
        }


        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            var appbusiness = Services.ServiceProvider.Inst.AppBusiness;

            if(appbusiness.IsSerialOpen)
            {
                await appbusiness.Disconnect();
                
            }
            else
            {
                await appbusiness.Connect(this.comport);
                
            }
        }

    }
}
