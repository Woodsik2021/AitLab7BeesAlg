using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AitLab7BeesAlg.Models;
using AitLab7BeesAlg.Models.BeesAlg;
using Newtonsoft.Json;

namespace AitLab7BeesAlg.ViewModels.Builders
{
    public class GeneticAlgorithmBuilderVm : INotifyPropertyChanged
    {
        private readonly Random _random;
        
        [JsonIgnore]
        public MessageViewModel? MessageViewModel { get; init; } = new();
        
        public BoundsVm X1Bounds { get; set; } = new();
        public BoundsVm X2Bounds { get; set; } = new();

        public int PopulationSize { get; set; } = 20;
        public int CountGenerations { get; set; } = 100;

        
        [JsonIgnore]
        public List<SelectionBuilderVm> Selections { get; }
        
        [JsonProperty("Selection Strategy")]
        public SelectionBuilderVm SelectedSelection { get; set; }

        [JsonIgnore]
        public List<CrossoverBuilderVm> Crossovers { get; }
        
        [JsonProperty("Crossover Strategy")]
        public CrossoverBuilderVm SelectedCrossover { get; set; }

        [JsonIgnore]
        public List<MutationBuilderVm> Mutations { get; }
        
        [JsonProperty("Mutation Strategy")]
        public MutationBuilderVm SelectedMutation { get; set; }

        [JsonProperty("Objective Function")]
        public string SelectedFunction { get; set; }

        [JsonIgnore]
        public List<string> ObjectiveFunctions { get; }


        public GeneticAlgorithmBuilderVm(Random? random = null)
        {
            _random = random ?? new Random();

            Selections = new List<SelectionBuilderVm>
            {
                new TournamentSelectionBuilderVm(_random),
                new RouletteSelectionBuilderVm(_random),
                new SusSelectionBuilderVm(_random)
            };
            SelectedSelection = Selections.First();

            Crossovers = new List<CrossoverBuilderVm>
            {
                new ExtendedLineCrossoverBuilderVm(_random),
                new AlphaCrossoverBuilderVm(_random)
            };
            SelectedCrossover = Crossovers.First();

            Mutations = new List<MutationBuilderVm>
            {
                new UniformMutationBuilderVm(_random),
                new GaussianMutationBuilderVm(_random)
            };
            SelectedMutation = Mutations.First();

            ObjectiveFunctions = new List<string>()
            {
                "x1^2 + 3*x2^2 + 2*x1*x2",
                "100*(x2 - x1^2)^2 + (1 - x1)^2",
                "-12*x2 + 4*x1^2 + 4*x2^2 - 4*x1*x2",
                "(x1 - 2)^4 + (x1 - 2*x2)^2",
                "4*(x1 - 5)^2 + (x2 - 6)^2"
            };
            SelectedFunction = ObjectiveFunctions.FirstOrDefault(s => s.Equals("4*(x1 - 5)^2 + (x2 - 6)^2")) ??
                               String.Empty;
        }

        public GeneticAlgorithm Build() => new(
            FunctionParser.ParseFunction(SelectedFunction),
            new GeneticAlgSettings
            {
                CountGenerations = CountGenerations, PopulationSize = PopulationSize,
                X1Bounds = X1Bounds.ToValTuple(), X2Bounds = X2Bounds.ToValTuple(),
            },
            SelectedSelection.Build(), SelectedCrossover.Build(X1Bounds, X2Bounds),
            SelectedMutation.Build(X1Bounds, X2Bounds),
            _random);
        
        
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}