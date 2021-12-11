using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AitLab7BeesAlg.Models.Extension;
using MathNet.Numerics.Distributions;

namespace AitLab7BeesAlg.Models.BeesAlg
{
    public class BeesAlgorithm
    {
        private const double Eps = 0.00001;

        private readonly Random _random;

        private readonly BeesAlgSettings _algSettings;
        private List<Chromosome> _population = new();

        private double _curTemp;

        private int _scoutsCount;
        private int _maxScoutsCount;
        private List<Chromosome> _scouts = new();
        private List<Chromosome> _workers = new();
        private Chromosome _best = new();

        private readonly ISelectionAlg _selectionStrategy;
        private readonly ICrossoverAlg _crossoverStrategy;
        private readonly IMutationAlg _mutationStrategy;


        public Func<double, double, double> ObjectiveFunction { get; }

        public BeesAlgorithm(Func<double, double, double> objectiveFunction, BeesAlgSettings algSettings,
            Random? randomSource = null)
        {
            ObjectiveFunction = objectiveFunction;
            _algSettings = algSettings;
            _scoutsCount = algSettings.InitScoutsCount;
            _maxScoutsCount = _scoutsCount;
            _curTemp = algSettings.TempInit;
            _random = randomSource ?? new Random();

            _mutationStrategy =
                new NeighborhoodMutation(algSettings.X1Bounds, algSettings.X2Bounds, objectiveFunction, _random);
        }


        private List<Chromosome> GenerateScouts() => Enumerable.Range(0, _scoutsCount)
            .Select(_ => new Chromosome
            {
                X1 = ContinuousUniform.Sample(_random, _algSettings.X1Bounds.a, _algSettings.X1Bounds.b),
                X2 = ContinuousUniform.Sample(_random, _algSettings.X2Bounds.a, _algSettings.X2Bounds.b),
            }).ToList();

        private void AlgorithmBody(int i)
        {
            _scouts = GenerateScouts();
            _scouts.EvalFunction(ObjectiveFunction);

            _best = _scouts.Concat(_workers).MinBy(chr => chr.FuncValue);

            _workers = _workers.Concat(_scouts.Where(chr => AcceptanceProb(chr, _best))).ToList();

            _mutationStrategy.Mutate(_workers);

            _best = _workers.MinBy(chr => chr.FuncValue);

            _workers = _workers.Where(chr => AcceptanceProb(chr, _best)).ToList();

            _scoutsCount = Math.Max(0, _maxScoutsCount - _workers.Count);
            //Debug.WriteLine($"Scouts count: {_scoutsCount}");
            _curTemp = _algSettings.TempInit * (1.0 - (i + 1.0) / _algSettings.IterationsCount); //ToDo
        }

        protected bool AcceptanceProb(Chromosome chr, Chromosome refChr) =>
            chr.FuncValue <= refChr.FuncValue + 1e-15 ||
            Math.Exp(-Math.Abs(chr.FuncValue - refChr.FuncValue) / _curTemp) > _random.NextDouble();


        public Chromosome FindMinFunction()
        {
            for (var i = 0; i < _algSettings.IterationsCount; i++)
            {
                AlgorithmBody(i);
            }

            return _best;
        }

        public IEnumerable<List<Chromosome>> FindMinFunctionAndSave()
        {
            for (var i = 0; i < _algSettings.IterationsCount; i++)
            {
                AlgorithmBody(i);
                yield return new List<Chromosome>(_population);
            }
        }
    }
}


// var newWorkers2 = _workers.Select(chr => new Chromosome
// {
//     X1 = _best.X1 + ContinuousUniform.Sample(_random, -1, 1) * (chr.X1 - _best.X1),
//     X2 = _best.X2 + ContinuousUniform.Sample(_random, -1, 1) * (chr.X2 - _best.X2)
// }).ToList();
// newWorkers2.EvalFunction(ObjectiveFunction);

// var sumFunc = newWorkers.Sum(chr => chr.FuncValue);
// var normalizedWorkers = newWorkers.Select(chr =>
//     new { Chromosome = chr, NormalizedFunc = chr.FuncValue / sumFunc });
// _workers = normalizedWorkers.SelectMany(chrWithNormValue =>
//     Enumerable.Repeat(chrWithNormValue.Chromosome, chrWithNormValue.NormalizedFunc switch
//     {
//         _ => _random.NextDouble() > 0.3 ? 0 : 1 // ToDo
//     })).ToList();