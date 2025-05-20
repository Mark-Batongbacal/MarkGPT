using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static MarkGPT.Methods.StepModels;

namespace MarkGPT.Methods
{
    public class Simpson
    {
        public static List<IntegrationStep> Solve(Func<double, double> f, double a, double b, int n)
        {
            var steps = new List<IntegrationStep>();

            if (n % 2 != 0)
                throw new ArgumentException("n must be even for Simpson's 1/3 rule.");

            double h = (b - a) / n;
            double sum = f(a) + f(b);

            var xValues = new List<double> { a };
            var yValues = new List<double> { f(a) };

            for (int i = 1; i < n; i++)
            {
                double x = a + i * h;
                double fx = f(x);

                sum += (i % 2 == 0) ? 2 * fx : 4 * fx;

                xValues.Add(x);
                yValues.Add(fx);
            }

            xValues.Add(b);
            yValues.Add(f(b));

            double result = (h / 3) * sum;

            var step = new IntegrationStep
            {
                Iteration = 1,
                Method = "Simpson's 1/3 Rule",
                Result = result,
                XValues = xValues,
                YValues = yValues,
                IntervalA = a,
                IntervalB = b,
                SubIntervals = n,
                Formula = $"h/3 * [f(a) + 4f(x_odd) + 2f(x_even) + f(b)] = {result.ToString("0.#####", CultureInfo.InvariantCulture)}"
            };

            steps.Add(step);
            return steps;
        }
    }
}
