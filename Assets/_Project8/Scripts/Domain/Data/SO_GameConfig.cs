using UnityEngine;

namespace Project8.Domain.Data
{
    [CreateAssetMenu(
        fileName = "SO_GameConfig",
        menuName = "Project8/Config/Game Config")]
    public sealed class SO_GameConfig : ScriptableObject
    {
        [Header("Pot")]
        [SerializeField] private TasteValue _initialTaste = new TasteValue(40f, 40f, 40f);
        [SerializeField] private float _initialVolume = 60f;
        [SerializeField] private float _maxVolume = 100f;
        [SerializeField] private FoodType _initialFoodType = FoodType.Tteokbokki;

        [Header("Simulation")]
        [SerializeField] private float _thickIncreasePerSecond = 2f;
        [SerializeField] private float _volumeDecreasePerSecond = 0.5f;
        [SerializeField] private float _friedRiceThreshold = 85f;
        [SerializeField] private float _burnPenaltyThreshold = 95f;

        [Header("Orders")]
        [SerializeField] private int _maxActiveOrders = 3;
        [SerializeField] private float _orderSpawnIntervalMin = 8f;
        [SerializeField] private float _orderSpawnIntervalMax = 12f;

        [Header("Game")]
        [SerializeField] private float _gameSeconds = 120f;
        [SerializeField] private float _servingVolumeCost = 20f;
        [SerializeField] private float _minimumServingVolume = 20f;

        public TasteValue InitialTaste { get { return _initialTaste; } }
        public float InitialVolume { get { return _initialVolume; } }
        public float MaxVolume { get { return _maxVolume; } }
        public FoodType InitialFoodType { get { return _initialFoodType; } }

        public float ThickIncreasePerSecond { get { return _thickIncreasePerSecond; } }
        public float VolumeDecreasePerSecond { get { return _volumeDecreasePerSecond; } }
        public float FriedRiceThreshold { get { return _friedRiceThreshold; } }
        public float BurnPenaltyThreshold { get { return _burnPenaltyThreshold; } }

        public int MaxActiveOrders { get { return _maxActiveOrders; } }
        public float OrderSpawnIntervalMin { get { return _orderSpawnIntervalMin; } }
        public float OrderSpawnIntervalMax { get { return _orderSpawnIntervalMax; } }

        public float GameSeconds { get { return _gameSeconds; } }
        public float ServingVolumeCost { get { return _servingVolumeCost; } }
        public float MinimumServingVolume { get { return _minimumServingVolume; } }
    }
}
