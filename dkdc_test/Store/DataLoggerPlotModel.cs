using dkdc_test.Components.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkdc_test.Store
{
    public class DataLoggerPlotModel : BindableBase
    {
        public string Tag { get; set; }
        public string Name { get; set; }

        
        public float newestValue = 0f;
        public float NewestValue
        {
            get => this.newestValue;
            set
            {
                Set(ref this.newestValue, value);
            }
        }


        int range = 120;
        public int Range
        {
            get => this.range;
            set
            {
                this.ChangeRange(value);

            }
        }

        public void ChangeRange(int val)
        {




        }

        public double Period
        {
            get => this.Signal.Period;
            set
            {
                this.Signal.Data.XOffset = -value;
                this.Signal.Period = value;
            }
        }

        ScottPlot.WPF.WpfPlot plot;

        bool isVisible = true;
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible == value) return;
                if (this.Signal != null)
                {
                    this.Signal.IsVisible = value;
                    plot?.Refresh();
                }

                Set(ref isVisible, value);
            }
        }

        //ScottPlot.Collections.CircularBuffer<double> buffer;
        //ScottPlot.Collections.CircularBuffer<double> bufferPause;

        //public DataLoggerPlotModel() { }

        void Init(string name, int range = 2048)
        {
            this.Name = name;
            this.range = 120;
            //buffer = new ScottPlot.Collections.CircularBuffer<double>(range);
            //this.bufferPause = new ScottPlot.Collections.CircularBuffer<double>(range);

        }


        public DataLoggerPlotModel(string name, int range = 2048)
        {
            this.Init(name, range);

        }

        public DataLoggerPlotModel(ScottPlot.WPF.WpfPlot plot, string name, int range = 2048)
        {
            this.Init(name, range);
            this.CreateSignalFromPlot(plot);


        }


        public ScottPlot.Plottables.DataLogger Signal { get; private set; }


        bool isAutoScroll;
        public bool IsAutoScroll
        {
            get => isAutoScroll;
            set
            {

                if (this.Signal != null)
                {


                    if (value)
                    {
                        this.Signal.AxisManager = this.axisSlideManager;
                    }
                    else
                    {

                        this.Signal.AxisManager = new Components.utils.CustomAxisLimitManager();

                    }
                }
                Set(ref isAutoScroll, value);

            }
        }

        ScottPlot.AxisLimitManagers.Slide axisSlideManager = new ScottPlot.AxisLimitManagers.Slide();


        public int ScrollDepth
        {
            get => (int)(axisSlideManager.Width / this.Signal.Period);
            set => axisSlideManager.Width = value * this.Signal.Period;
        }



        public ScottPlot.Plottables.DataLogger CreateSignalFromPlot(ScottPlot.WPF.WpfPlot plot)
        {

            this.plot = plot;
            this.Signal = plot.Plot.Add.DataLogger();

            if (this.Signal is null)
            {
                throw new NullReferenceException();
            }



            this.Signal.AxisManager = new Components.utils.CustomAxisLimitManager();

            this.Signal.LegendText = this.Name;
            this.ChangeRange(this.range);

            this.Signal.ConnectStyle = ScottPlot.ConnectStyle.StepHorizontal;

            this.Tag = this.Signal.LineColor.ToHex();

            this.Signal.Data.XOffset = -1;

            this.Signal.Add(0);



            return this.Signal;
        }

        bool isPause = false;
        public bool IsPause
        {
            get => isPause;
            set
            {


                if (isPause && !value)
                {
                    //foreach (var item in this.bufferPause)
                    //{
                    //    this.buffer.Add(item);
                    //}
                    //this.bufferPause.Clear();

                    this.ChangeRange(this.range);

                }

                isPause = value;
            }
        }

        public void Add(float val)
        {
            if (!this.IsPause)
            {
                //this.buffer.Add(val);
                this.Signal.Add(val);
                this.newestValue = val;
            }
            else
            {
                //this.bufferPause.Add(val);
            }

        }

        public void Clear()
        {
            //this.buffer.Clear();
            Signal.Clear();
            Signal.Add(0);
        }

        public void Render()
        {
            this.OnPropertyChanged(nameof(NewestValue));
        }
    }
}
