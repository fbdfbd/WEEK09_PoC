namespace Project8.Domain.Data
{
    public readonly struct IngredientApplyResult
    {
        public readonly bool IsSuccess;
        public readonly string IngredientId;
        public readonly string Message;

        public IngredientApplyResult(bool isSuccess, string ingredientId, string message)
        {
            IsSuccess = isSuccess;
            IngredientId = ingredientId;
            Message = message;
        }

        public static IngredientApplyResult Success(string ingredientId)
        {
            return new IngredientApplyResult(true, ingredientId, string.Empty);
        }

        public static IngredientApplyResult Fail(string ingredientId, string message)
        {
            return new IngredientApplyResult(false, ingredientId, message);
        }
    }
}
