using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace dkdc_test.Components.Converter
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class ConnectSttToBtnString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tmp = (bool)value;
            return tmp ? "Kết nối" : "Ngắt kết nối";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
