namespace Project8.Application.Events
{
    public readonly struct IngredientAppliedEvent : IGameEvent
    {
        public readonly string IngredientId;
        public readonly string DisplayName;

        public IngredientAppliedEvent(string ingredientId, string displayName)
        {
            IngredientId = ingredientId;
            DisplayName = displayName;
        }
    }
}
