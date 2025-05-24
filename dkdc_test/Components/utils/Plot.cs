using ScottPlot.Plottables;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkdc_test.Components.utils
{
    public class Plot
    {

        public static (SignalXY signalXY, DataPoint point) GetSignalXYUnderMouse(ScottPlot.Plot plot, double x, double y)
        {
            Pixel mousePixel = new Pixel(x, y);

            Coordinates mouseLocation = plot.GetCoordinates(mousePixel);

            var plot_limit = plot.Axes.GetLimits();

            if (plot_limit.Left > mouseLocation.X)
            {
                return (null, DataPoint.None);
            }

            if (plot_limit.Right < mouseLocation.X)
            {
                return (null, DataPoint.None);
            }

            if (plot_limit.Bottom > mouseLocation.Y)
            {
                return (null, DataPoint.None);
            }

            if (plot_limit.Top < mouseLocation.Y)
            {
                return (null, DataPoint.None);
            }


            foreach (var signal in plot.GetPlottables<SignalXY>().Reverse())
            {
                if (!signal.IsVisible) continue;
                DataPoint nearest = signal.Data.GetNearest(mouseLocation, plot.LastRender);
                if (nearest.IsReal)
                {
                    return (signal, nearest);
                }
            }


            return (null, DataPoint.None);
        }

        public class DataLogerUnderMouse
        {
            public DataLogger signalXY = null;
            public DataPoint point = DataPoint.None;

            public DataLogerUnderMouse() { }


            public DataLogerUnderMouse(DataLogger signalXY, DataPoint point)
            {
                this.signalXY = signalXY;
                this.point = point;
            }
        };




        public static DataLogerUnderMouse GetDataLoggerUnderMouse(ScottPlot.Plot plot, double x, double y)
        {
            Pixel mousePixel = new Pixel(x, y);

            Coordinates mouseLocation = plot.GetCoordinates(mousePixel);

            var plot_limit = plot.Axes.GetLimits();

            if (plot_limit.Left > mouseLocation.X)
            {
                return new DataLogerUnderMouse();
            }

            if (plot_limit.Right < mouseLocation.X)
            {
                return new DataLogerUnderMouse();
            }

            if (plot_limit.Bottom > mouseLocation.Y)
            {
                return new DataLogerUnderMouse();
            }

            if (plot_limit.Top < mouseLocation.Y)
            {
                return new DataLogerUnderMouse();
            }

            var searchZoneSize = (plot_limit.Top - plot_limit.Bottom) / 32;



            List<DataLogerUnderMouse> nearests = new List<DataLogerUnderMouse>();

            foreach (var signal in plot.GetPlottables<DataLogger>().Reverse())
            {
                if (!signal.IsVisible) continue;

                if (signal.Data.Coordinates.Count == 0) continue;

                var rangeX = signal.Data.GetRangeX().Rectified();
                //var px = (int)(mouseLocation.X + 1.5);
                var index = (int)(mouseLocation.X / signal.Period + 0.5);
                var px = index * signal.Period;

                index++;

                if (rangeX.Value1 > px)
                {
                    continue;
                }

                if (rangeX.Value2 < px)
                {
                    continue;
                }

                try
                {
                    var py = signal.Data.Coordinates[index].Y;

                    DataPoint nearest = new DataPoint(px, py, index);
                    nearests.Add(new DataLogerUnderMouse(signal, nearest));

                }
                catch (Exception)
                {


                }


            }

            double min = double.MaxValue;
            DataLogerUnderMouse itemMatch = null;

            foreach (var item in nearests)
            {
                var delta = Math.Abs(mouseLocation.Y - item.point.Y);

                if (delta > searchZoneSize) continue;
                if (delta <= min)
                {
                    min = delta;
                    itemMatch = item;
                }
            }

            if (itemMatch != null)
            {
                return itemMatch;
            }


            return new DataLogerUnderMouse();
        }

        public static DataLogerUnderMouse GetDataLoggerStraightUnderMouse(ScottPlot.Plot plot, double x, double y)
        {
            Pixel mousePixel = new Pixel(x, y);

            Coordinates mouseLocation = plot.GetCoordinates(mousePixel);

            var plot_limit = plot.Axes.GetLimits();

            if (plot_limit.Left > mouseLocation.X)
            {
                return new DataLogerUnderMouse();
            }

            if (plot_limit.Right < mouseLocation.X)
            {
                return new DataLogerUnderMouse();
            }

            if (plot_limit.Bottom > mouseLocation.Y)
            {
                return new DataLogerUnderMouse();
            }

            if (plot_limit.Top < mouseLocation.Y)
            {
                return new DataLogerUnderMouse();
            }

            var searchZoneSize = (plot_limit.Top - plot_limit.Bottom) / 16;



            List<DataLogerUnderMouse> nearests = new List<DataLogerUnderMouse>();

            foreach (var signal in plot.GetPlottables<DataLogger>().Reverse())
            {
                if (!signal.IsVisible) continue;

                if (signal.Data.Coordinates.Count == 0) continue;

                var rangeX = signal.Data.GetRangeX().Rectified();
                //var px = (int)(mouseLocation.X + 1.5);
                var px = (int)(mouseLocation.X + 0.5);


                if (rangeX.Value1 > px)
                {
                    continue;
                }

                if (rangeX.Value2 < px)
                {
                    continue;
                }

                try
                {
                    var py = signal.Data.Coordinates[px].Y;

                    DataPoint nearest = new DataPoint(px, py, px);
                    //DataPoint nearest = new DataPoint(px - 1, py, px - 1);
                    nearests.Add(new DataLogerUnderMouse(signal, nearest));

                }
                catch (Exception)
                {


                }


            }

            double min = double.MaxValue;
            DataLogerUnderMouse itemMatch = null;

            foreach (var item in nearests)
            {
                var delta = Math.Abs(mouseLocation.Y - item.point.Y);

                if (delta > searchZoneSize) continue;
                if (delta <= min)
                {
                    min = delta;
                    itemMatch = item;
                }
            }

            if (itemMatch != null)
            {
                return itemMatch;
            }


            return new DataLogerUnderMouse();
        }

        public static void Button_Zoom_XY(
            ScottPlot.WPF.WpfPlot[] plot,
            Wpf.Ui.Controls.Button btnX,
            Wpf.Ui.Controls.Button btnY,
            Wpf.Ui.Controls.Button btnXY)
        {
            foreach (var item in plot)
            {
                item.Plot.Axes.Rules.Clear();
                item.Refresh();
            }

            btnX.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;
            btnY.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;
            btnXY.Appearance = Wpf.Ui.Common.ControlAppearance.Primary;
        }

        public static void Button_Zoom_X(
            ScottPlot.WPF.WpfPlot[] plot,
            Wpf.Ui.Controls.Button btnX,
            Wpf.Ui.Controls.Button btnY,
            Wpf.Ui.Controls.Button btnXY)
        {
            foreach (var item in plot)
            {
                ScottPlot.AxisLimits limits = item.Plot.Axes.GetLimits();
                var rule = new ScottPlot.AxisRules.LockedVertical(
                    item.Plot.Axes.Left,
                    limits.Bottom,
                    limits.Top);


                item.Plot.Axes.Rules.Clear();
                item.Plot.Axes.Rules.Add(rule);
                item.Refresh();
            }

            btnX.Appearance = Wpf.Ui.Common.ControlAppearance.Primary;
            btnY.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;
            btnXY.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;
        }

        public static void Button_Zoom_Y(
            ScottPlot.WPF.WpfPlot[] plot,
            Wpf.Ui.Controls.Button btnX,
            Wpf.Ui.Controls.Button btnY,
            Wpf.Ui.Controls.Button btnXY)
        {
            foreach (var item in plot)
            {
                ScottPlot.AxisLimits limits = item.Plot.Axes.GetLimits();
                var rule = new ScottPlot.AxisRules.LockedHorizontal(
                    item.Plot.Axes.Bottom,
                    limits.Left,
                    limits.Right);


                item.Plot.Axes.Rules.Clear();
                item.Plot.Axes.Rules.Add(rule);
                item.Refresh();
            }

            btnX.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;
            btnY.Appearance = Wpf.Ui.Common.ControlAppearance.Primary;
            btnXY.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;
        }

        public static string DoubleToFormatedString(double value)
        {
            string formatedY = $"{value:E4}";
            int exponent = int.Parse(formatedY.Split('E')[1]);

            if ((exponent < 3) && (exponent > -3))
            {
                return $"{value:N4}";
            }

            int round_e = ((exponent) / 3) * 3;
            var shiftFactor = Math.Pow(10, -round_e);
            var formatedY2 = $"{shiftFactor * value:N4}";

            if (round_e < 0)
            {
                formatedY2 += $" E{round_e}";
            }
            else if (round_e > 0)
            {
                formatedY2 += $" E+{round_e}";
            }

            return formatedY2;
        }

        public static string VerticalLineText_ValueInfoString(int index, double periodMs)
        {
            return $"index: {index} - {index * periodMs} ms";
        }
    }
}
