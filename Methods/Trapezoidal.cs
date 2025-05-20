using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static MarkGPT.Methods.StepModels;

namespace MarkGPT.Methods
{
    internal class Trapezoidal
    {
        public static List<IntegrationStep> Solve(Func<double, double> f, double a, double b, int n)
        {
            var steps = new List<IntegrationStep>();

            double h = (b - a) / n;
            double sum = (f(a) + f(b)) / 2;

            var xValues = new List<double> { a };
            var yValues = new List<double> { f(a) };

            for (int i = 1; i < n; i++)
            {
                double x = a + i * h;
                double fx = f(x);

                sum += fx;
                xValues.Add(x);
                yValues.Add(fx);
            }

            xValues.Add(b);
            yValues.Add(f(b));

            double result = h * sum;

            var step = new IntegrationStep
            {
                Iteration = 1,
                Method = "Trapezoidal Rule",
                Result = result,
                XValues = xValues,
                YValues = yValues,
                IntervalA = a,
                IntervalB = b,
                SubIntervals = n,
                Formula = $"h * [0.5 * f(a) + Σf(x_i) + 0.5 * f(b)] = {result.ToString("0.#####", CultureInfo.InvariantCulture)}"
            };

            steps.Add(step);
            return steps;
        }
    }
}
