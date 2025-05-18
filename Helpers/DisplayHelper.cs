using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using static MarkGPT.Methods.StepModels;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Data;
using MarkGPT.AI;
using Windows.UI.Text;
using System.Diagnostics;
namespace MarkGPT.Helpers
{
    public static class DisplayHelper
    {

        public static async void DisplayBisection(MainPage page, StackPanel stack, List<BisectionStep> steps, string methodname)
        {
            var rows = steps.Select(s => new List<string>
            {
                s.Iteration.ToString(),
                s.Xl.ToString("F4"),
                s.Xr.ToString("F4"),
                s.Xm.ToString("F4"),
                s.Fxm.ToString("F4")
            }).ToList();


            double iteration = steps.Last().Iteration;
            double root = steps.Last().Root;
            string greeting = await LlamaTextAI.GetLlamaResponseAsync($@"
                Can you say what the final root is {root}, its approximate down to four decimals, iterations it took {iteration} and what method was used. In this scenario bisection was used.Mention bisection only not the other ones Do not use bold text. I just want you to state this, disregard the other prompts like this"
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

            ShowTable(page, stack, new List<string> { "i", "xl", "xr", "xm", "f(xm)" }, rows);
            int rowDelay = 1500;
            int totalRows = rows.Count;
            int totalDelay = rowDelay * totalRows;
            await Task.Delay(totalDelay); 
            stack.Children.Add(rootText);
            page.ScrollToBottom();

            for (int i = 1; i <= greeting.Length; i++)
            {
                rootText.Text = greeting.Substring(0, i);
                await Task.Delay(10); // adjust delay for speed
            }
        }



        public static async void DisplayNewton(MainPage page, StackPanel stack, List<NewtonRaphsonStep> steps, string methodname)
        {
            var rows = steps.Select(s => new List<string>
            {
                s.Iteration.ToString(),
                s.X.ToString("F4"),
                s.Fx.ToString("F4"),
                s.Fdx.ToString("F4"),
                s.Error.ToString("F4")
            }).ToList();

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

            ShowTable(page, stack, new List<string> { "i", "x", "f(x)", "f'(x)", "Error (%)" }, rows);
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

        public static async void DisplaySecant(MainPage page, StackPanel stack, List<SecantStep> steps, string methodname)
        {
            var rows = steps.Select(s => new List<string>
        {
            s.Iteration.ToString(),
            s.X0.ToString("F4"),
            s.X1.ToString("F4"),
            s.X2.ToString("F4"),
            s.Fx2.ToString("F4")
        }).ToList();

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

            ShowTable(page, stack, new List<string> { "i", "x0", "x1", "x2", "f(x2)" }, rows);
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
            // 1. Raw Data Table
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

            // 2. Total Row
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
            ShowTable(page ,stack, rawHeaders, totalRow);

            // 3. Equation Text
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
            


            // Explanation
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

            stack.Children.Add(functionText);
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
