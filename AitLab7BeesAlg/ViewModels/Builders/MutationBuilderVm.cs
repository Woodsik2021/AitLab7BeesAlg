using System;
using System.ComponentModel;
using AitLab7BeesAlg.Models.BeesAlg;

namespace AitLab7BeesAlg.ViewModels.Builders
{
    public abstract class MutationBuilderVm : INotifyPropertyChanged
    {
        protected Random Random { get; }

        public double MutationProbability { get; set; } = 0.1;


        protected MutationBuilderVm(Random? random = null)
        {
            Random = random ?? new Random();
        }

        public abstract MutationAlg Build(BoundsVm x1Bounds, BoundsVm x2Bounds);


        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class GaussianMutationBuilderVm : MutationBuilderVm
    {
        public double StdDevPercent { get; set; } = 0.1;

        public GaussianMutationBuilderVm(Random? random = null) : base(random)
        {
        }
        

        public override GaussianMutation Build(BoundsVm x1Bounds, BoundsVm x2Bounds) => new(x1Bounds.ToValTuple(), x2Bounds.ToValTuple(), Random)
        {
            MutationProbability = MutationProbability,
            StdDevPercent = StdDevPercent
        };
    }

    public class UniformMutationBuilderVm : MutationBuilderVm
    {
        public UniformMutationBuilderVm(Random? random = null) : base(random)
        {
        }

        public override UniformMutation Build(BoundsVm x1Bounds, BoundsVm x2Bounds) => new(x1Bounds.ToValTuple(), x2Bounds.ToValTuple(), Random)
        {
            MutationProbability = MutationProbability
        };
    }
}