using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinUI;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Drawing;
using System.Diagnostics;
using LiveChartsCore.Drawing;

namespace MarkGPT.Helpers
{
    public static class ChartHelper
    {
        public static CartesianChart CreateFunctionChart(Func<double, double> function, double minX, double maxX, int numPoints = 100, string title = "Function Plot")
        {
            var xValues = new List<double>();
            var yValues = new List<double>();
            double step = (maxX - minX) / (numPoints - 1);

            double closestY = double.MaxValue;
            double rootX = 0;

            for (int i = 0; i < numPoints; i++)
            {
                double x = minX + i * step;
                double y = function(x);

                // Track the point where y is closest to 0
                if (Math.Abs(y) < Math.Abs(closestY))
                {
                    closestY = y;
                    rootX = x;
                }

                xValues.Add(x);
                yValues.Add(y);
            }

            var lineSeries = new LineSeries<ObservablePoint>
            {
                DataPadding = new LvcPoint(0, 0),
                GeometryStroke = null,
                Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 5 },
                GeometryFill = null,
                Fill = null,
                Values = new ObservableCollection<ObservablePoint>(
                    xValues.Zip(yValues, (x, y) => new ObservablePoint(x, y)).ToList()
                ),
                Name = title
            };

            var rootSeries = new LineSeries<ObservablePoint>
            {
                GeometrySize = 14,
                GeometryFill = new SolidColorPaint(SKColors.Red),
                Values = new ObservableCollection<ObservablePoint>
                {
                    new ObservablePoint(rootX, function(rootX))
                },
                Name = $"Root (approx): x = {rootX:0.###}"
            };

            var chart = new CartesianChart
            {
                Series = new ISeries[] { lineSeries, rootSeries },
                Height = 350,
                Width = 500,
                Background = new SolidColorBrush(Colors.DarkGray),
                Margin = new Thickness(0, 10, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            return chart;
        }


        public static CartesianChart CreateLinearRegressionChart(ObservableCollection<double> xValues, ObservableCollection<double> yValues,
    double slope, double intercept, string equation)
        {
            // Create scatter series for data points
            // Create scatter series for data points
            var scatterSeries = new LineSeries<ObservablePoint>
            {
                GeometrySize = 14,
                DataPadding = new LvcPoint(0, 0),
                Fill = null,
                GeometryFill = null,
                Values = new ObservableCollection<ObservablePoint>(), // Initialize an empty ObservableCollection
                Name = "Data Points" // Set the name property of the series, not the collection
            };


            // Calculate line points for the regression line
            double minX = xValues.Min();
            double maxX = xValues.Max();

            // Create line series for regression line
            var lineSeries = new LineSeries<ObservablePoint>
            {
                DataPadding = new LvcPoint(0, 0),
                GeometryStroke = null,
                Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 5 },
                GeometryFill = null,
                Fill = null,
                Values = new ObservableCollection<ObservablePoint> // Initialize an *empty* ObservableCollection
        {
            new ObservablePoint(minX, slope * minX + intercept),
            new ObservablePoint(maxX, slope * maxX + intercept)
        },
                Name = equation
            };

            // Create the chart with both series
            var chart = new CartesianChart
            {
                Series = new ISeries[] { scatterSeries, lineSeries },
                Height = 400,
                Width = 600,
                Background = new SolidColorBrush(Colors.Transparent),
                Margin = new Thickness(0, 10, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom
            };

            // Populate the scatter series' Values collection
            var scatterValues = scatterSeries.Values as ObservableCollection<ObservablePoint>;
            if (scatterValues != null)
            {
                foreach (var x in xValues)
                {
                    scatterValues.Add(new ObservablePoint(x, yValues.ElementAt(xValues.IndexOf(x))));
                }
            }


            return chart;
        }

        public static CartesianChart CreatePolynomialRegressionChart(
            ObservableCollection<double> xValues,
            ObservableCollection<double> yValues,
            List<double> coefficients,
            string equation)
        {
            // Create scatter series for data points
            var scatterSeries = new LineSeries<ObservablePoint>
            {
                GeometrySize = 14,
                DataPadding = new LvcPoint(0, 0),
                Fill = null,
                GeometryFill = null,
                Values = new ObservableCollection<ObservablePoint>(),
                Name = "Data Points"
            };

            // Fill scatter points
            var scatterValues = scatterSeries.Values as ObservableCollection<ObservablePoint>;
            if (scatterValues != null)
            {
                foreach (var x in xValues)
                {
                    var y = yValues.ElementAt(xValues.IndexOf(x));
                    scatterValues.Add(new ObservablePoint(x, y));
                }
            }

            // Generate smoother curve points for the polynomial line
            var polyPoints = new ObservableCollection<ObservablePoint>();
            double minX = xValues.Min();
            double maxX = xValues.Max();
            double step = (maxX - minX) / 100; // More steps = smoother curve

            for (double x = minX; x <= maxX; x += step)
            {
                double y = 0;
                for (int i = 0; i < coefficients.Count; i++)
                {
                    y += coefficients[i] * Math.Pow(x, i);
                }
                polyPoints.Add(new ObservablePoint(x, y));
            }

            // Line series for polynomial regression
            var lineSeries = new LineSeries<ObservablePoint>
            {
                DataPadding = new LvcPoint(0, 0),
                GeometryStroke = null,
                Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 5 },
                GeometryFill = new SolidColorPaint(SKColors.Gray),
                Fill = null,
                Values = polyPoints,
                Name = equation
            };

            // Create the chart
            var chart = new CartesianChart
            {
                Series = new ISeries[] { scatterSeries, lineSeries },
                Height = 400,
                Width = 600,
                Background = new SolidColorBrush(Colors.Transparent),
                Margin = new Thickness(0, 10, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom
            };

            return chart;
        }

        public static CartesianChart CreateSimpsonChart(Func<double, double> function, double a, double b, int n, string title = "Simpson's 1/3 Rule")
        {
            // Simpson's requires n to be even
            if (n % 2 != 0) throw new ArgumentException("n must be even for Simpson's 1/3 rule.");

            var xValues = new List<double>();
            var yValues = new List<double>();

            double step = (b - a) / n;

            // Calculate all points at subinterval ends (a, a+step, ..., b)
            for (int i = 0; i <= n; i++)
            {
                double x = a + i * step;
                xValues.Add(x);
                yValues.Add(function(x));
            }

            // Create the line series for the function curve (more dense points for smoothness)
            var smoothX = new List<double>();
            var smoothY = new List<double>();
            int smoothPoints = 200;
            double smoothStep = (b - a) / (smoothPoints - 1);
            for (int i = 0; i < smoothPoints; i++)
            {
                double x = a + i * smoothStep;
                smoothX.Add(x);
                smoothY.Add(function(x));
            }

            var functionLineSeries = new LineSeries<ObservablePoint>
            {
                DataPadding = new LvcPoint(0, 0),
                GeometryStroke = null,
                Stroke = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 3 },
                GeometryFill = null,
               
                Values = new ObservableCollection<ObservablePoint>(
                    smoothX.Zip(smoothY, (x, y) => new ObservablePoint(x, y)).ToList()
                ),
                Name = title
            };

            // Create scatter points for the sampled points Simpson uses
            var simpsonPointsSeries = new LineSeries<ObservablePoint>
            {
                GeometrySize = 10,
                GeometryFill = new SolidColorPaint(SKColors.Red),
                Stroke = null,
                Values = new ObservableCollection<ObservablePoint>(
                    xValues.Zip(yValues, (x, y) => new ObservablePoint(x, y)).ToList()
                ),
                Name = "Sample Points"
            };

            // Create the chart with both series
            var chart = new CartesianChart
            {
                Series = new ISeries[] { functionLineSeries, simpsonPointsSeries },
                Height = 400,
                Width = 600,
                Background = new SolidColorBrush(Colors.Transparent),
                Margin = new Thickness(0, 10, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom
            };

            return chart;
        }






        /// <summary>
        /// Adds a chart to the specified stack panel and scrolls to show it
        /// </summary>
        public static void AddChartToDisplay(MainPage page, StackPanel stack, CartesianChart chart)
        {
            // Add the chart to the stack panel
            stack.Children.Add(new Border { Child = chart });
            Debug.WriteLine($"Chart object: {chart}"); // Check if the chart object is null
            Debug.WriteLine($"Chart type: {chart?.GetType().FullName}"); // Check the type of the chart
            page.ScrollToBottom();
        }
    }
}