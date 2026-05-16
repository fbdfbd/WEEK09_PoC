using System;

namespace Project8.Domain.Data
{
    [Serializable]
    public struct TasteValue
    {
        public float Spicy;
        public float Sweet;
        public float Thick;

        public TasteValue(float spicy, float sweet, float thick)
        {
            Spicy = spicy;
            Sweet = sweet;
            Thick = thick;
        }

        public static TasteValue Zero
        {
            get { return new TasteValue(0f, 0f, 0f); }
        }
    }
}
