using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkdc_test.Services
{
    internal class ServiceProvider
    {
        public static ServiceProvider Inst { get; } = new ServiceProvider();

        public Store.AppBusiness AppBusiness { get; private set; }


        private ServiceProvider()
        {
            this.AppBusiness = new Store.AppBusiness();
        }
    }
}
