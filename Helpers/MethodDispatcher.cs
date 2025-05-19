using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkGPT.Methods;
using Microsoft.UI.Xaml.Controls;

namespace MarkGPT.Helpers
{
    public static class MethodDispatcher
    {
        static double EvaluateFunction(string fx, double x)
        {
            var parser = new Mathos.Parser.MathParser();

            parser.LocalVariables["x"] = x;

            double result = parser.Parse(fx);

            return result;
        }
        public static List<(double x, double y)> ParsePoints(string input)
        {
            var pairs = input.Split(',');
            var points = new List<(double x, double y)>();

            foreach (var pair in pairs)
            {
                var parts = pair.Split(';');
                if (parts.Length != 2) continue;

                if (double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                    double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                {
                    points.Add((x, y));
                }
            }

            Debug.Write(points);
            return points;
        }

        public static (string botMessage, Action displayAction) Dispatch(MainPage page, string[] parts, StackPanel chatStack)
        {

            string methodCode = parts[1];
            string methodName = ""; // Declare methodName to be passed

            switch (methodCode)
            {
                case "1B":  // BISECTION METHOD
                    methodName = "Bisection";
                    double xl = Convert.ToDouble(parts[2]);
                    double xr = Convert.ToDouble(parts[3]);
                    string fx = parts[4];
                    double error = Convert.ToDouble(parts[5]);

                    // Create the delegate that uses your Mathos parser
                    Func<double, double> function = x => EvaluateFunction(fx, x);

                    var bisResult = Bisection.Solve(xl, xr, error, fx);
                    return ("", () =>
                        DisplayHelper.DisplayBisection(page, chatStack, bisResult, methodName, function));

                case "1NR":  // NEWTON RAPHSON METHOD
                    methodName = "Newton-Raphson";
                    double x0 = Convert.ToDouble(parts[2]);
                    string fx_nr = parts[3];
                    string dfx_nr = parts[4];
                    double error_nr = Convert.ToDouble(parts[5]);

                    // Create the delegate for the function using Mathos parser
                    Func<double, double> function_nr = x => EvaluateFunction(fx_nr, x);

                    var nrResult = NewtonRaphson.Solve(x0, error_nr, fx_nr, dfx_nr);
                    return ("", () =>
                        DisplayHelper.DisplayNewton(page, chatStack, nrResult, methodName, function_nr));

                case "1S":  // SECANT METHOD
                    methodName = "Secant";
                    double x0_s = Convert.ToDouble(parts[2]);
                    double x1_s = Convert.ToDouble(parts[3]);
                    string fx_s = parts[4];
                    double error_s = Convert.ToDouble(parts[5]);

                    // Create the delegate for the function using Mathos parser
                    Func<double, double> function_s = x => EvaluateFunction(fx_s, x);

                    var secResult = Secant.Solve(x0_s, x1_s, error_s, fx_s);
                    return ("", () =>
                        DisplayHelper.DisplaySecant(page, chatStack, secResult, methodName, function_s));


                case "2GE":  //GAUSSIAN ELIMINATION METHOD
                    methodName = "Gaussian Elimination";
                    int n_ge = Convert.ToInt32(parts[2]);
                    double[,] matrix_ge = new double[n_ge, n_ge];
                    double[] vector_ge = new double[n_ge];

                    for (int i = 0; i < n_ge; i++)
                    {
                        var row = parts[3 + i].Split(',');
                        for (int j = 0; j < n_ge; j++)
                        {
                            matrix_ge[i, j] = Convert.ToDouble(row[j]);
                        }
                        vector_ge[i] = Convert.ToDouble(row[n_ge]); // the constant term (b)
                    }

                    var geSteps = GaussianElimination.Solve(matrix_ge, vector_ge);
                    return ("", () =>
                        DisplayHelper.DisplayGaussian(page, chatStack, geSteps, methodName));

                case "2GJ":  //GAUSS JORDAN METHOD
                    methodName = "Gauss Jordan";
                    int n_gj = Convert.ToInt32(parts[2]);
                    double[,] matrix_gj = new double[n_gj, n_gj];
                    double[] vector_gj = new double[n_gj];

                    for (int i = 0; i < n_gj; i++)
                    {
                        var row = parts[3 + i].Split(',');
                        for (int j = 0; j < n_gj; j++)
                        {
                            matrix_gj[i, j] = Convert.ToDouble(row[j]);
                        }
                        vector_gj[i] = Convert.ToDouble(row[n_gj]); // the constant term (b)
                    }

                    var gjSteps = GaussJordan.Solve(matrix_gj, vector_gj);
                    return ("", () =>
                        DisplayHelper.DisplayGaussian(page, chatStack, gjSteps, methodName));

                case "2GS":  // GAUSS SEIDEL METHOD
                    methodName = "Gauss Seidel";
                    int n_gs = Convert.ToInt32(parts[2]);
                    double[,] matrix_gs = new double[n_gs, n_gs];
                    double[] vector_gs = new double[n_gs];

                    // Extract matrix and vector
                    for (int i = 0; i < n_gs; i++)
                    {
                        var row = parts[3 + i].Split(',');
                        for (int j = 0; j < n_gs; j++)
                        {
                            matrix_gs[i, j] = Convert.ToDouble(row[j]);
                        }
                        vector_gs[i] = Convert.ToDouble(row[n_gs]); // the constant term (b)
                    }

                    // Extract initial guess
                    var initialGuessRow = parts[3 + n_gs].Split(',');
                    double[] initialGuess_gs = new double[n_gs];
                    for (int i = 0; i < n_gs; i++)
                    {
                        initialGuess_gs[i] = Convert.ToDouble(initialGuessRow[i]);
                    }

                    // Solve using Gauss-Seidel method with the provided initial guess
                    var gsSteps = GaussSeidel.Solve(matrix_gs, vector_gs, initialGuess_gs);
                    return ("", () =>
                        DisplayHelper.DisplayGaussian(page, chatStack, gsSteps, methodName));

                case "3LR":  // LINEAR REGRESSION METHOD
                    Debug.Write("The method is linear regression");
                    methodName = "Linear Regression";
                    string pointsInput = parts[2];  // Example: "1,2;2,3;3,5"

                    // Parse the points from the string (assuming they are comma-separated)
                    var points = ParsePoints(pointsInput);

                    // Call the Linear Regression method to get the result
                    var lrResult = LinearRegression.Solve(points);

                    // Return the formatted response for the display
                    return ("", () =>
                        DisplayHelper.DisplayLinearRegression(page, chatStack, lrResult, methodName));

                default:
                    return ("Unrecognized method code. Please check your input format.", null);

                case "3PR":  // POLYNOMIAL REGRESSION METHOD
                    Debug.Write("The method is polynomial regression");
                    methodName = "Polynomial Regression";

                    string polyPointsInput = parts[2];   // Example: "1,2;2,3;3,7"
                    int degree = int.Parse(parts[3]);    // Degree of polynomial

                    var polyPoints = ParsePoints(polyPointsInput);

                    // Solve polynomial regression
                    var polyResult = PolynomialRegression.Solve(polyPoints, degree);

                    return ("", () =>
                        DisplayHelper.DisplayPolynomialRegression(page, chatStack, polyResult, methodName));
            }
        }
    }
}
