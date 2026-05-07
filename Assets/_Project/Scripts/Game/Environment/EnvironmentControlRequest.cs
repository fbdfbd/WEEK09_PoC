namespace App.Gameplay.Environment
{
    public sealed class EnvironmentControlRequest
    {
        public EnvironmentControlRequest(string controlId)
        {
            ControlId = controlId;
        }

        public string ControlId { get; }
    }
}
