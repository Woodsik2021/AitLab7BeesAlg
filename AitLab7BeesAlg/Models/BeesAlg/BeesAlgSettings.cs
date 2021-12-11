namespace AitLab7BeesAlg.Models.BeesAlg
{   
    public class BeesAlgSettings    
    {
        public int IterationsCount { get; init; }
        public double TempInit { get; init; }
        public double Alpha { get; init; }
        public int InitScoutsCount { get; init; }
        
        public (double a, double b) X1Bounds { get; init; }
        public (double a, double b) X2Bounds { get; init; }
    }
}