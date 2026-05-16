using Project9.Data;

namespace Project9.Presentation
{
    public sealed class ParagraphViewModel
    {
        public ParagraphViewModel(
            string id,
            string title,
            string text,
            int informationValue,
            int sensitivity,
            int integrity,
            int exposure,
            ParagraphActionType actionType,
            string selectedEditOptionId)
        {
            Id = id;
            Title = title;
            Text = text;
            InformationValue = informationValue;
            Sensitivity = sensitivity;
            Integrity = integrity;
            Exposure = exposure;
            ActionType = actionType;
            SelectedEditOptionId = selectedEditOptionId;
        }

        public string Id { get; }
        public string Title { get; }
        public string Text { get; }
        public int InformationValue { get; }
        public int Sensitivity { get; }
        public int Integrity { get; }
        public int Exposure { get; }
        public ParagraphActionType ActionType { get; }
        public string SelectedEditOptionId { get; }
    }
}
