using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class TooltipView : MonoBehaviour
{
    [SerializeField] private SO_TooltipDatabase _database;
    [SerializeField] private GameObject _root;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Vector2 _screenOffset = new(18f, -18f);
    [SerializeField] private float _screenPadding = 8f;

    private RectTransform _rootRectTransform;
    private RectTransform _rootParentRectTransform;
    private Canvas _canvas;
    private bool _isShowing;
    private Vector2 _lastPointerScreenPosition;
    private readonly Vector3[] _worldCorners = new Vector3[4];

    private void Awake()
    {
        CacheReferences();
        DisableRaycasts();
        Hide();
    }

    private void Update()
    {
        if (!_isShowing)
            return;

        MoveTo(GetPointerScreenPosition());
    }

    public void Show(TooltipKey key)
    {
        Show(key, GetPointerScreenPosition());
    }

    public void Show(TooltipKey key, Vector2 pointerScreenPosition)
    {
        if (_database == null || !_database.TryGetText(key, out var tooltipText))
        {
            Hide();
            return;
        }

        ShowText(tooltipText, pointerScreenPosition);
    }

    public void ShowText(string tooltipText)
    {
        ShowText(tooltipText, GetPointerScreenPosition());
    }

    public void ShowText(string tooltipText, Vector2 pointerScreenPosition)
    {
        if (string.IsNullOrWhiteSpace(tooltipText))
        {
            Hide();
            return;
        }

        if (_text != null)
            _text.text = tooltipText;

        if (_root != null)
            _root.SetActive(true);

        _isShowing = true;
        Canvas.ForceUpdateCanvases();
        MoveTo(pointerScreenPosition);
    }

    public void MoveTo(Vector2 pointerScreenPosition)
    {
        if (!_isShowing)
            return;

        _lastPointerScreenPosition = pointerScreenPosition;
        FollowPointer(_lastPointerScreenPosition);
    }

    public void Hide()
    {
        if (_root != null)
            _root.SetActive(false);

        _isShowing = false;
    }

    private void CacheReferences()
    {
        if (_root != null)
        {
            _rootRectTransform = _root.GetComponent<RectTransform>();
            _rootParentRectTransform = _rootRectTransform != null
                ? _rootRectTransform.parent as RectTransform
                : null;
        }

        _canvas = _root != null
            ? _root.GetComponentInParent<Canvas>()
            : GetComponentInParent<Canvas>();
    }

    private void FollowPointer(Vector2 pointerScreenPosition)
    {
        if (_rootRectTransform == null || _rootParentRectTransform == null || _canvas == null)
            return;

        var camera = GetEventCamera();
        var screenPosition = pointerScreenPosition + _screenOffset;

        if (!SetRootScreenPosition(screenPosition, camera))
            return;

        Canvas.ForceUpdateCanvases();
        var clampedScreenPosition = ClampRootInsideScreen(screenPosition, camera);

        if (clampedScreenPosition != screenPosition)
            SetRootScreenPosition(clampedScreenPosition, camera);
    }

    private void DisableRaycasts()
    {
        if (_root == null)
            return;

        foreach (var graphic in _root.GetComponentsInChildren<Graphic>(true))
            graphic.raycastTarget = false;

        if (!_root.TryGetComponent<CanvasGroup>(out var canvasGroup))
            canvasGroup = _root.AddComponent<CanvasGroup>();

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private Camera GetEventCamera()
    {
        if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return _canvas.worldCamera != null
            ? _canvas.worldCamera
            : Camera.main;
    }

    private bool SetRootScreenPosition(Vector2 screenPosition, Camera camera)
    {
        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rootParentRectTransform,
                screenPosition,
                camera,
                out var worldPosition))
        {
            return false;
        }

        _rootRectTransform.position = worldPosition;
        return true;
    }

    private Vector2 ClampRootInsideScreen(Vector2 targetScreenPosition, Camera camera)
    {
        _rootRectTransform.GetWorldCorners(_worldCorners);

        var min = new Vector2(float.MaxValue, float.MaxValue);
        var max = new Vector2(float.MinValue, float.MinValue);

        for (var i = 0; i < _worldCorners.Length; i++)
        {
            var corner = RectTransformUtility.WorldToScreenPoint(camera, _worldCorners[i]);
            min = Vector2.Min(min, corner);
            max = Vector2.Max(max, corner);
        }

        var correction = Vector2.zero;

        if (min.x < _screenPadding)
            correction.x = _screenPadding - min.x;
        else if (max.x > Screen.width - _screenPadding)
            correction.x = Screen.width - _screenPadding - max.x;

        if (min.y < _screenPadding)
            correction.y = _screenPadding - min.y;
        else if (max.y > Screen.height - _screenPadding)
            correction.y = Screen.height - _screenPadding - max.y;

        return targetScreenPosition + correction;
    }

    private Vector2 GetPointerScreenPosition()
    {
        return Pointer.current != null
            ? Pointer.current.position.ReadValue()
            : _lastPointerScreenPosition;
    }
}
