namespace FLaGLib.Data.StateMachines
{
    public class MinimizingIterationPostReport : MinimizingBeginPostReport
    {
        public bool IsLastIteration
        {
            get;
            private set;
        }

        public MinimizingIterationPostReport(SetsOfEquivalence setsOfEquivalence, int iteration, bool isLastIteration) 
            : base(setsOfEquivalence, iteration)
        {
            IsLastIteration = isLastIteration;
        }
    }
}
