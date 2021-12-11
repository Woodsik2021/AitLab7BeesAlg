namespace AitLab7BeesAlg.Models.BeesAlg
{
    public class GeneticAlgSettings
    {
        public int PopulationSize { get; init; }
        public int CountGenerations { get; init; }
        
        public (double a, double b) X1Bounds { get; init; }
        public (double a, double b) X2Bounds { get; init; }
    }
}