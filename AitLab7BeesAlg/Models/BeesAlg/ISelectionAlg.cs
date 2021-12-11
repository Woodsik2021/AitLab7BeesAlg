using System;
using System.Collections.Generic;
using System.Linq;
using AitLab7BeesAlg.Models.Extension;
using MathNet.Numerics.Distributions;

namespace AitLab7BeesAlg.Models.BeesAlg
{
    public interface ISelectionAlg
    {
        public List<Chromosome> Selection(IList<Chromosome> currentPopulation, int outPopulationSize);
    }

    public abstract class SelectionAlg : ISelectionAlg
    {
        protected Random Random { get; }

        protected SelectionAlg(Random? random = null)
        {
            Random = random ?? new Random();
        }

        public abstract List<Chromosome> Selection(IList<Chromosome> currentPopulation, int outPopulationSize);
    }

    public class RouletteSelection : SelectionAlg
    {
        public RouletteSelection(Random? random = null) : base(random)
        {
        }

        public override List<Chromosome> Selection(IList<Chromosome> currentPopulation, int outPopulationSize)
        {
            var sortedDescByFuncPopulation = currentPopulation.OrderByDescending(chr => chr.FuncValue).ToList();

            List<(Chromosome chr, int accWeight)> chrWithAccWList = new(sortedDescByFuncPopulation.Count);
            var (weight, accWeight, prevChr) = (1, 0, sortedDescByFuncPopulation.First());
            foreach (var chromosome in sortedDescByFuncPopulation)
            {
                if (Math.Abs(prevChr.FuncValue - chromosome.FuncValue) > 1e-15)
                    weight++;
                accWeight += weight;
                chrWithAccWList.Add((chromosome, accWeight));
            }

            var maxAccWeight = chrWithAccWList.Last().accWeight;

            return DiscreteUniform.Samples(Random, 0, maxAccWeight - 1)
                .Take(outPopulationSize)
                .Select(randomAccWeight =>
                    chrWithAccWList.First(chrWithAccW => chrWithAccW.accWeight > randomAccWeight).chr)
                .ToList();
        }
    }


    public class SusSelection : SelectionAlg
    {
        public SusSelection(Random? random = null) : base(random)
        {
        }

        public override List<Chromosome> Selection(IList<Chromosome> currentPopulation, int outPopulationSize)
        {
            var sortedDescByFuncPopulation = currentPopulation.OrderByDescending(chr => chr.FuncValue).ToList();

            List<(Chromosome chr, int accWeight)> chrWithAccWList = new(sortedDescByFuncPopulation.Count);
            var (weight, accWeight, prevChr) = (1, 0, sortedDescByFuncPopulation.First());
            foreach (var chromosome in sortedDescByFuncPopulation)
            {
                if (Math.Abs(prevChr.FuncValue - chromosome.FuncValue) > 1e-15)
                    weight++;
                accWeight += weight;
                chrWithAccWList.Add((chromosome, accWeight));
            }

            var maxAccWeight = chrWithAccWList.Last().accWeight;


            chrWithAccWList = chrWithAccWList.GetRandomElements().ToList(); // ShuffleList
            var distBetweenPoints = maxAccWeight / (double)outPopulationSize;
            var seedPoint = ContinuousUniform.Sample(Random, 0, distBetweenPoints);
            var randomPoints = Enumerable.Range(0, outPopulationSize).Select(i => seedPoint + i * distBetweenPoints)
                .ToList();
            List<Chromosome> selectedChromosomes = new();

            var (index, curChrWithAccW) = (0, chrWithAccWList.First());
            foreach (var point in randomPoints)
            {
                while (point > curChrWithAccW.accWeight)
                {
                    index++;
                    if (index < chrWithAccWList.Count) curChrWithAccW = chrWithAccWList[index];
                }
                selectedChromosomes.Add(curChrWithAccW.chr);
            }

            return selectedChromosomes;
        }
    }


    public class TournamentSelection : SelectionAlg
    {
        public int TournamentSize { get; init; } = 3;

        public TournamentSelection(Random? random = null) : base(random)
        {
        }

        public override List<Chromosome> Selection(IList<Chromosome> currentPopulation, int outPopulationSize) =>
            Enumerable.Range(0, outPopulationSize).Select(_ =>
                    currentPopulation.GetRandomElements(Random)
                        .Take(Math.Min(TournamentSize, currentPopulation.Count))
                        .MinBy(x => x.FuncValue))
                .ToList();
    }
}