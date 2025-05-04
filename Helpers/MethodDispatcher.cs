using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkGPT.Methods;
using Microsoft.UI.Xaml.Controls;

namespace MarkGPT.Helpers
{
    public static class MethodDispatcher
    {
        public static (string botMessage, Action displayAction) Dispatch(MainPage page,string[] parts, StackPanel chatStack)
        {
            string methodCode = parts[1];
            switch (methodCode)
            {
                case "1B":
                    double xl = Convert.ToDouble(parts[2]);
                    double xr = Convert.ToDouble(parts[3]);
                    string fx = parts[4];
                    double error = Convert.ToDouble(parts[5]);
                    var bisResult = Bisection.Solve(xl, xr, error, fx);
                    return (null, () =>
                        DisplayHelper.DisplayBisection(page, chatStack, bisResult));

                case "1NR":
                    double x0 = Convert.ToDouble(parts[2]);
                    string fx_nr = parts[3];
                    string dfx_nr = parts[4];
                    double error_nr = Convert.ToDouble(parts[5]);
                    var nrResult = NewtonRaphson.Solve(x0, error_nr, fx_nr, dfx_nr);
                    return (null, () =>
                        DisplayHelper.DisplayNewton(page, chatStack, nrResult));

                case "1S":
                    double x0_s = Convert.ToDouble(parts[2]);
                    double x1_s = Convert.ToDouble(parts[3]);
                    string fx_s = parts[4];
                    double error_s = Convert.ToDouble(parts[5]);
                    var secResult = Secant.Solve(x0_s, x1_s, error_s, fx_s);
                    return (null, () =>
                        DisplayHelper.DisplaySecant(page, chatStack, secResult));

                //case "1NS":
                //    
                //    break;

                //case "1NS":
                //    
                //    break;

                //case "1NS":
                //    
                //    break;

                //case "1NS":
                //    
                //    break;

                //case "1NS":
                //    
                //    break;

                //case "1NS":
                //    
                //    break;

                //case "1NS":
                //    
                //    break;

                //case "1NS":
                //    
                //    break;
                default:
                    return ("Unrecognized method code. Please check your input format.", null);
            }
        }
    }

}
