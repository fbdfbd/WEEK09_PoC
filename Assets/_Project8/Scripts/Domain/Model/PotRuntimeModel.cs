using Project8.Domain.Data;

namespace Project8.Domain.Model
{
    public sealed class PotRuntimeModel
    {
        public FoodType FoodType { get; private set; }
        public TasteValue Taste { get; private set; }
        public float Volume { get; private set; }
        public float MaxVolume { get; private set; }

        public PotRuntimeModel(
            FoodType foodType,
            TasteValue taste,
            float volume,
            float maxVolume)
        {
            FoodType = foodType;
            Taste = taste;
            Volume = volume;
            MaxVolume = maxVolume;
        }

        public void SetFoodType(FoodType foodType)
        {
            FoodType = foodType;
        }

        public void SetTaste(TasteValue taste)
        {
            Taste = taste;
        }

        public void SetVolume(float volume)
        {
            Volume = volume;
        }
    }
}
