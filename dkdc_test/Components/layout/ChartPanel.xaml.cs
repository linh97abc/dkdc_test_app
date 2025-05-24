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

using dkdc_test.Components.utils;
using dkdc_test.Services;
using ScottPlot;
using Wpf.Ui.Common;

namespace dkdc_test.Components.layout
{
    /// <summary>
    /// Interaction logic for ChartPanel.xaml
    /// </summary>
    public partial class ChartPanel : UserControl
    {
        Store.ChartViewModel viewModel;

        public ChartPanel()
        {
            InitializeComponent();
            this.InitChart(this.chart);
            this.viewModel = ServiceProvider.Inst.ChartViewModel;
            this.DataContext = this.viewModel;

            this.viewModel.InitChart(this.chart);
        }

        private void Button_Zoom_Click(object sender, RoutedEventArgs e)
        {
            var item = this.chart;
            item.Plot.Axes.AutoScale();
            item.Refresh();


        }

        private void Button_ZoomXY_Click(object sender, RoutedEventArgs e)
        {
            Components.utils.Plot.Button_Zoom_XY(new ScottPlot.WPF.WpfPlot[] { this.chart }, this.btnZoomX, this.btnZoomY, this.btnZoomXY);


        }


        private void Button_ZoomX_Click(object sender, RoutedEventArgs e)
        {
            Components.utils.Plot.Button_Zoom_X(new ScottPlot.WPF.WpfPlot[] { this.chart }, this.btnZoomX, this.btnZoomY, this.btnZoomXY);

        }

        private void Button_ZoomY_Click(object sender, RoutedEventArgs e)
        {
            Components.utils.Plot.Button_Zoom_Y(new ScottPlot.WPF.WpfPlot[] { this.chart }, this.btnZoomX, this.btnZoomY, this.btnZoomXY);

        }

        private void Button_Help_Click(object sender, RoutedEventArgs e)
        {
            ServiceProvider.Inst.GUIService.ShowPlotHelp();
        }

        void Button_AutoScroll_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.IsChartAutoScroll = !this.viewModel.IsChartAutoScroll;


            
        }

        void InitChart(ScottPlot.WPF.WpfPlot plot)
        {
            ScottPlot.Plottables.Crosshair crosshair;
            this.chart.Plot.Legend.IsVisible = false;

            crosshair = plot.Plot.Add.Crosshair(0, 0);
            crosshair.IsVisible = false;
            crosshair.MarkerShape = MarkerShape.OpenCircle;
            crosshair.MarkerSize = 15;
            crosshair.TextColor = ScottPlot.Colors.White;
            crosshair.TextBackgroundColor = crosshair.HorizontalLine.Color;


            plot.Plot.FigureBackground.Color = ScottPlot.Colors.Transparent;
            plot.Plot.Axes.Color(ScottPlot.Colors.Gray);
            plot.Plot.Grid.MajorLineColor = ScottPlot.Colors.Gray.WithOpacity(0.25);
            plot.Plot.Grid.MinorLineColor = ScottPlot.Colors.Gray;
            //plot.Plot.Title("Đồ thị vị trí xi lanh (mm)");
            plot.Plot.Axes.Title.Label.Bold = true;

            plot.Plot.Axes.Title.Label.FontName = ScottPlot.Fonts.System;
            plot.Plot.Axes.Title.Label.FontSize = 16;
            plot.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    return;
                }

                if (e.RightButton == MouseButtonState.Pressed)
                {
                    return;
                }

                var pos = e.GetPosition(plot);
                //var xy = Helper.Plot.GetSignalXYUnderMouse(plot.Plot, pos.X, pos.Y);
                var xy = Components.utils.Plot.GetDataLoggerUnderMouse(plot.Plot, pos.X, pos.Y);

                if (xy.point.IsReal)
                {
                    crosshair.IsVisible = true;
                    crosshair.Position = xy.point.Coordinates;

                    //crosshair.VerticalLine.Text = Components.utils.Plot.VerticalLineText_ValueInfoString(xy.point.Index - 1, this.viewModel.ControlPeriodMs);
                    crosshair.HorizontalLine.Text = $"{xy.signalXY.LegendText}: {Components.utils.Plot.DoubleToFormatedString(xy.point.Y)}";
                    plot.Refresh();
                }
                else
                {
                    if (crosshair.IsVisible)
                    {
                        crosshair.IsVisible = false;
                        plot.Refresh();

                    }
                }

            };
        }

    }
}
