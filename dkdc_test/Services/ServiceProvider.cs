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

        public Store.InputDataModel InputDataModel { get; private set; } = new Store.InputDataModel();

        public Store.ChartViewModel ChartViewModel { get; private set; }
        public Services.GUIService GUIService { get; private set; }

        public IpcService IpcService { get; private set; } = new IpcService();

        private ServiceProvider()
        {
            this.AppBusiness = new Store.AppBusiness();
            this.GUIService = new Services.GUIService();
            this.ChartViewModel = new Store.ChartViewModel();
        }


        public void TestChart()
        {
            this.ChartViewModel.AddLineData(new float[] { 1, 2, 3, 4, 5 });
            this.ChartViewModel.AddLineData(new float[] { 1.2f, 2.2f, -23, 4, 5 });
            this.ChartViewModel.AddLineData(new float[] { 1, 2, 3, 4, 5 });
        }
    }
}
