using Project8.Domain.Data;

namespace Project8.Domain.Model
{
    public sealed class OrderRuntimeModel
    {
        public string InstanceId { get; private set; }
        public string DefinitionId { get; private set; }
        public string DisplayName { get; private set; }
        public FoodType FoodType { get; private set; }
        public TasteRange SpicyRange { get; private set; }
        public TasteRange SweetRange { get; private set; }
        public TasteRange ThickRange { get; private set; }
        public float PatienceSeconds { get; private set; }
        public float RemainingPatienceSeconds { get; private set; }
        public int BaseScore { get; private set; }

        public OrderRuntimeModel(
            string instanceId,
            string definitionId,
            string displayName,
            FoodType foodType,
            TasteRange spicyRange,
            TasteRange sweetRange,
            TasteRange thickRange,
            float patienceSeconds,
            int baseScore)
        {
            InstanceId = instanceId;
            DefinitionId = definitionId;
            DisplayName = displayName;
            FoodType = foodType;
            SpicyRange = spicyRange;
            SweetRange = sweetRange;
            ThickRange = thickRange;
            PatienceSeconds = patienceSeconds;
            RemainingPatienceSeconds = patienceSeconds;
            BaseScore = baseScore;
        }

        public void SetRemainingPatienceSeconds(float seconds)
        {
            RemainingPatienceSeconds = seconds;
        }
    }
}
