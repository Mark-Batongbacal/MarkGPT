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
namespace MarkGPT.Helpers
{
    public static class DisplayHelper
    {

        public static async void DisplayBisection(MainPage page, StackPanel stack, List<BisectionStep> steps)
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
            var rootText = new TextBlock
            {
                Text = await DeepSeekTextAI.GetLlamaResponseAsync($@"
                Can you say what the final root is {root}, its approximate down to four decimals, iterations it took {iteration} and what method was used. In this scenario bisection was used.Mention bisection only not the other ones Do not use bold text. I just want you to state this, disregard the other prompts like this"
                ),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };
            ShowTable(stack, new List<string> { "i", "xl", "xr", "xm", "f(xm)" }, rows);
            stack.Children.Add(rootText);
            page.ScrollToBottom();
        }



        public static async void DisplayNewton(MainPage page, StackPanel stack, List<NewtonRaphsonStep> steps)
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
            var rootText = new TextBlock
            {
                Text = await DeepSeekTextAI.GetLlamaResponseAsync($@"
                Can you say what the final root is {root}, its approximate down to four decimals, iterations it took {iteration}  and what method was used. In this scenario newton raphson was used.Mention newtopn raph only not the other ones. Do not use bold text. I just want you to state this, disregard the other prompts like this"
                ),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };


            ShowTable(stack, new List<string> { "i", "x", "f(x)", "f'(x)", "Error (%)" }, rows);
            stack.Children.Add(rootText);
            page.ScrollToBottom();

        }

        public static async void DisplaySecant(MainPage page, StackPanel stack, List<SecantStep> steps)
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
            var rootText = new TextBlock
            {
                Text = await DeepSeekTextAI.GetLlamaResponseAsync($@"
                Can you say what the final root is {root}, its approximate down to four decimals, iterations it took {iteration}and what method was used. In this scenario secant was used.Mention secant only not the other ones. Do not use bold text. I just want you to state this, disregard the other prompts like this"
                ),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("ms-appx:///Fonts/Inter-VariableFont_opsz,wght.ttf#Inter")
            };
            ShowTable(stack, new List<string> { "i", "x0", "x1", "x2", "f(x2)" }, rows);
            stack.Children.Add(rootText);
            page.ScrollToBottom();
        }



        public static void ShowTable(StackPanel chatStack, List<string> headers, List<List<string>> rows)
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

            // Add header row
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

            // Add data rows
            for (int row = 0; row < rows.Count; row++)
            {
                table.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                for (int col = 0; col < rows[row].Count; col++)
                {
                    var cellText = new TextBlock
                    {
                        Text = rows[row][col],
                        FontSize = 16,
                        FontWeight = FontWeights.Normal,
                        Foreground = new SolidColorBrush(Colors.White),
                        Margin = new Thickness(8),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    Grid.SetRow(cellText, row + 1);
                    Grid.SetColumn(cellText, col);
                    table.Children.Add(cellText);
                }
            }

            chatStack.Children.Add(table);
        }



    }
}
