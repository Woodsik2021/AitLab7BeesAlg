using System.ComponentModel;

namespace AitLab7BeesAlg.ViewModels.Builders
{
    public class BoundsVm : INotifyPropertyChanged
    {
        public double A { get; set; } = -10;
        public double B { get; set; } = 10;

        public (double a, double b) ToValTuple() => (A, B);

        public void Deconstruct(out double a, out double b) => (a, b) = (A, B);

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}