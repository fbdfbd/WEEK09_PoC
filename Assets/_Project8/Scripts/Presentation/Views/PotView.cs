using Project8.Domain.Model;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project8.Presentation.Views
{
    public sealed class PotView : MonoBehaviour, IPotView
    {
        [SerializeField] private TMP_Text _foodTypeText;
        [SerializeField] private Slider _spicySlider;
        [SerializeField] private Slider _sweetSlider;
        [SerializeField] private Slider _thickSlider;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private TMP_Text _spicyText;
        [SerializeField] private TMP_Text _sweetText;
        [SerializeField] private TMP_Text _thickText;
        [SerializeField] private TMP_Text _volumeText;
        [SerializeField] private Image _potBorderImage;
        [SerializeField] private Image _potContentImage;
        [SerializeField] private PotVisualColorSettings _visualColorSettings = new PotVisualColorSettings();

        private PotVisualColorResolver _visualColorResolver;

        private void Awake()
        {
            _visualColorResolver = new PotVisualColorResolver(_visualColorSettings);
            ApplyBorderColor();
        }

        public void SetPot(PotRuntimeModel pot)
        {
            if (pot == null)
            {
                return;
            }

            SetSlider(_spicySlider, pot.Taste.Spicy);
            SetSlider(_sweetSlider, pot.Taste.Sweet);
            SetSlider(_thickSlider, pot.Taste.Thick);
            SetSlider(_volumeSlider, pot.Volume);

            SetText(_foodTypeText, GetFoodTypeName(pot.FoodType));
            SetText(_spicyText, "매운맛 " + FormatValue(pot.Taste.Spicy));
            SetText(_sweetText, "단맛 " + FormatValue(pot.Taste.Sweet));
            SetText(_thickText, "농도 " + FormatValue(pot.Taste.Thick));
            SetText(_volumeText, "양 " + FormatValue(pot.Volume));

            ApplyContentColor(pot);
        }

        private static void SetSlider(Slider slider, float value)
        {
            if (slider != null)
            {
                slider.value = value;
            }
        }

        private static void SetText(TMP_Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        private static string FormatValue(float value)
        {
            return Mathf.RoundToInt(value).ToString();
        }

        private static string GetFoodTypeName(Domain.Data.FoodType foodType)
        {
            return foodType == Domain.Data.FoodType.FriedRice ? "볶음밥" : "떡볶이";
        }

        private void ApplyBorderColor()
        {
            if (_potBorderImage != null)
            {
                _potBorderImage.color = _visualColorResolver.GetBorderColor();
            }
        }

        private void ApplyContentColor(PotRuntimeModel pot)
        {
            if (_potContentImage != null)
            {
                _potContentImage.color = _visualColorResolver.GetContentColor(pot);
            }
        }
    }

    [Serializable]
    public sealed class PotVisualColorSettings
    {
        [SerializeField] private Color _tteokbokkiThinColor = new Color(0.95f, 0.18f, 0.12f, 1f);
        [SerializeField] private Color _tteokbokkiThickColor = new Color(0.48f, 0.08f, 0.04f, 1f);
        [SerializeField] private Color _friedRiceColor = new Color(0.86f, 0.54f, 0.18f, 1f);
        [SerializeField] private Color _potBorderColor = new Color(0.08f, 0.08f, 0.08f, 1f);

        public Color TteokbokkiThinColor { get { return _tteokbokkiThinColor; } }
        public Color TteokbokkiThickColor { get { return _tteokbokkiThickColor; } }
        public Color FriedRiceColor { get { return _friedRiceColor; } }
        public Color PotBorderColor { get { return _potBorderColor; } }
    }

    public sealed class PotVisualColorResolver
    {
        private readonly PotVisualColorSettings _settings;

        public PotVisualColorResolver(PotVisualColorSettings settings)
        {
            _settings = settings;
        }

        public Color GetContentColor(PotRuntimeModel pot)
        {
            if (pot.FoodType == Domain.Data.FoodType.FriedRice)
            {
                return _settings.FriedRiceColor;
            }

            var thickRatio = Mathf.InverseLerp(0f, 100f, pot.Taste.Thick);
            return Color.Lerp(
                _settings.TteokbokkiThinColor,
                _settings.TteokbokkiThickColor,
                thickRatio);
        }

        public Color GetBorderColor()
        {
            return _settings.PotBorderColor;
        }
    }
}
