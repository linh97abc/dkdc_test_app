using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkdc_test.Store
{
    internal class AppBusiness : Components.utils.BindableBase
    {
        bool isSerialOpen;
        public bool IsSerialOpen
        {
            get => isSerialOpen;
            set
            {
                Set(ref isSerialOpen, value);
            }
        }
    }
}
