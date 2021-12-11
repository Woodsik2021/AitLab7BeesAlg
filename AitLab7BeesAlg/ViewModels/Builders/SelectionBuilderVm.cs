using System;
using System.ComponentModel;
using AitLab7BeesAlg.Models.BeesAlg;

namespace AitLab7BeesAlg.ViewModels.Builders
{
    public abstract class SelectionBuilderVm : INotifyPropertyChanged
    {
        protected Random Random { get; }

        protected SelectionBuilderVm(Random? random = null)
        {
            Random = random ?? new Random();
        }

        public abstract SelectionAlg Build();
        

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class TournamentSelectionBuilderVm : SelectionBuilderVm
    {
        public int TournamentSize { get; set; } = 3;

        public TournamentSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override TournamentSelection Build() => new(Random) {TournamentSize = TournamentSize};
    }
    
    public class RouletteSelectionBuilderVm : SelectionBuilderVm
    {
        public RouletteSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override RouletteSelection Build() => new(Random);
    }
    
    public class SusSelectionBuilderVm : SelectionBuilderVm
    {
        public SusSelectionBuilderVm(Random? random = null) : base(random)
        {
        }

        public override SusSelection Build() => new(Random);
    }
}