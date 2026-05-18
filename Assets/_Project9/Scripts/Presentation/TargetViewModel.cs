namespace Project9.Presentation
{
    public sealed class TargetViewModel
    {
        public TargetViewModel(
            string id,
            string displayName,
            string description,
            int reputation,
            bool isSelected,
            bool isSubmitted)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            Reputation = reputation;
            IsSelected = isSelected;
            IsSubmitted = isSubmitted;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public int Reputation { get; }
        public bool IsSelected { get; }
        public bool IsSubmitted { get; }
    }
}
