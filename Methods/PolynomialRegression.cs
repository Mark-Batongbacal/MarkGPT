using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static MarkGPT.Methods.StepModels;

namespace MarkGPT.Methods
{
    public class PolynomialRegression
    {
        public static List<LinearRegressionStep> Solve(List<(double x, double y)> points, int degree)
        {
            var steps = new List<LinearRegressionStep>();
            int n = points.Count;

            var xValues = points.Select(p => p.x).ToList();
            var yValues = points.Select(p => p.y).ToList();

            // Create matrices for normal equations
            double[,] matrix = new double[degree + 1, degree + 2];

            for (int row = 0; row <= degree; row++)
            {
                for (int col = 0; col <= degree; col++)
                {
                    matrix[row, col] = points.Sum(p => Math.Pow(p.x, row + col));
                }
                matrix[row, degree + 1] = points.Sum(p => Math.Pow(p.x, row) * p.y);
            }

            // Solve the system using Gaussian elimination
            double[] coefficients = GaussianElimination(matrix);

            // Format the equation
            var terms = new List<string>();
            for (int i = 0; i < coefficients.Length; i++)
            {
                var coeff = coefficients[i].ToString("0.###", CultureInfo.InvariantCulture);
                if (i == 0)
                    terms.Add($"{coeff}");
                else if (i == 1)
                    terms.Add($"{coeff}x");
                else
                    terms.Add($"{coeff}x^{i}");
            }

            string equation = "y = " + string.Join(" + ", terms);

            var step = new LinearRegressionStep
            {
                Iteration = 1,
                Equation = equation,
                PolynomialCoefficients = coefficients.ToList(), // Optional: define this in your StepModel
                XValues = xValues,
                YValues = yValues
            };

            steps.Add(step);
            return steps;
        }

        private static double[] GaussianElimination(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            for (int i = 0; i < n; i++)
            {
                // Normalize row
                double pivot = matrix[i, i];
                for (int j = 0; j <= n; j++)
                    matrix[i, j] /= pivot;

                // Eliminate below
                for (int k = i + 1; k < n; k++)
                {
                    double factor = matrix[k, i];
                    for (int j = 0; j <= n; j++)
                        matrix[k, j] -= factor * matrix[i, j];
                }
            }

            // Back substitution
            double[] result = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                result[i] = matrix[i, n];
                for (int j = i + 1; j < n; j++)
                    result[i] -= matrix[i, j] * result[j];
            }

            return result;
        }
    }
}
