using UnityEngine;
using UnityEngine.EventSystems;

public sealed class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] private TooltipKey _key;
    [SerializeField][TextArea] private string _overrideText;

    private TooltipView _view;

    public void SetKey(TooltipKey key)
    {
        _key = key;
    }

    public void SetOverrideText(string text)
    {
        _overrideText = text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ResolveView();

        if (_view == null)
            return;

        if (!string.IsNullOrWhiteSpace(_overrideText))
        {
            _view.ShowText(_overrideText, eventData.position);
            return;
        }

        _view.Show(_key, eventData.position);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        ResolveView();
        _view?.MoveTo(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResolveView();
        _view?.Hide();
    }

    private void ResolveView()
    {
        if (_view != null)
            return;

#if UNITY_2023_1_OR_NEWER
        _view = Object.FindFirstObjectByType<TooltipView>(FindObjectsInactive.Include);
#else
        _view = Object.FindObjectOfType<TooltipView>(true);
#endif
    }
}
