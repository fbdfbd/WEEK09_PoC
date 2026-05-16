namespace Project8.Presentation.Views
{
    public interface IHudView
    {
        void SetScore(int score);
        void SetTime(float seconds);
        void ShowGameEnded(int finalScore);
    }
}
