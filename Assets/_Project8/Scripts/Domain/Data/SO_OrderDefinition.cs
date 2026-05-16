using UnityEngine;

namespace Project8.Domain.Data
{
    [CreateAssetMenu(
        fileName = "SO_OrderDefinition",
        menuName = "Project8/Data/Order Definition")]
    public sealed class SO_OrderDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private FoodType _foodType;
        [SerializeField] private TasteRange _spicyRange;
        [SerializeField] private TasteRange _sweetRange;
        [SerializeField] private TasteRange _thickRange;
        [SerializeField] private float _patienceSeconds = 40f;
        [SerializeField] private int _baseScore = 100;

        public string Id { get { return _id; } }
        public string DisplayName { get { return _displayName; } }
        public FoodType FoodType { get { return _foodType; } }
        public TasteRange SpicyRange { get { return _spicyRange; } }
        public TasteRange SweetRange { get { return _sweetRange; } }
        public TasteRange ThickRange { get { return _thickRange; } }
        public float PatienceSeconds { get { return _patienceSeconds; } }
        public int BaseScore { get { return _baseScore; } }
    }
}
