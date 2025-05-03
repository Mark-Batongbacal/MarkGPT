using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MarkGPT.Methods.StepModels;

namespace MarkGPT.Methods
    {
       
        public static class NewtonRaphson
        {
            static double EvaluateFunction(string fx, double x)
            {
                var parser = new Mathos.Parser.MathParser(); 
                parser.LocalVariables["x"] = x;
                return parser.Parse(fx);
            }

            public static List<NewtonRaphsonStep> Solve(double x0, double allowedError, string fx, string dfx)
            {
                var steps = new List<NewtonRaphsonStep>();
                int iteration = 0;
                double error = 100.0;

                while (error > allowedError)
                {
                    double fxVal = EvaluateFunction(fx, x0);
                    double dfxVal = EvaluateFunction(dfx, x0);

                    if (dfxVal == 0)
                        break;

                    double x1 = x0 - fxVal / dfxVal;
                    error = Math.Abs((x1 - x0) / x1) * 100;

                    steps.Add(new NewtonRaphsonStep
                    {
                        Iteration = ++iteration,
                        X = x0,
                        Fx = fxVal,
                        Fdx = dfxVal,
                        Error = error
                    });

                    x0 = x1;
                }

                return steps;
            }
        }
    }

