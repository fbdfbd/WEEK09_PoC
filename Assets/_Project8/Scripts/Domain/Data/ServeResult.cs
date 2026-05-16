namespace Project8.Domain.Data
{
    public readonly struct ServeResult
    {
        public readonly string OrderInstanceId;
        public readonly float TasteScore;
        public readonly int Star;
        public readonly int FinalScore;
        public readonly bool IsSuccess;

        public ServeResult(
            string orderInstanceId,
            float tasteScore,
            int star,
            int finalScore,
            bool isSuccess)
        {
            OrderInstanceId = orderInstanceId;
            TasteScore = tasteScore;
            Star = star;
            FinalScore = finalScore;
            IsSuccess = isSuccess;
        }
    }
}
