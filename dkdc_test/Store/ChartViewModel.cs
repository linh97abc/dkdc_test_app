using dkdc_test.Components.utils;
using dkdc_test.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace dkdc_test.Store
{

    public class ChartViewModel : BindableBase
    {
        string[] linesNames = new string[] { "line1", "Line2" };
        const int PLOT_MAX_POINT = 4096;

        public ChartViewModel()
        {
            this.Lines = new List<DataLoggerPlotModel>();

            // Register event


        }



        public void AddLineData(float[] data)
        {
            this.delayChartDatas.Enqueue(data);
        }


        public List<DataLoggerPlotModel> Lines { get; set; }
        public ScottPlot.WPF.WpfPlot Chart { get; private set; }

        void ChartRefresh()
        {
            this.Chart.Refresh();
        }

        public void InitChart(ScottPlot.WPF.WpfPlot chart)
        {

            this.Chart = chart;

            foreach (var lineName in this.linesNames)
            {
                this.Lines.Add(new DataLoggerPlotModel(this.Chart, lineName, PLOT_MAX_POINT));

            }
        }



        int chartScrollDepth;
        public int ChartScrollDepth
        {
            get => chartScrollDepth;
            set
            {
                Set(ref chartScrollDepth, value);

                var depth = Math.Pow(2, value);

                foreach (var item in Lines)
                {

                    item.ScrollDepth = (int)depth;
                }
            }
        }

        bool isChartAutoScroll;
        public bool IsChartAutoScroll
        {
            get => isChartAutoScroll;
            set
            {

                foreach (var item in Lines)
                {
                    item.IsAutoScroll = value;

                }

                Set(ref isChartAutoScroll, value);
            }
        }

        bool isChartRunning;



        public void StopChart()
        {

            displayTimer?.Stop();
            this.isChartRunning = false;
            this.IsChartAutoScroll = false;
        }

        public void StartChartMonitor()
        {

            this.PlotClear();

            this.IsChartAutoScroll = true;
            this.isChartRunning = true;

            displayTimer = new DispatcherTimer();
            //displayTimer.Interval = TimeSpan.FromMilliseconds(AppService.GetInst().ChartRefreshRate);
            displayTimer.Interval = TimeSpan.FromMilliseconds(500);
            displayTimer.Tick += DisplayTimer_Tick;
            displayTimer.Start();
        }


        private void DisplayTimer_Tick(object sender, EventArgs e)
        {
            this.OnDisplayTick(this);
        }

        DispatcherTimer displayTimer;




        public object Sync { get; } = new object();
        ConcurrentQueue<float[]> delayChartDatas = new ConcurrentQueue<float[]>();

        void PlotAddNewData(float[] data)
        {
            int index = 0;
            foreach (var item in this.Lines)
            {
                item.Add(data[index++]);
            }

        }

        void PlotClear()
        {

            this.delayChartDatas = new ConcurrentQueue<float[]>();


            var period = 1;
            foreach (var item in this.Lines)
            {
                item.Period = period;
                item.Clear();
            }


            this.Chart.Plot.Axes.SetLimitsY(-5, 160);


            this.Chart.Plot.Axes.SetLimitsX(-1, 0);



        }

        void OnDisplayTick(object sender)
        {

            if (isChartRunning)
            {
                lock (this.Sync)
                {
                    bool hasNewData = false;


                    while (this.delayChartDatas.TryDequeue(out var data))
                    {

                        hasNewData = true;
                        this.PlotAddNewData(data);
                    }




                    if (!hasNewData)
                    {
                        return;
                    }

                    foreach (var item in Lines)
                    {
                        item.Render();
                    }
                    this.ChartRefresh();

                }

            }
        }






    }


}
