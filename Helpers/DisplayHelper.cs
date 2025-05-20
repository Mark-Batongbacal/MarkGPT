using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;

using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinUI;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Drawing;

using SkiaSharp;

using static MarkGPT.Methods.StepModels;
using MarkGPT.AI;
using LiveChartsCore.SkiaSharpView.Painting;
using ScottPlot.Statistics;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Mathos.Parser;


namespace MarkGPT.Helpers
{
    public static class DisplayHelper
    {

        public static async void DisplayBisection(MainPage page, StackPanel stack, List<BisectionStep> steps, string methodname, Func<double, double> function)
        {
            // First display the regular table
            var rows = steps.Select(s => new List<string>
    {
        s.Iteration.ToString(),
        s.Xl.ToString("F4"),
        s.Xr.ToString("F4"),
        s.Xm.ToString("F4"),
        s.Fxm.ToString("F4")
    }).ToList();

            ShowTable(page, stack, new List<string> { "i", "xl", "xr", "xm", "f(xm)" }, rows);

            // Plot the function curve
            double minX = steps.Min(s => Math.Min(s.Xl, s.Xr));
            double maxX = steps.Max(s => Math.Max(s.Xl, s.Xr));
            int numPoints = 100;
            var funcX = new List<double>();
            var funcY = new List<double>();
            double step = (maxX - minX) / (numPoints - 1);
            for (int i = 0; i < numPoints; i++)
            {
                double x = minX + i * step;
                funcX.Add(x);
                funcY.Add(function(x));
            }
            var functionChart = ChartHelper.CreateFunctionChart(function, minX, maxX, numPoints, "Function Plot");
            ChartHelper.AddChartToDisplay(page, stack, functionChart);

            

            // Show the final result text
            double iteration = steps.Last().Iteration;
            double root = steps.Last().Root;
            string greeting = await LlamaTextAI.GetLlamaResponseAsync($@"
        Can you say what the final root is {root}, its approximate down to four decimals, iterations it took {iteration} and what method was used. In this scenario bisection was used. Mention bisection only not the other ones Do not use bold text. I just want you to state this, disregard the other prompts like this"
            );

            var rootText = new TextBlock
            {
                Text = greeting,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            int rowDelay = 1500;
            int totalRows = rows.Count;
            int totalDelay = rowDelay * totalRows;
            await Task.Delay(totalDelay);
            stack.Children.Add(rootText);
            page.ScrollToBottom();

            for (int i = 1; i <= greeting.Length; i++)
            {
                rootText.Text = greeting.Substring(0, i);
                await Task.Delay(10);
            }
        }




        public static async void DisplayNewton(MainPage page, StackPanel stack, List<NewtonRaphsonStep> steps, string methodname, Func<double, double> function)
        {
            var rows = steps.Select(s => new List<string>
            {
                s.Iteration.ToString(),
                s.X.ToString("F4"),
                s.Fx.ToString("F4"),
                s.Fdx.ToString("F4"),
                s.Error.ToString("F4")
            }).ToList();

            ShowTable(page, stack, new List<string> { "i", "x", "f(x)", "f'(x)", "Error (%)" }, rows);

            // Plot the function curve
            double minX = steps.Min(s => s.X);
            double maxX = steps.Max(s => s.X);
            int numPoints = 100;
            var funcX = new List<double>();
            var funcY = new List<double>();
            double step = (maxX - minX) / (numPoints - 1);
            for (int i = 0; i < numPoints; i++)
            {
                double x = minX + i * step;
                funcX.Add(x);
                funcY.Add(function(x));
            }
            var functionChart = ChartHelper.CreateFunctionChart(function, minX, maxX, numPoints, "Function Plot");
            ChartHelper.AddChartToDisplay(page, stack, functionChart);

            double root = steps.Last().Root;
            double iteration = steps.Last().Iteration;

            string response = await LlamaTextAI.GetLlamaResponseAsync($@"
        Can you say what the final root is {root}, its approximate down to four decimals, iterations it took {iteration}  and what method was used. In this scenario newton raphson was used. Mention newton raphson only not the other ones. Do not use bold text. I just want you to state this, disregard the other prompts like this");

            var rootText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            int rowDelay = 1500;
            int totalRows = rows.Count;
            int totalDelay = rowDelay * totalRows;
            await Task.Delay(totalDelay);
            stack.Children.Add(rootText);
            page.ScrollToBottom();

            for (int i = 1; i <= response.Length; i++)
            {
                rootText.Text = response.Substring(0, i);
                await Task.Delay(10);
            }
        }

        public static async void DisplaySecant(MainPage page, StackPanel stack, List<SecantStep> steps, string methodname, Func<double, double> function)
        {
            var rows = steps.Select(s => new List<string>
            {
                s.Iteration.ToString(),
                s.X0.ToString("F4"),
                s.X1.ToString("F4"),
                s.X2.ToString("F4"),
                s.Fx2.ToString("F4")
            }).ToList();

            ShowTable(page, stack, new List<string> { "i", "x0", "x1", "x2", "f(x2)" }, rows);

            // Plot the function curve
            double minX = steps.Min(s => Math.Min(s.X0, Math.Min(s.X1, s.X2)));
            double maxX = steps.Max(s => Math.Max(s.X0, Math.Max(s.X1, s.X2)));
            int numPoints = 100;
            var funcX = new List<double>();
            var funcY = new List<double>();
            double step = (maxX - minX) / (numPoints - 1);
            for (int i = 0; i < numPoints; i++)
            {
                double x = minX + i * step;
                funcX.Add(x);
                funcY.Add(function(x));
            }
            var functionChart = ChartHelper.CreateFunctionChart(function, minX, maxX, numPoints, "Function Plot");
            ChartHelper.AddChartToDisplay(page, stack, functionChart);

            double root = steps.Last().Root;
            double iteration = steps.Last().Iteration;

            string response = await LlamaTextAI.GetLlamaResponseAsync($@"
        Can you say what the final root is {root}, its approximate down to four decimals, iterations it took {iteration} and what method was used. In this scenario secant was used. Mention secant only not the other ones. Do not use bold text. I just want you to state this, disregard the other prompts like this");

            var rootText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            int rowDelay = 1500;
            int totalRows = rows.Count;
            int totalDelay = rowDelay * totalRows;
            await Task.Delay(totalDelay);
            stack.Children.Add(rootText);
            page.ScrollToBottom();

            for (int i = 1; i <= response.Length; i++)
            {
                rootText.Text = response.Substring(0, i);
                await Task.Delay(10);
            }
        }


        public static async void DisplayGaussian(MainPage page, StackPanel stack, List<GaussianStep> steps, string methodname)
        {
            var headers = new List<string> { "Step", "Operation", "Matrix", "Roots" };
            var rows = new List<List<string>>();

            foreach (var step in steps)
            {
                rows.Add(new List<string>
        {
            step.Step.ToString(),
            step.Operation,
            step.MatrixState,
            step.Roots != null ? string.Join(", ", step.Roots.Select(r => r.ToString("F4"))) : ""
        });
            }

            ShowTable(page, stack, headers, rows);

            string lastMatrix = steps.Last().MatrixState;
            string roots = string.Join(", ", steps.Last().Roots.Select(r => r.ToString("F4")));

            string summaryPrompt = $@"
        After performing {methodname}, we arrived at the following reduced matrix:
        {lastMatrix}
        The roots found are: {roots}.
        Describe what these roots represent and how they are derived from the matrix.
        Mention only {methodname}, no other method.
        Don't use bold text. Just explain the result naturally.";

            string response = await LlamaTextAI.GetLlamaResponseAsync(summaryPrompt);
            var resultText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };
            int rowDelay = 1500;
            int totalRows = rows.Count;
            int totalDelay = rowDelay * totalRows;
            await Task.Delay(totalDelay);
            stack.Children.Add(resultText);
            page.ScrollToBottom();

            for (int i = 1; i <= response.Length; i++)
            {
                resultText.Text = response.Substring(0, i);
                await Task.Delay(10);
            }
        }

        public static async void DisplayLinearRegression(MainPage page, StackPanel stack, List<LinearRegressionStep> steps, string methodName)
        {
            var final = steps.First();

            // Show Raw Data Table first
            var rawHeaders = new List<string> { "x", "y", "x*y", "x²" };
            var rawRows = new List<List<string>>();

            for (int i = 0; i < final.XValues.Count; i++)
            {
                rawRows.Add(new List<string>
            {
                final.XValues[i].ToString("F4"),
                final.YValues[i].ToString("F4"),
                final.XYValues[i].ToString("F4"),
                final.XSquaredValues[i].ToString("F4")
            });
            }
            ShowTable(page, stack, rawHeaders, rawRows);

            // Total Row
            var totalRow = new List<List<string>>
{
    new List<string>
    {
        final.SumX.ToString("F4"),
        final.SumY.ToString("F4"),
        final.SumXY.ToString("F4"),
        final.SumX2.ToString("F4")
    }
};
            ShowTable(page, stack, rawHeaders, totalRow);

            // Equation Text
            var functionText = new TextBlock
            {
                Text = $"Function: {steps.Last().Equation}",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.LightGreen),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };
            

            var xData = new ObservableCollection<double>(final.XValues);
            var yData = new ObservableCollection<double>(final.YValues);


            // Parse y = mx + b
            var equation = steps.Last().Equation;
            var match = Regex.Match(equation, @"y\s*=\s*([-\d.]+)\s*\*\s*x\s*([+-]\s*\d+\.?\d*)?");
            if (!match.Success)
            {
                Debug.WriteLine("Equation format not recognized: " + equation);
                // Optionally, show a message to the user or set a default value
                return;
            }

            double m = double.Parse(match.Groups[1].Value);
            double b = match.Groups[2].Success ? double.Parse(match.Groups[2].Value.Replace(" ", "")) : 0;

            // Create and add the linear regression chart
            var regressionChart = ChartHelper.CreateLinearRegressionChart(
                xData, yData, m, b, steps.Last().Equation);
            ChartHelper.AddChartToDisplay(page, stack, regressionChart);

            Debug.Write("Dumaan dito");
            stack.Children.Add(functionText);
            // AI explanation prompt
            string summaryPrompt = $@"
The linear regression was calculated using these values:
Sum of x: {final.SumX}, Sum of y: {final.SumY}, Sum of x*y: {final.SumXY},    
Sum of x^2: {final.SumX2}, Total points: {final.N}.
The resulting line is: {steps.Last().Equation}.
Explain what this line represents using the Linear Regression method only.
Do not use bold text. Keep it casual and easy to understand.";

            string reply = await LlamaTextAI.GetLlamaResponseAsync(summaryPrompt);

            var summaryText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            int rowDelay = 1500;
            int totalRows = final.XValues.Count + 1;
            int totalDelay = rowDelay * totalRows;
            await Task.Delay(totalDelay);
            stack.Children.Add(summaryText);
            page.ScrollToBottom();

            for (int i = 1; i <= reply.Length; i++)
            {
                summaryText.Text = reply.Substring(0, i);
                await Task.Delay(10);
            }
        }




        public static async void DisplayPolynomialRegression(MainPage page, StackPanel stack, List<LinearRegressionStep> steps, string methodName)
        {
            var final = steps.First();

            // Show Raw Data Table (only x and y for now)
            var rawHeaders = new List<string> { "x", "y" };
            var rawRows = new List<List<string>>();

            for (int i = 0; i < final.XValues.Count; i++)
            {
                rawRows.Add(new List<string>
        {
            final.XValues[i].ToString("F4"),
            final.YValues[i].ToString("F4")
        });
            }

            ShowTable(page, stack, rawHeaders, rawRows);

            // Equation Text
            var functionText = new TextBlock
            {
                Text = $"Function: {steps.Last().Equation}",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.LightGreen),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            var xData = new ObservableCollection<double>(final.XValues);
            var yData = new ObservableCollection<double>(final.YValues);

            // Polynomial coefficients (assumed already in the Step)
            var coefficients = final.PolynomialCoefficients;

            // Chart for polynomial regression
            var polyChart = ChartHelper.CreatePolynomialRegressionChart(
                xData, yData, coefficients, steps.Last().Equation);
            ChartHelper.AddChartToDisplay(page, stack, polyChart);

            Debug.Write("Displayed polynomial regression chart");
            stack.Children.Add(functionText);

            // AI Explanation
            string summaryPrompt = $@"
            The polynomial regression was calculated using a degree-{coefficients.Count - 1} polynomial.
            The resulting equation is: {steps.Last().Equation}.
            Briefly explain what this polynomial means in the context of regression.
            Don't use bold text. Just keep it clear and friendly.";

            string reply = await LlamaTextAI.GetLlamaResponseAsync(summaryPrompt);

            var summaryText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            int rowDelay = 1500;
            int totalRows = final.XValues.Count + 1;
            int totalDelay = rowDelay * totalRows;
            await Task.Delay(totalDelay);
            stack.Children.Add(summaryText);
            page.ScrollToBottom();

            for (int i = 1; i <= reply.Length; i++)
            {
                summaryText.Text = reply.Substring(0, i);
                await Task.Delay(10);
            }
        }


        public static async void DisplaySimpson(MainPage page, StackPanel stack, List<IntegrationStep> steps, string methodName)
        {
            var final = steps.Last();

            // Show Raw Data Table (x and f(x))
            var rawHeaders = new List<string> { "x", "f(x)" };
            var rawRows = new List<List<string>>();

            for (int i = 0; i < final.XValues.Count; i++)
            {
                rawRows.Add(new List<string>
        {
            final.XValues[i].ToString("F4"),
            final.YValues[i].ToString("F4")
        });
            }

            ShowTable(page, stack, rawHeaders, rawRows);

            // Create and add the function plot chart
            // We will plot the (x, y) points from the integration step
            var points = new ObservableCollection<ObservablePoint>();
            for (int i = 0; i < final.XValues.Count; i++)
            {
                points.Add(new ObservablePoint(final.XValues[i], final.YValues[i]));
            }

            var functionSeries = new LineSeries<ObservablePoint>
            {
                Values = points,
                DataPadding = new LvcPoint(0, 0),
                GeometryStroke = null,
                Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 5 },
                GeometryFill = null,

                Name = "Function f(x)"
            };

            var chart = new CartesianChart
            {
                Series = new ISeries[] { functionSeries },
                Height = 350,
                Width = 500,
                Background = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, 10, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom
            };

            // Add the chart to the stack panel
            stack.Children.Add(new Border { Child = chart });

            // Result Text
            var resultText = new TextBlock
            {
                Text = $"{final.Method} Result: {final.Result:F6}",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.LightGreen),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            stack.Children.Add(resultText);

            // AI Explanation Prompt
            string summaryPrompt = $@"
    Using {final.Method} with {final.SubIntervals} intervals from {final.IntervalA} to {final.IntervalB},
    the estimated integral of the function is approximately {final.Result:F6}.
    Explain briefly how {final.Method} works and what this result means, in a clear and friendly way.";

            string reply = await DeepSeekTextAI.GetLlamaResponseAsync(summaryPrompt);

            var summaryText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            int delayBeforeSummary = 1500;
            await Task.Delay(delayBeforeSummary);

            stack.Children.Add(summaryText);
            page.ScrollToBottom();

            // Animate the explanation typing
            for (int i = 1; i <= reply.Length; i++)
            {
                summaryText.Text = reply.Substring(0, i);
                await Task.Delay(10);
            }
        }


        public static async void DisplayTrapezoidal(MainPage page, StackPanel stack, List<IntegrationStep> steps, string methodName)
        {
            var final = steps.Last();

            // --- Show Raw Data Table (x and f(x)) ---
            var rawHeaders = new List<string> { "x", "f(x)" };
            var rawRows = new List<List<string>>();

            for (int i = 0; i < final.XValues.Count; i++)
            {
                rawRows.Add(new List<string>
                {
                    final.XValues[i].ToString("F4"),
                    final.YValues[i].ToString("F4")
                });
            }

            ShowTable(page, stack, rawHeaders, rawRows);

            // --- Prepare Trapezoidal Points (zigzag) ---
            var trapezoidalPoints = new ObservableCollection<ObservablePoint>();
            for (int i = 0; i < final.XValues.Count; i++)
            {
                trapezoidalPoints.Add(new ObservablePoint(final.XValues[i], final.YValues[i]));
            }

            // --- Prepare Real Function Curve Points ---
            var functionPoints = new ObservableCollection<ObservablePoint>();
            int dense = 200;
            double stepSize = (final.IntervalB - final.IntervalA) / dense;

            // Extract all variable names from the formula (except 'x')
            var variableRegex = new Regex(@"\b([a-zA-Z_][a-zA-Z0-9_]*)\b");
            var matches = variableRegex.Matches(final.Formula);
            var variables = matches
                .Select(m => m.Value)
                .Where(v => v != "x" && !double.TryParse(v, out _))
                .Distinct()
                .ToList();

            // You must provide values for these variables. Here, we set them to 0 as a placeholder.
            // Replace with actual values as needed.
            var extraVariables = new Dictionary<string, double>();
            foreach (var v in variables)
                extraVariables[v] = 0; // TODO: Replace 0 with the actual value for each variable

            for (int i = 0; i <= dense; i++)
            {
                double x = final.IntervalA + i * stepSize;
                var mathParser = new MathParser();
                mathParser.LocalVariables["x"] = x;
                foreach (var kvp in extraVariables)
                    mathParser.LocalVariables[kvp.Key] = kvp.Value;

                double y;
                try
                {
                    y = mathParser.Parse(final.Formula);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error parsing formula '{final.Formula}' at x={x}: {ex.Message}");
                    y = double.NaN;
                }
                functionPoints.Add(new ObservablePoint(x, y));
            }

            // --- Function Curve Series (f(x)) ---
            var functionSeries = new LineSeries<ObservablePoint>
            {
                Values = trapezoidalPoints,
                DataPadding = new LvcPoint(0, 0),
                GeometryStroke = null,
                Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 3 },
                GeometryFill = null,
                Name = "f(x)"
            };

            // --- Trapezoidal Connection Series ---
            var trapezoidSeries = new LineSeries<ObservablePoint>
            {
                Values = trapezoidalPoints,
                Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 2 },
                GeometryStroke = new SolidColorPaint(SKColors.White),
                GeometryFill = new SolidColorPaint(SKColors.White),
                LineSmoothness = 0,
                Name = "Trapezoids"
            };

            // --- Create Chart ---
            var chart = new CartesianChart
            {
                Series = new ISeries[] { functionSeries, trapezoidSeries },
                Height = 350,
                Width = 500,
                Background = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, 10, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom,
                LegendTextPaint = new SolidColorPaint(SKColors.White),
            };

            stack.Children.Add(new Border { Child = chart });

            // --- Result Text ---
            var resultText = new TextBlock
            {
                Text = $"{final.Method} Result: {final.Result:F6}",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.LightGreen),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            stack.Children.Add(resultText);

            // --- AI Explanation Prompt ---
            string summaryPrompt = $@"
        Using {final.Method} with {final.SubIntervals} intervals from {final.IntervalA} to {final.IntervalB},
        the estimated integral of the function is approximately {final.Result:F6}.
        Explain briefly how {final.Method} works and what this result means, in a clear and friendly way.";

            string reply = await DeepSeekTextAI.GetLlamaResponseAsync(summaryPrompt);

            var summaryText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };

            await Task.Delay(1500);
            stack.Children.Add(summaryText);
            page.ScrollToBottom();

            for (int i = 1; i <= reply.Length; i++)
            {
                summaryText.Text = reply.Substring(0, i);
                await Task.Delay(10);
            }
        }






        public static async Task ShowTable(MainPage page, StackPanel chatStack, List<string> headers, List<List<string>> rows)
        {
            var table = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(10)
            };

            // Define columns based on headers
            for (int i = 0; i < headers.Count; i++)
            {
                table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Add header row immediately
            table.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int i = 0; i < headers.Count; i++)
            {
                var headerText = new TextBlock
                {
                    Text = headers[i],
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(8),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetRow(headerText, 0);
                Grid.SetColumn(headerText, i);
                table.Children.Add(headerText);

            }

            chatStack.Children.Add(table);

            // Add data rows with delay for typing effect
            for (int row = 0; row < rows.Count; row++)
            {
                 
                table.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                for (int col = 0; col < rows[row].Count; col++)
                {
                    page.ScrollToBottom();
                    
                    var cellText = new TextBlock
                    {
                        Text = "", // start empty for typing effect
                        FontSize = 16,
                        FontWeight = FontWeights.Normal,
                        Foreground = new SolidColorBrush(Colors.White),
                        Margin = new Thickness(8),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    Grid.SetRow(cellText, row + 1);
                    Grid.SetColumn(cellText, col);
                    table.Children.Add(cellText);

                    // Typing effect: fill text letter by letter
                    string fullText = rows[row][col]; // Declare the variable outside the loop
                    if (string.IsNullOrEmpty(fullText))
                    {
                        fullText = ""; 
                    }
                    for (int i = 0; i <= fullText.Length; i++)
                    {
                        cellText.Text = fullText.Substring(0, i);
                        await Task.Delay(10); 
                    }
                }
                await Task.Delay(150); 
                
            }
        }
    }
}
