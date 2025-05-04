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

            // Check if allowedError is valid
            if (allowedError <= 0)
            {
                throw new ArgumentException("Allowed error must be greater than 0.");
            }

            // Ensure the loop runs at least once
            while (error > allowedError)
            {
                double fxVal = EvaluateFunction(fx, x0);
                double dfxVal = EvaluateFunction(dfx, x0);

                // Avoid division by zero (checking the derivative value)
                if (dfxVal == 0)
                {
                    throw new InvalidOperationException("Derivative is zero, unable to proceed with Newton-Raphson method.");
                }

                double x1 = x0 - fxVal / dfxVal;
                error = Math.Abs((x1 - x0) / x1) * 100;

                // Add step to the list
                steps.Add(new NewtonRaphsonStep
                {
                    Iteration = ++iteration,
                    X = x0,
                    Fx = fxVal,
                    Fdx = dfxVal,
                    Error = error,
                    Root = x1 // store the new root estimate
                });

                // Update x0 for the next iteration
                x0 = x1;

                // Prevent infinite loops (if error is not reducing)
                if (iteration > 1000) // This is an arbitrary large iteration count; adjust as needed
                {
                    throw new InvalidOperationException("The method did not converge within 1000 iterations.");
                }
            }

            if (steps.Count == 0)
            {
                throw new InvalidOperationException("No steps were added, possibly due to an invalid function or initial guess.");
            }

            return steps;
        }
    }
}
