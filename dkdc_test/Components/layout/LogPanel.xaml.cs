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
            editor.Text = "ljksdfjlksdjfkl\nasdjflsjkdhflsd\nsdifsdikfjsd\nedfjk sdfjls sdifjlskdjf";
        }
    }
}
