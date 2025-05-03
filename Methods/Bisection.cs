using System;
using System.Collections.Generic;
using NCalc;
using RestSharp;
using static MarkGPT.Methods.StepModels;
namespace MarkGPT.Methods
{
    

    public static class Bisection
    {
        static double EvaluateFunction(string fx, double x)
        {
            var parser = new Mathos.Parser.MathParser();

            parser.LocalVariables["x"] = x;

            double result = parser.Parse(fx);

            return result;
        }

        public static List<BisectionStep> Solve(double xl, double xr, double error, string fx)
        {
            var steps = new List<BisectionStep>();
            int iteration = 0;
            double xm = 0;

            while ((xr - xl) >= error)
            {
                xm = (xl + xr) / 2;
                double fxm = EvaluateFunction(fx, xm);
                double fxl = EvaluateFunction(fx, xl);

                steps.Add(new BisectionStep
                {
                    Iteration = ++iteration,
                    Xl = xl,
                    Xr = xr,
                    Xm = xm,
                    Fxm = fxm
                });

                if (fxm == 0.0)
                    break;
                else if (fxm * fxl < 0)
                    xr = xm;
                else
                    xl = xm;
            }

            return steps;
        }
    }
}
