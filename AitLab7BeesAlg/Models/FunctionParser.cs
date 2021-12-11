using System;
using System.Collections.Generic;
using Jace;

namespace AitLab7BeesAlg.Models
{
    public static class FunctionParser
    {
        public static Func<double, double, double> ParseFunction(string functionStr)
        {
            var calculationEngine = new CalculationEngine();
            var formula = calculationEngine.Build(functionStr);
            var variables = new Dictionary<string, double>();

            return (x1, x2) =>
            {
                variables["x1"] = x1;
                variables["x2"] = x2;
                return formula(variables);
            };
        }
    }
}