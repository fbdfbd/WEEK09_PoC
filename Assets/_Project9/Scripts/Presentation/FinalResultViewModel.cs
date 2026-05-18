using System.Collections.Generic;

namespace Project9.Presentation
{
    public sealed class FinalResultViewModel
    {
        public FinalResultViewModel(
            int finalReputation,
            int maxReputation,
            IReadOnlyList<FinalTargetReputationViewModel> targetReputations)
        {
            FinalReputation = finalReputation;
            MaxReputation = maxReputation;
            TargetReputations = targetReputations;
        }

        public int FinalReputation { get; }
        public int MaxReputation { get; }
        public IReadOnlyList<FinalTargetReputationViewModel> TargetReputations { get; }
    }
}
