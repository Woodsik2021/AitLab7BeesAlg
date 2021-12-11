using System;
using System.Collections.Generic;
using System.Linq;
using AitLab7BeesAlg.Models.Extension;
using MathNet.Numerics.Distributions;

namespace AitLab7BeesAlg.Models.BeesAlg
{
    public interface IMutationAlg
    {
        public void Mutate(IList<Chromosome> currentPopulation);
    }

    public abstract class MutationAlg : IMutationAlg
    {
        protected Random Random { get; }

        public (double a, double b) X1Bounds { get; }
        public (double a, double b) X2Bounds { get; }

        public double MutationProbability { get; init; } = 0.1;

        protected MutationAlg((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null)
        {
            X1Bounds = x1Bounds;
            X2Bounds = x2Bounds;
            Random = random ?? new Random();
        }


        public abstract void Mutate(IList<Chromosome> currentPopulation);
    }

    public class UniformMutation : MutationAlg
    {
        public UniformMutation((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null) :
            base(x1Bounds, x2Bounds, random)
        {
        }

        public override void Mutate(IList<Chromosome> currentPopulation)
        {
            foreach (var chromosome in currentPopulation)
            {
                chromosome.X1 = MutationProbability < Random.NextDouble()
                    ? chromosome.X1
                    : ContinuousUniform.Sample(Random, X1Bounds.a, X1Bounds.b);
                chromosome.X2 = MutationProbability < Random.NextDouble()
                    ? chromosome.X2
                    : ContinuousUniform.Sample(Random, X2Bounds.a, X2Bounds.b);
            }
        }
    }

    public class GaussianMutation : MutationAlg
    {
        public double StdDevPercent { get; init; } = 0.1;

        public GaussianMutation((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null) :
            base(x1Bounds, x2Bounds, random)
        {
        }


        public override void Mutate(IList<Chromosome> currentPopulation)
        {
            foreach (var chromosome in currentPopulation)
            {
                chromosome.X1 = MutationProbability < Random.NextDouble()
                    ? chromosome.X1
                    : Math.Clamp(
                        Normal.Sample(Random, chromosome.X1, StdDevPercent * (X1Bounds.b - X1Bounds.a)),
                        X1Bounds.a, X1Bounds.b);
                chromosome.X2 = MutationProbability < Random.NextDouble()
                    ? chromosome.X2
                    : Math.Clamp(
                        Normal.Sample(Random, chromosome.X2, StdDevPercent * (X2Bounds.b - X2Bounds.a)),
                        X2Bounds.a, X2Bounds.b);
            }
        }
    }

    public class NeighborhoodMutation : MutationAlg
    {
        private readonly Func<double, double, double> _objFunc;

        public NeighborhoodMutation((double a, double b) x1Bounds, (double a, double b) x2Bounds,
            Func<double, double, double> objFunc, Random? random = null) : base(x1Bounds, x2Bounds, random)
        {
            _objFunc = objFunc;
        }

        public override void Mutate(IList<Chromosome> currentPopulation)
        {
            var tempPop = currentPopulation.Select(chromosome => new Chromosome()
            {
                X1 = chromosome.X1,
                X2 = chromosome.X2,
                FuncValue = chromosome.FuncValue
            }).ToList();

            var i = -1;
            foreach (var chr in currentPopulation)
            {
                i++;
                
                var rndNeighbor = tempPop.GetRandomElementsExcept(tempPop[i], Random).First();
                var rndDimension = DiscreteUniform.Sample(Random, 1, 2);
                
                switch (rndDimension)
                {
                    case 1:
                        var newX1 = 
                            chr.X1 + ContinuousUniform.Sample(Random, -1, 1) * (chr.X1 - rndNeighbor.X1)
                            ;
                        var newX1FuncValue = _objFunc(newX1, chr.X2);
                        if (newX1FuncValue + 1e-10 < chr.FuncValue)
                        {
                            chr.X1 = newX1;
                            chr.FuncValue = newX1FuncValue;
                            //(chr.X1, chr.FuncValue) = (newX1, newX1FuncValue);
                        }
                        break;
                    case 2:
                        var newX2 = 
                            chr.X2 + ContinuousUniform.Sample(Random, -1, 1) * (chr.X2 - rndNeighbor.X2)
                            ;
                        var newX2FuncValue = _objFunc(chr.X1, newX2);
                        if (newX2FuncValue + 1e-10 < chr.FuncValue)
                        {
                            chr.X2 = newX2;
                            chr.FuncValue = newX2FuncValue;
                            //(chr.X2, chr.FuncValue) = (newX2, newX2FuncValue);
                        }
                        break;

                    default: throw new InvalidOperationException("Dimension out of bounds");
                }
            }
        }
    }
}