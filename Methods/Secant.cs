using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarkGPT.Methods.StepModels;
namespace MarkGPT.Methods
{
    
        

        public static class Secant
        {
            static double EvaluateFunction(string fx, double x)
            {
                var parser = new Mathos.Parser.MathParser();
                parser.LocalVariables["x"] = x;
                return parser.Parse(fx);
            }

            public static List<SecantStep> Solve(double x0, double x1, double error, string fx)
            {
                var steps = new List<SecantStep>();
                int iteration = 0;
                double x2;

                while (Math.Abs(x1 - x0) >= error)
                {
                    double fx0 = EvaluateFunction(fx, x0);
                    double fx1 = EvaluateFunction(fx, x1);

                    x2 = x1 - (fx1 * (x1 - x0)) / (fx1 - fx0);
                    double fx2 = EvaluateFunction(fx, x2);

                    steps.Add(new SecantStep
                    {
                        Iteration = ++iteration,
                        X0 = x0,
                        X1 = x1,
                        X2 = x2,
                        Fx2 = fx2,
                        Root = x2
                    });

                    x0 = x1;
                    x1 = x2;
                }

                return steps;
            }
        }
    }

