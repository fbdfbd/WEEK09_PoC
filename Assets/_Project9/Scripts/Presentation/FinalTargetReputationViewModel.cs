namespace Project9.Presentation
{
    public sealed class FinalTargetReputationViewModel
    {
        public FinalTargetReputationViewModel(
            string targetId,
            string targetName,
            int reputation)
        {
            TargetId = targetId;
            TargetName = targetName;
            Reputation = reputation;
        }

        public string TargetId { get; }
        public string TargetName { get; }
        public int Reputation { get; }
    }
}
