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
    /// <summary>
    /// Interaction logic for LogPanel.xaml
    /// </summary>
    public partial class LogPanel : UserControl
    {
        public LogPanel()
        {
            InitializeComponent();
            Services.ServiceProvider.Inst.IpcService.OnNewLog += (string line) => {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.AddLog(line);
                });
                
            };
        }

        void AddLog(string line)
        {
            if (editor.Text.Length > 0)
            {
                editor.Text += "\n";
            }
            editor.Text += line;
            editor.ScrollToEnd();
        }

        void ClearLog()
        {
            editor.Text = string.Empty;
        }

        private void Button_ClearLog_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }

        private void Button_ScrollLog_Click(object sender, RoutedEventArgs e)
        {
            editor.ScrollToEnd();
        }
    }
}
