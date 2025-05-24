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

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

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
            Services.ServiceProvider.Inst.IpcService.OnNewLog += (string line) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        this.AddLog(line);

                    }
                    catch (Exception)
                    {


                    }
                });

            };
        }

        void AddLog(string line)
        {

            var lastVisibleLine = editor.TextArea.TextView.VisualLines.LastOrDefault();
            int totalLines = editor.Document.LineCount;
            int lastVisibleLineNumber = lastVisibleLine?.LastDocumentLine?.LineNumber ?? 0;
            bool isAtBottom = lastVisibleLineNumber >= totalLines - 1;



            if (editor.Text.Length > 0)
            {
                editor.Text += "\n";
            }
            editor.Text += line;

            if (isAtBottom)
            {
                editor.ScrollToEnd();

            }

            // Only scroll to end if caret is at the last line
            //int caretLine = editor.GetLineIndexFromCharacterIndex(editor.CaretIndex);
            //int lastLine = editor.LineCount - 1;
            //if (caretLine == lastLine)
            //{
            //    editor.ScrollToEnd();
            //}
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
