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

namespace MarkGPT.Helpers
{
    public static class DisplayHelper
    {

        public static void DisplayBisection(StackPanel stack, List<BisectionStep> steps)
        {
            ShowTable(stack,
                new List<string> { "Iter", "xl", "xr", "xm", "f(xm)" },
                steps.Select(s => new List<string>
                {
                s.Iteration.ToString(),
                s.Xl.ToString("F4"),
                s.Xr.ToString("F4"),
                s.Xm.ToString("F4"),
                s.Fxm.ToString("F4")
                }).ToList()
            );
        }

        public static void DisplayNewton(StackPanel stack, List<NewtonRaphsonStep> steps)
        {
            ShowTable(stack,
                new List<string> { "Iter", "x", "f(x)", "f'(x)", "Error (%)" },
                steps.Select(s => new List<string>
                {
                s.Iteration.ToString(),
                s.X.ToString("F4"),
                s.Fx.ToString("F4"),
                s.Fdx.ToString("F4"),
                s.Error.ToString("F4")
                }).ToList()
            );
        }

        public static void DisplaySecant(StackPanel stack, List<SecantStep> steps)
        {
            ShowTable(stack,
                new List<string> { "Iter", "x0", "x1", "x2", "f(x2)" },
                steps.Select(s => new List<string>
                {
                s.Iteration.ToString(),
                s.X0.ToString("F4"),
                s.X1.ToString("F4"),
                s.X2.ToString("F4"),
                s.Fx2.ToString("F4")
                }).ToList()
            );
        }


        public static void ShowTable(StackPanel chatStack, List<string> headers, List<List<string>> rows)
        {
            var table = new StackPanel();

            table.Children.Add(new TextBlock
            {
                Text = string.Join(" | ", headers),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 4),
                Foreground = new SolidColorBrush(Colors.White)
            });

            foreach (var row in rows)
            {
                table.Children.Add(new TextBlock
                {
                    Text = string.Join(" | ", row),
                    Foreground = new SolidColorBrush(Colors.White)
                });
            }

            var border = new Border
            {
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 51, 51, 51)),
                CornerRadius = new CornerRadius(25),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 6),
                Child = table,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 1000
            };

            chatStack.Children.Add(border);
        }
    }
}
