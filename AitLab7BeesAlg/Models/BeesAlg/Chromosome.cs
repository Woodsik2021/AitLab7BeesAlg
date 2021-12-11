using System;
using System.Collections.Generic;

namespace AitLab7BeesAlg.Models.BeesAlg
{
    public class Chromosome
    {
        public double X1 { get; set; }
        public double X2 { get; set; }

        public Chromosome(double x1, double x2, double funcValue) => (X1, X2, FuncValue) = (x1, x2, funcValue);

        public Chromosome(double x1, double x2, Func<double, double, double> function) :
            this(x1, x2, function(x1, x2)) => _ = 1;

        public Chromosome(double x1, double x2) => (X1, X2) = (x1, x2);

        public Chromosome()
        {
            
            
            
            
            
            
            
        }

        public double FuncValue { get; set; }

        public override string ToString() => $"F(x1,x2) = {FuncValue} in X1: {X1}; X2: {X2}";
    }


    public static class ChromosomeExtensions
    {
        public static void EvalFunction(this IEnumerable<Chromosome> chromosomes, Func<double, double, double> function)
        {
            foreach (var chromosome in chromosomes)
            {
                chromosome.FuncValue = function(chromosome.X1, chromosome.X2);
            }
        }
    }
}