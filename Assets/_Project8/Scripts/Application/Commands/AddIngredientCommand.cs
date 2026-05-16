namespace Project8.Application.Commands
{
    public readonly struct AddIngredientCommand : IGameCommand
    {
        public readonly string IngredientId;

        public AddIngredientCommand(string ingredientId)
        {
            IngredientId = ingredientId;
        }
    }
}
