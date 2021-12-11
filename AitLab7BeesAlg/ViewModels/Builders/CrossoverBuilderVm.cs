using System;
using System.ComponentModel;
using AitLab7BeesAlg.Models.BeesAlg;

namespace AitLab7BeesAlg.ViewModels.Builders
{
    public abstract class CrossoverBuilderVm : INotifyPropertyChanged
    {
        protected Random Random { get; }

        public double CrossingProbability { get; set; } = 0.98;


        protected CrossoverBuilderVm(Random? random = null)
        {
            Random = random ?? new Random();
        }


        public abstract CrossoverAlg Build(BoundsVm x1Bounds, BoundsVm x2Bounds);

        
        public event PropertyChangedEventHandler? PropertyChanged;
    }

    
    public class ExtendedLineCrossoverBuilderVm : CrossoverBuilderVm
    {
        public ExtendedLineCrossoverBuilderVm(Random? random = null) : base(random)
        {
        }

        public override ExtendedLineCrossover Build(BoundsVm x1Bounds, BoundsVm x2Bounds) => new(x1Bounds.ToValTuple(), x2Bounds.ToValTuple(), Random)
        {
            CrossingProbability = CrossingProbability
        };
    }


    public class AlphaCrossoverBuilderVm : CrossoverBuilderVm
    {
        public double Alpha { get; set; } = 0.5;

        public AlphaCrossoverBuilderVm(Random? random = null) : base(random)
        {
        }

        public override AlphaCrossover Build(BoundsVm x1Bounds, BoundsVm x2Bounds) => new(x1Bounds.ToValTuple(), x2Bounds.ToValTuple(), Random)
        {
            CrossingProbability = CrossingProbability,
            Alpha = Alpha
        };
    }
}