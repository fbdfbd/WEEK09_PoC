using TMPro;
using UnityEngine;

namespace Project8.Presentation.Views
{
    public sealed class HudView : MonoBehaviour, IHudView
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _gameStateText;

        public void SetScore(int score)
        {
            SetText(_scoreText, "점수 " + score);
        }

        public void SetTime(float seconds)
        {
            SetText(_timeText, "남은 시간 " + Mathf.CeilToInt(seconds));
        }

        public void ShowGameEnded(int finalScore)
        {
            SetText(_gameStateText, "게임 종료 / 최종 점수 " + finalScore);
        }

        private static void SetText(TMP_Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
