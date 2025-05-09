using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MarkGPT.Methods.StepModels;

namespace MarkGPT.Methods
{
    public static class GaussSeidel
    {
        public static List<GaussianStep> Solve(double[,] matrix, double[] vector, double[] initialGuess = null,
            int maxIterations = 100, double tolerance = 1e-6)
        {
            var steps = new List<GaussianStep>();
            int n = matrix.GetLength(0);

            // Initialize solution vector
            double[] x = initialGuess != null ? (double[])initialGuess.Clone() : new double[n];
            if (initialGuess == null)
            {
                // Default initial guess: set all to 0
                for (int i = 0; i < n; i++)
                    x[i] = 0;
            }

            // Create augmented matrix for display purposes
            double[,] augmentedMatrix = new double[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmentedMatrix[i, j] = matrix[i, j];
                }
                augmentedMatrix[i, n] = vector[i];
            }

            // Add initial state
            steps.Add(new GaussianStep
            {
                Step = 0,
                MatrixState = FormatMatrix(augmentedMatrix, n),
                Roots = x.ToList(),
                Operation = "Initial Matrix and Guess"
            });

            // Check for diagonal dominance
            bool isDiagonallyDominant = true;
            for (int i = 0; i < n; i++)
            {
                double rowSum = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i != j)
                        rowSum += Math.Abs(matrix[i, j]);
                }

                if (Math.Abs(matrix[i, i]) <= rowSum)
                {
                    isDiagonallyDominant = false;
                    steps.Add(new GaussianStep
                    {
                        Step = steps.Count,
                        MatrixState = FormatMatrix(augmentedMatrix, n),
                        Roots = x.ToList(),
                        Operation = "Warning: Matrix is not diagonally dominant. Method may not converge."
                    });
                    break;
                }
            }

            // Perform Gauss-Seidel iterations
            int iteration = 0;
            double error = double.MaxValue;

            while (iteration < maxIterations && error > tolerance)
            {
                error = 0;
                StringBuilder iterationDetails = new StringBuilder();
                iterationDetails.AppendLine($"Iteration {iteration + 1}:");

                for (int i = 0; i < n; i++)
                {
                    double oldValue = x[i];
                    double sum1 = 0; // Sum for already computed values in current iteration
                    double sum2 = 0; // Sum for values from previous iteration

                    for (int j = 0; j < i; j++)
                    {
                        sum1 += matrix[i, j] * x[j];
                    }

                    for (int j = i + 1; j < n; j++)
                    {
                        sum2 += matrix[i, j] * x[j];
                    }

                    x[i] = (vector[i] - sum1 - sum2) / matrix[i, i];

                    // Calculate local error
                    double localError = Math.Abs(x[i] - oldValue);
                    error = Math.Max(error, localError);

                    iterationDetails.AppendLine($"    x{i + 1} = {x[i]:F6} (change: {localError:F6})");
                }

                iteration++;

                // Record step for this iteration
                steps.Add(new GaussianStep
                {
                    Step = steps.Count,
                    MatrixState = FormatMatrix(augmentedMatrix, n),
                    Roots = x.ToList(),
                    Operation = $"{iterationDetails.ToString().TrimEnd()}\nCurrent Error: {error:F6}"
                });

                // Don't add more than 20 steps to avoid overwhelming output
                if (steps.Count > 20 && iteration < maxIterations - 1 && error > tolerance)
                {
                    steps.Add(new GaussianStep
                    {
                        Step = steps.Count,
                        MatrixState = FormatMatrix(augmentedMatrix, n),
                        Roots = x.ToList(),
                        Operation = "Partial results shown. Continuing iterations..."
                    });

                    // Skip to near the end
                    while (iteration < maxIterations - 1 && error > tolerance)
                    {
                        error = 0;

                        for (int i = 0; i < n; i++)
                        {
                            double oldValue = x[i];
                            double sum1 = 0;
                            double sum2 = 0;

                            for (int j = 0; j < i; j++)
                            {
                                sum1 += matrix[i, j] * x[j];
                            }

                            for (int j = i + 1; j < n; j++)
                            {
                                sum2 += matrix[i, j] * x[j];
                            }

                            x[i] = (vector[i] - sum1 - sum2) / matrix[i, i];
                            error = Math.Max(error, Math.Abs(x[i] - oldValue));
                        }

                        iteration++;
                    }
                }
            }

            // Add final result
            string finalMessage = error <= tolerance
                ? $"Converged after {iteration} iterations. Final Error: {error:F6}"
                : $"Failed to converge after {maxIterations} iterations. Final Error: {error:F6}";

            steps.Add(new GaussianStep
            {
                Step = steps.Count,
                MatrixState = FormatMatrix(augmentedMatrix, n),
                Roots = x.ToList(),
                Operation = finalMessage
            });

            return steps;
        }

        private static string FormatMatrix(double[,] matrix, int n)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    sb.Append(matrix[i, j].ToString("F4"));
                    if (j < n)
                        sb.Append(", ");
                    else if (j == n)
                        sb.Append(" | " + matrix[i, j].ToString("F4"));
                }
                if (i < n - 1)
                    sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}