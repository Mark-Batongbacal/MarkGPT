using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarkGPT.Methods.StepModels;

namespace MarkGPT.Methods
{
    public class LinearRegression
    {
        public static List<LinearRegressionStep> Solve(List<(double x, double y)> points)
        {
            var steps = new List<LinearRegressionStep>();
            int n = points.Count;

            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            // Store individual values for table display
            var xValues = new List<double>();
            var yValues = new List<double>();
            var xyValues = new List<double>();
            var xSquaredValues = new List<double>();

            foreach (var (x, y) in points)
            {
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;

                xValues.Add(x);
                yValues.Add(y);
                xyValues.Add(x * y);
                xSquaredValues.Add(x * x);
            }

            var step1 = new LinearRegressionStep
            {
                Iteration = 1,
                SumX = sumX,
                SumY = sumY,
                SumXY = sumXY,
                SumX2 = sumX2,
                N = n,
                XValues = xValues,
                YValues = yValues,
                XYValues = xyValues,
                XSquaredValues = xSquaredValues
            };
            steps.Add(step1);

            double numeratorM = (n * sumXY) - (sumX * sumY);
            double denominatorM = (n * sumX2) - (sumX * sumX);
            double m = numeratorM / denominatorM;
            double b = (sumY - m * sumX) / n;

            var step2 = new LinearRegressionStep
            {
                Iteration = 2,
                Slope = m,
                Intercept = b,
                Equation = $"y = {m.ToString("0.###", CultureInfo.InvariantCulture)}x + {b.ToString("0.###", CultureInfo.InvariantCulture)}"
            };
            steps.Add(step2);

            var step3 = new LinearRegressionStep
            {
                Iteration = 3,
                Slope = m,
                Intercept = b,
                Equation = $"Fitted Line: y = {m.ToString("0.###", CultureInfo.InvariantCulture)}*x + {b.ToString("0.###", CultureInfo.InvariantCulture)}"
            };
            steps.Add(step3);

            return steps;
        }
    }
}
