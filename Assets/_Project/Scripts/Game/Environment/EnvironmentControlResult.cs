namespace App.Gameplay.Environment
{
    public sealed class EnvironmentControlResult
    {
        private EnvironmentControlResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; }
        public string Message { get; }

        public static EnvironmentControlResult Success()
        {
            return new EnvironmentControlResult(true, string.Empty);
        }

        public static EnvironmentControlResult Failed(string message)
        {
            return new EnvironmentControlResult(false, message);
        }
    }
}
