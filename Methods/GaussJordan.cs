using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarkGPT.Methods.StepModels;

namespace MarkGPT.Methods
{
    public static class GaussJordan
    {
        public static List<GaussianStep> Solve(double[,] matrix, double[] vector)
        {
            var steps = new List<GaussianStep>();
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

            // Add initial matrix state as step 0
            steps.Add(new GaussianStep
            {
                Step = 0,
                MatrixState = FormatMatrix(augmentedMatrix, n),
                Roots = GetRoots(augmentedMatrix, n),
                Operation = "Initial Matrix"
            });

            // Perform Gauss-Jordan Elimination
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
                    for (int k = 0; k <= n; k++) // Include the constant column
                    {
                        double temp = augmentedMatrix[i, k];
                        augmentedMatrix[i, k] = augmentedMatrix[maxRow, k];
                        augmentedMatrix[maxRow, k] = temp;
                    }

                    // Add step for row swap
                    steps.Add(new GaussianStep
                    {
                        Step = steps.Count,
                        MatrixState = FormatMatrix(augmentedMatrix, n),
                        Roots = GetRoots(augmentedMatrix, n),
                        Operation = $"R{i + 1} <-> R{maxRow + 1} (Row Swap)"
                    });
                }

                // Scale the pivot row to make the pivot element 1
                double pivot = augmentedMatrix[i, i];
                if (Math.Abs(pivot) < 1e-10) // Handle small pivots
                {
                    steps.Add(new GaussianStep
                    {
                        Step = steps.Count,
                        MatrixState = FormatMatrix(augmentedMatrix, n),
                        Roots = GetRoots(augmentedMatrix, n),
                        Operation = $"Pivot at R{i + 1} is too small, system may be singular."
                    });
                    continue; // Skip this row
                }

                if (Math.Abs(pivot - 1.0) > 1e-10) // Only scale if not already 1
                {
                    for (int j = i; j <= n; j++) // Include the constant column
                    {
                        augmentedMatrix[i, j] /= pivot;
                    }

                    // Add step for scaling
                    steps.Add(new GaussianStep
                    {
                        Step = steps.Count,
                        MatrixState = FormatMatrix(augmentedMatrix, n),
                        Roots = GetRoots(augmentedMatrix, n),
                        Operation = $"R{i + 1} = R{i + 1} / {pivot:F4} (Scale to make pivot = 1)"
                    });
                }

                // Eliminate all other elements in column i
                for (int k = 0; k < n; k++)
                {
                    if (k != i)
                    {
                        double factor = augmentedMatrix[k, i];
                        if (Math.Abs(factor) > 1e-10) // Only perform elimination if needed
                        {
                            for (int j = i; j <= n; j++) // Include the constant column
                            {
                                augmentedMatrix[k, j] -= factor * augmentedMatrix[i, j];
                            }

                            // Add step for each row elimination
                            steps.Add(new GaussianStep
                            {
                                Step = steps.Count,
                                MatrixState = FormatMatrix(augmentedMatrix, n),
                                Roots = GetRoots(augmentedMatrix, n),
                                Operation = $"R{k + 1} = R{k + 1} - ({factor:F4})R{i + 1}"
                            });
                        }
                    }
                }
            }

            // Add final solution as the last step if not already added
            if (steps.Last().Operation != "Final Solution")
            {
                steps.Add(new GaussianStep
                {
                    Step = steps.Count,
                    MatrixState = FormatMatrix(augmentedMatrix, n),
                    Roots = GetRoots(augmentedMatrix, n),
                    Operation = "Final Solution"
                });
            }

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
                }
                if (i < n - 1)
                    sb.AppendLine();
            }
            return sb.ToString();
        }

        private static List<double> GetRoots(double[,] augmentedMatrix, int n)
        {
            var roots = new List<double>();
            for (int i = 0; i < n; i++)
            {
                roots.Add(augmentedMatrix[i, n]);
            }
            return roots;
        }
    }
}