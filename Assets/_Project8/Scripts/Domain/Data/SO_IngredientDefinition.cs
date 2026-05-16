using UnityEngine;

namespace Project8.Domain.Data
{
    [CreateAssetMenu(
        fileName = "SO_IngredientDefinition",
        menuName = "Project8/Data/Ingredient Definition")]
    public sealed class SO_IngredientDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private TasteValue _tasteDelta;
        [SerializeField] private float _volumeDelta;
        [SerializeField] private bool _isRice;

        public string Id { get { return _id; } }
        public string DisplayName { get { return _displayName; } }
        public TasteValue TasteDelta { get { return _tasteDelta; } }
        public float VolumeDelta { get { return _volumeDelta; } }
        public bool IsRice { get { return _isRice; } }
    }
}
