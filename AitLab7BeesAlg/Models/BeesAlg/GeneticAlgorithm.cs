using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace AitLab7BeesAlg.Models.BeesAlg
{
    public class GeneticAlgorithm
    {
        private const double Eps = 0.00001;

        private readonly GeneticAlgSettings _algSettings;
        private List<Chromosome> _population = new();
        private readonly Random _randomSource;
        private readonly ISelectionAlg _selectionStrategy;
        private readonly ICrossoverAlg _crossoverStrategy;
        private readonly IMutationAlg _mutationStrategy;

        public Func<double, double, double> ObjectiveFunction { get; }

        public GeneticAlgorithm(Func<double, double, double> objectiveFunction, GeneticAlgSettings algSettings,
            ISelectionAlg selectionStrategy, ICrossoverAlg crossoverStrategy, IMutationAlg mutationStrategy,
            Random? randomSource = null)
        {
            _algSettings = algSettings;
            ObjectiveFunction = objectiveFunction;
            _selectionStrategy = selectionStrategy;
            _crossoverStrategy = crossoverStrategy;
            _mutationStrategy = mutationStrategy;
            _randomSource = randomSource ?? new Random();
        }

        public GeneticAlgorithm(Func<double, double, double> objectiveFunction, GeneticAlgSettings algSettings,
            Random? randomSource = null) :
            this(objectiveFunction, algSettings,
                selectionStrategy: new TournamentSelection(randomSource),
                crossoverStrategy: new AlphaCrossover(
                    (algSettings.X1Bounds.a, algSettings.X1Bounds.b),
                    (algSettings.X2Bounds.a, algSettings.X2Bounds.b),
                    randomSource),
                mutationStrategy: new UniformMutation(
                    (algSettings.X1Bounds.a, algSettings.X1Bounds.b),
                    (algSettings.X2Bounds.a, algSettings.X2Bounds.b),
                    randomSource
                ),
                randomSource)
        {
        }


        private List<Chromosome> GenerateInitPopulation() => Enumerable.Range(0, _algSettings.PopulationSize)
            .Select(_ => new Chromosome
            {
                X1 = ContinuousUniform.Sample(_randomSource, _algSettings.X1Bounds.a, _algSettings.X1Bounds.b),
                X2 = ContinuousUniform.Sample(_randomSource, _algSettings.X2Bounds.a, _algSettings.X2Bounds.b),
            }).ToList();

        private void AlgorithmBody()
        {
            var parents = _selectionStrategy.Selection(_population, _algSettings.PopulationSize);
            _population = _crossoverStrategy.Crossover(parents, _algSettings.PopulationSize);
            _mutationStrategy.Mutate(_population);
            _population.EvalFunction(ObjectiveFunction);
        }


        public Chromosome FindMinFunction()
        {
            _population = GenerateInitPopulation();
            _population.EvalFunction(ObjectiveFunction);

            for (var i = 0; i < _algSettings.CountGenerations - 1; i++)
            {
                AlgorithmBody();
            }

            return _population.OrderBy(x => x.FuncValue).First();
        }

        public IEnumerable<List<Chromosome>> FindMinFunctionAndSave()
        {
            _population = GenerateInitPopulation();
            _population.EvalFunction(ObjectiveFunction);
            yield return new List<Chromosome>(_population);

            for (var i = 0; i < _algSettings.CountGenerations - 1; i++)
            {
                AlgorithmBody();
                yield return new List<Chromosome>(_population);
            }
        }
    }
}