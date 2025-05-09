using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarkGPT.Methods.StepModels;

namespace MarkGPT.Methods
{
    public static class GaussianElimination
    {
        public static List<GaussianStep> Solve(double[,] matrix, double[] vector)
        {
            var steps = new List<GaussianStep>();
            var operationLogs = new List<string>();
            int n = matrix.GetLength(0);

            // Create augmented matrix
            double[,] augmentedMatrix = new double[n, n + 1];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    augmentedMatrix[i, j] = matrix[i, j];
                }
                augmentedMatrix[i, n] = vector[i]; // Assign constants to last column
            }

            // Perform Gaussian Elimination
            for (int i = 0; i < n; i++)
            {
                // Pivoting: find max element in column for numerical stability
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(augmentedMatrix[k, i]) > Math.Abs(augmentedMatrix[maxRow, i]))
                    {
                        maxRow = k;
                    }
                }

                // Swap rows if necessary
                if (maxRow != i)
                {
                    for (int k = i; k < n + 1; k++)
                    {
                        double temp = augmentedMatrix[i, k];
                        augmentedMatrix[i, k] = augmentedMatrix[maxRow, k];
                        augmentedMatrix[maxRow, k] = temp;
                    }

                    operationLogs.Add($"R{i + 1} <-> R{maxRow + 1}");
                }

                // Scale the pivot row
                double pivot = augmentedMatrix[i, i];
                if (pivot != 1)
                {
                    operationLogs.Add($"R{i + 1} = R{i + 1} / {pivot:F4}");
                }
                for (int j = i; j < n + 1; j++)
                {
                    augmentedMatrix[i, j] /= pivot;
                }

                // Eliminate below
                for (int k = i + 1; k < n; k++)
                {
                    double factor = augmentedMatrix[k, i];
                    if (factor != 0)
                    {
                        operationLogs.Add($"R{k + 1} = R{k + 1} - ({factor:F4})R{i + 1}");
                    }
                    for (int j = i; j < n + 1; j++)
                    {
                        augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                    }
                }

                // Track the matrix state after each step
                var matrixState = string.Join("\n", Enumerable.Range(0, n)
                .Select(r => string.Join(", ", Enumerable.Range(0, n + 1)
                    .Select(c => augmentedMatrix[r, c].ToString("F4")))));

                List<double> roots = null;

                string operation = string.Join("; ", operationLogs);
                operationLogs.Clear();

                steps.Add(new GaussianStep
                {
                    Step = i + 1,
                    Operation = operation,
                    MatrixState = matrixState,
                    Roots = roots
                });
            }

            double[] solution = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                solution[i] = augmentedMatrix[i, n];
                for (int j = i + 1; j < n; j++)
                {
                    solution[i] -= augmentedMatrix[i, j] * solution[j];
                }
            }

            // Add final step with actual roots (after back-substitution)
            steps.Add(new GaussianStep
            {
                Step = steps.Count + 1,
                MatrixState = "Final Solution",
                Roots = solution.ToList()
            });

            return steps;
        }

    }
}
