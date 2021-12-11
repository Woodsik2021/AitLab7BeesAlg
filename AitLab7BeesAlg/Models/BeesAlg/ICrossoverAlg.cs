using System;
using System.Collections.Generic;
using System.Linq;
using AitLab7BeesAlg.Models.Extension;
using MathNet.Numerics.Distributions;

namespace AitLab7BeesAlg.Models.BeesAlg
{
    public interface ICrossoverAlg
    {
        public List<Chromosome> Crossover(IList<Chromosome> parents, int outPopulationSize);
    }

    //Todo add Bounds
    public abstract class CrossoverAlg : ICrossoverAlg
    {
        protected Random Random { get; }

        public (double a, double b) X1Bounds { get; }
        public (double a, double b) X2Bounds { get; }

        public double CrossingProbability { get; init; } = 0.98;

        protected CrossoverAlg((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null)
        {
            X1Bounds = x1Bounds;
            X2Bounds = x2Bounds;
            Random = random ?? new Random();
        }

        //Todo add Bounds
        public abstract List<Chromosome> Crossover(IList<Chromosome> parents, int outPopulationSize);
    }

    public class ExtendedLineCrossover : CrossoverAlg
    {
        public ExtendedLineCrossover((double a, double b) x1Bounds, (double a, double b) x2Bounds,
            Random? random = null) : base(x1Bounds, x2Bounds, random)
        {
        }

        public override List<Chromosome> Crossover(IList<Chromosome> parents, int outPopulationSize) =>
            parents.GetRandomUniquePairsIndexes(Random).Take(outPopulationSize)
                .Select<(int parent1, int parent2), Chromosome>(parentsIndexes =>
                    CrossingProbability > Random.NextDouble()
                        ? CrossoverBody(parents[parentsIndexes.parent1], parents[parentsIndexes.parent2])
                        : parents[Random.Next() > 0.5 ? parentsIndexes.parent1 : parentsIndexes.parent2]).ToList();


        //Todo add Bounds
        protected virtual Chromosome CrossoverBody(Chromosome parent1, Chromosome parent2)
        {
            var w = ContinuousUniform.Sample(Random, -0.25, 1.25);
            return new Chromosome
            {
                X1 = Math.Clamp(parent1.X1 + w * (parent2.X1 - parent1.X1), X1Bounds.a, X1Bounds.b),
                X2 = Math.Clamp(parent1.X2 + w * (parent2.X2 - parent1.X2), X2Bounds.a, X2Bounds.b)
            };
        }
    }


    public class AlphaCrossover : CrossoverAlg
    {
        public double Alpha { get; init; } = 0.5;

        public AlphaCrossover((double a, double b) x1Bounds, (double a, double b) x2Bounds, Random? random = null) :
            base(x1Bounds, x2Bounds, random)
        {
            
        }

        public override List<Chromosome> Crossover(IList<Chromosome> parents, int outPopulationSize) =>
            parents.GetRandomUniquePairsIndexes(Random).Take(outPopulationSize)
                .Select<(int parent1, int parent2), Chromosome>(parentsIndexes =>
                    CrossingProbability > Random.NextDouble()
                        ? CrossoverBody(parents[parentsIndexes.parent1], parents[parentsIndexes.parent2])
                        : parents[Random.Next() > 0.5 ? parentsIndexes.parent1 : parentsIndexes.parent2]).ToList();

        // public override List<Chromosome> Crossover(IList<Chromosome> parents, int outPopulationSize) =>
        //     parents.GetRandomUniquePairs(Random).Take(outPopulationSize)
        //         .Select<(Chromosome parent1, Chromosome parent2), Chromosome>(twoParents =>
        //             CrossingProbability > Random.NextDouble()
        //                 ? CrossoverBody(twoParents.parent1, twoParents.parent2)
        //                 : twoParents.parent1).ToList();


        //Todo add Bounds
        protected virtual Chromosome CrossoverBody(Chromosome parent1, Chromosome parent2)
        {
            var (x1Min, x1Max) = (Math.Min(parent1.X1, parent2.X1), Math.Max(parent1.X1, parent2.X1));
            var (x2Min, x2Max) = (Math.Min(parent1.X2, parent2.X2), Math.Max(parent1.X2, parent2.X2));
            var (i1, i2) = (x1Max - x1Min, x2Max - x2Min);

            return new Chromosome
            {
                X1 = Math.Clamp(ContinuousUniform.Sample(Random, x1Min - Alpha * i1, x1Max + Alpha * i1), X1Bounds.a, X1Bounds.b),
                X2 = Math.Clamp(ContinuousUniform.Sample(Random, x2Min - Alpha * i2, x2Max + Alpha * i2), X2Bounds.a, X2Bounds.b)
            };
        }
    }
}