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
        }

        public class NewtonRaphsonStep
        {
            public int Iteration { get; set; }
            public double X { get; set; }
            public double Fx { get; set; }
            public double Fdx { get; set; }
            public double Error { get; set; }
        }

        public class SecantStep
        {
            public int Iteration { get; set; }
            public double X0 { get; set; }
            public double X1 { get; set; }
            public double X2 { get; set; }
            public double Fx2 { get; set; }
        }

    }
}
