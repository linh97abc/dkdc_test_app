using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui.Mvvm.Services;

namespace dkdc_test.Services
{
    internal class GUIService
    {
        public Wpf.Ui.Controls.Dialog RootDialog { get; set; }

        public Wpf.Ui.Controls.Snackbar RootSnackBar { get; set; }
        

        public void ShowPlotHelp()
        {
            this.RootDialog.Show("Hướng dẫn",
                "Click vào đồ thị, sau đó:\n- Kéo thả chuột trái: di chuyển đồ thị\n- Lăn chuột: phóng to, thu nhỏ đồ thị\n- Kéo thả chuột phải: co dãn vùng đồ thị\n- Giữ Ctrl+Lăn chuột: zoom theo chiều ngang\n- Giữ Shift+Lăn chuột: zoom theo chiều dọc");
        }

        public void ShowNotifyError(string title, string message)
        {
            this.RootSnackBar.Show(title, message, Wpf.Ui.Common.SymbolRegular.ErrorCircle24, Wpf.Ui.Common.ControlAppearance.Danger);
        }

        public void ShowNotifyInfo(string title, string message)
        {
            this.RootSnackBar.Show(title, message, Wpf.Ui.Common.SymbolRegular.Info24, Wpf.Ui.Common.ControlAppearance.Info);
        }

        public void ShowNotifyWarning(string title, string message)
        {
            this.RootSnackBar.Show(title, message, Wpf.Ui.Common.SymbolRegular.Warning24, Wpf.Ui.Common.ControlAppearance.Caution);
        }

        public void ShowNotifySuccess(string title, string message)
        {
            this.RootSnackBar.Show(title, message, Wpf.Ui.Common.SymbolRegular.Check24, Wpf.Ui.Common.ControlAppearance.Success);
        }

        public void ShowNotifyException(string title, string message)
        {
            this.RootDialog.Show(title, message);
        }


        public void ShowNotify(string title, string message, Wpf.Ui.Common.SymbolRegular symbol, Wpf.Ui.Common.ControlAppearance appearance)
        {
            this.RootSnackBar.Show(title, message, symbol, appearance);
        }
    }
}
