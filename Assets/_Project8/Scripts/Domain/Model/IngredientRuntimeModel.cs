using Project8.Domain.Data;

namespace Project8.Domain.Model
{
    public sealed class IngredientRuntimeModel
    {
        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public TasteValue TasteDelta { get; private set; }
        public float VolumeDelta { get; private set; }
        public bool IsRice { get; private set; }

        public IngredientRuntimeModel(
            string id,
            string displayName,
            TasteValue tasteDelta,
            float volumeDelta,
            bool isRice)
        {
            Id = id;
            DisplayName = displayName;
            TasteDelta = tasteDelta;
            VolumeDelta = volumeDelta;
            IsRice = isRice;
        }
    }
}
