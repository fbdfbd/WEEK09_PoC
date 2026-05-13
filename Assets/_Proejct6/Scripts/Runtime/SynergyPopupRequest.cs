public class SynergyPopupRequest
{
    public string Title { get; private set; }
    public string Description { get; private set; }

    public SynergyPopupRequest(string title, string description)
    {
        Title = title;
        Description = description;
    }
}
