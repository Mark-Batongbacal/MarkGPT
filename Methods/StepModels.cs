using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkGPT.Methods
{
    public class StepModels
    {
        public class BisectionStep
        {
            public int Iteration { get; set; }
            public double Xl { get; set; }
            public double Xr { get; set; }
            public double Xm { get; set; }
            public double Fxm { get; set; }
            public double Root { get; set; }
        }

        public class NewtonRaphsonStep
        {
            public int Iteration { get; set; }
            public double X { get; set; }
            public double Fx { get; set; }
            public double Fdx { get; set; }
            public double Error { get; set; }
            public double Root { get; set; }
        }

        public class SecantStep
        {
            public int Iteration { get; set; }
            public double X0 { get; set; }
            public double X1 { get; set; }
            public double X2 { get; set; }
            public double Fx2 { get; set; }
            public double Root { get; set; }
        }


        public class GaussianStep
        {
            public int Step { get; set; }
            public string Operation { get; set; }
            public string MatrixState { get; set; }
            public List<double> Roots { get; set; } // Track the roots at each step
        }

        public class LinearRegressionStep
        {
            public int Iteration { get; set; } // Not used in current method, but kept for consistency
            public List<double> XValues { get; set; }
            public List<double> YValues { get; set; }
            public List<double> XYValues { get; set; }
            public List<double> XSquaredValues { get; set; }

            public double SumX { get; set; }
            public double SumY { get; set; }
            public double SumXY { get; set; }
            public double SumX2 { get; set; }
            public int N { get; set; }

            public double Slope { get; set; }
            public double Intercept { get; set; }

            public string Equation { get; set; }

            public List<double> PolynomialCoefficients { get; set; }
        }
    }
}
