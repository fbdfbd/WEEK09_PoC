namespace App.Gameplay.Phases
{
    public sealed class PhaseResult
    {
        private PhaseResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; }
        public string Message { get; }

        public static PhaseResult Success()
        {
            return new PhaseResult(true, string.Empty);
        }

        public static PhaseResult Failed(string message)
        {
            return new PhaseResult(false, message);
        }
    }
}
