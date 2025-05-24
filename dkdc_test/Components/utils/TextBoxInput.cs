using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace dkdc_test.Components.utils
{
    public class TextBoxInput
    {
        static public bool PredictNewVal(System.Windows.Controls.TextBox tb, TextCompositionEventArgs e, Predicate<double> match)
        {
            var oldText = tb.Text;
            var cursor = tb.SelectionStart;
            oldText = oldText.Remove(cursor, tb.SelectionLength);

            var newVal = oldText.Insert(cursor, e.Text);

            bool isDouble = double.TryParse(newVal, out var val);

            if (!isDouble)
            {
                return false;
            }

            return match.Invoke(val);

        }

        public static bool PredictRealNumber(System.Windows.Controls.TextBox tb, TextCompositionEventArgs e)
        {
            var oldText = tb.Text;
            var cursor = tb.SelectionStart;
            oldText = oldText.Remove(cursor, tb.SelectionLength);

            var newVal = oldText.Insert(cursor, e.Text) + "0";

            bool isDouble = double.TryParse(newVal, out var val);

            return isDouble;

        }
    }
}
