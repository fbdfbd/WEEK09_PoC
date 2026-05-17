using System;
using System.Collections.Generic;
using DG.Tweening;
using Project9.Data;
using Project9.Presentation;
using Project9.Runtime;
using Project9.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Project9.UI
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class Project9SceneRoot : MonoBehaviour
    {
        private const string RuntimeUiName = "POC9_RuntimeUI";
        private const string TooltipName = "HoverTooltip";
        private const string DefaultFontResourcePath = "Fonts & Materials/KoPubWorld Batang Bold 2";

        [SerializeField] private Project9ScenarioDefinition scenario;
        [SerializeField] private RectTransform uiRoot;
        [SerializeField] private bool buildInEditMode = true;
        [SerializeField] private TMP_FontAsset fontAsset;

        [Header("Generated Layout")]
        [SerializeField, Min(0)] private int rootPaddingHorizontal = 20;
        [SerializeField, Min(0)] private int rootPaddingVertical = 20;
        [SerializeField, Min(0)] private float rootSpacing = 14f;
        [SerializeField, Min(0)] private float headerHeight = 120f;
        [SerializeField, Min(0)] private float bodyHeight = 780f;
        [SerializeField, Min(0)] private float footerHeight = 88f;
        [SerializeField, Min(0)] private float bodyColumnSpacing = 14f;
        [SerializeField, Min(0)] private float reportPanelPreferredWidth = 1460f;
        [SerializeField, Min(0)] private float targetPanelPreferredWidth = 360f;
        [SerializeField, Min(0)] private float paragraphRowHeight = 132f;
        [SerializeField, Min(0)] private float paragraphTextPreferredWidth = 980f;
        [SerializeField, Min(0)] private float paragraphButtonsPreferredWidth = 380f;
        [SerializeField] private Vector2 paragraphButtonCellSize = new(184f, 40f);
        [SerializeField] private Vector2 paragraphButtonSpacing = new(8f, 8f);
        [SerializeField, Min(0)] private float targetCardHeight = 124f;
        [SerializeField, Min(0)] private float submitButtonHeight = 52f;
        [SerializeField] private Vector2 tooltipSize = new(420f, 220f);

        private readonly Dictionary<string, TextMeshProUGUI> _paragraphTexts = new();
        private readonly Dictionary<string, TextMeshProUGUI> _paragraphStats = new();
        private readonly Dictionary<string, TextMeshProUGUI> _targetReputations = new();
        private readonly Dictionary<string, Image> _targetBackgrounds = new();
        private readonly Dictionary<string, Tween> _targetColorTweens = new();
        private readonly List<Button> _editButtons = new();

        private Project9Presenter _presenter;
        private Canvas _canvas;
        private RectTransform _tooltipPanel;
        private CanvasGroup _tooltipCanvasGroup;
        private TextMeshProUGUI _tooltipTitle;
        private TextMeshProUGUI _tooltipBody;
        private TextMeshProUGUI _titleText;
        private TextMeshProUGUI _summaryText;
        private TextMeshProUGUI _resultText;
        private Tween _tooltipTween;
        private Tween _resultTween;

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (Application.isPlaying || !buildInEditMode)
            {
                return;
            }

            EditorApplication.delayCall -= BuildEditorUiIfNeeded;
            EditorApplication.delayCall += BuildEditorUiIfNeeded;
        }

        private void OnValidate()
        {
            if (Application.isPlaying || !buildInEditMode)
            {
                return;
            }

            EditorApplication.delayCall -= BuildEditorUiIfNeeded;
            EditorApplication.delayCall += BuildEditorUiIfNeeded;
        }
#endif

        private void Start()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            BindExistingLayout();
        }

        [ContextMenu("Build Project9 UI")]
        public void Build()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Project9 UI must be prebuilt in edit mode. Runtime building is disabled.", this);
                return;
            }

            if (scenario == null)
            {
                Debug.LogWarning("Project9SceneRoot needs a scenario asset.", this);
                return;
            }

            EnsureEventSystem();
            ClearExistingRuntimeUi();
            EnsureFontAsset();

            _paragraphTexts.Clear();
            _paragraphStats.Clear();
            _targetReputations.Clear();
            _targetBackgrounds.Clear();
            KillTargetColorTweens();
            _editButtons.Clear();

            _presenter?.Dispose();
            _presenter = new Project9Presenter(
                new ReportSessionFactory(),
                new SubmissionScoringSystem(),
                new ReputationSystem());

            var root = EnsureUiRoot();
            BuildLayout(root);
            BindExistingLayout();

#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
        }

        private void OnDestroy()
        {
            _tooltipTween?.Kill();
            _resultTween?.Kill();
            KillTargetColorTweens();
            _presenter?.Dispose();
        }

        private RectTransform EnsureUiRoot()
        {
            if (uiRoot != null)
            {
                return uiRoot;
            }

            var existing = transform.Find(RuntimeUiName);
            if (existing != null && existing.TryGetComponent(out RectTransform existingRect))
            {
                uiRoot = existingRect;
                return uiRoot;
            }

            var root = CreateRect(RuntimeUiName, transform);
            Stretch(root);
            uiRoot = root;
            return uiRoot;
        }

        private void BuildLayout(RectTransform root)
        {
            _canvas = GetComponentInParent<Canvas>();

            var background = root.gameObject.AddComponent<Image>();
            background.color = new Color(0.08f, 0.09f, 0.1f, 1f);

            var vertical = root.gameObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(
                rootPaddingHorizontal,
                rootPaddingHorizontal,
                rootPaddingVertical,
                rootPaddingVertical);
            vertical.spacing = rootSpacing;
            vertical.childControlWidth = true;
            vertical.childControlHeight = true;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            BuildHeader(root);
            BuildBody(root);
            BuildFooter(root);
            BuildTooltip(GetTooltipParent(root));
        }

        private void BuildHeader(Transform parent)
        {
            var header = CreatePanel("Header", parent, new Color(0.13f, 0.14f, 0.16f, 1f));
            AddLayoutElement(header, -1, headerHeight);

            var layout = header.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 14, 14);
            layout.spacing = 8;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;

            _titleText = CreateText("Title", header, 30, FontStyles.Bold, TextAlignmentOptions.Left);
            _summaryText = CreateText("Summary", header, 18, FontStyles.Normal, TextAlignmentOptions.Left);
            _summaryText.color = new Color(0.78f, 0.8f, 0.82f, 1f);
        }

        private void BuildBody(Transform parent)
        {
            var body = CreateRect("Body", parent);
            AddLayoutElement(body, -1, bodyHeight, flexibleHeight: 1);

            var horizontal = body.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontal.spacing = bodyColumnSpacing;
            horizontal.childControlWidth = true;
            horizontal.childControlHeight = true;
            horizontal.childForceExpandWidth = false;
            horizontal.childForceExpandHeight = true;

            BuildReportPanel(body);
            BuildTargetPanel(body);
        }

        private void BuildReportPanel(Transform parent)
        {
            var panel = CreatePanel("ReportPanel", parent, new Color(0.16f, 0.17f, 0.18f, 1f));
            AddLayoutElement(panel, reportPanelPreferredWidth, -1, flexibleWidth: 1);

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 14, 14);
            layout.spacing = 10;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            foreach (var paragraph in scenario.Report.Paragraphs)
            {
                BuildParagraphRow(panel, paragraph);
            }
        }

        private void BuildParagraphRow(Transform parent, ParagraphDefinition paragraph)
        {
            var row = CreatePanel($"Paragraph_{paragraph.Id}", parent, new Color(0.22f, 0.23f, 0.24f, 1f));
            AddLayoutElement(row, -1, paragraphRowHeight);
            AttachHover(
                row.gameObject,
                () => paragraph.Title,
                () => BuildParagraphTooltip(paragraph));

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 10, 10);
            layout.spacing = 12;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var textBox = CreateRect("TextBox", row);
            AddLayoutElement(textBox, paragraphTextPreferredWidth, -1, flexibleWidth: 1);

            var textLayout = textBox.gameObject.AddComponent<VerticalLayoutGroup>();
            textLayout.spacing = 4;
            textLayout.childControlWidth = true;
            textLayout.childControlHeight = true;
            textLayout.childForceExpandWidth = true;

            var title = CreateText("Title", textBox, 18, FontStyles.Bold, TextAlignmentOptions.Left);
            title.text = paragraph.Title;

            var body = CreateText("Body", textBox, 16, FontStyles.Normal, TextAlignmentOptions.Left);
            body.textWrappingMode = TextWrappingModes.Normal;
            body.name = $"ParagraphBody_{paragraph.Id}";
            _paragraphTexts.Add(paragraph.Id, body);

            var stat = CreateText("Stats", textBox, 14, FontStyles.Normal, TextAlignmentOptions.Left);
            stat.name = $"ParagraphStats_{paragraph.Id}";
            stat.color = new Color(0.74f, 0.76f, 0.78f, 1f);
            _paragraphStats.Add(paragraph.Id, stat);

            var buttons = CreateRect("EditButtons", row);
            AddLayoutElement(buttons, paragraphButtonsPreferredWidth, -1);
            var buttonLayout = buttons.gameObject.AddComponent<GridLayoutGroup>();
            buttonLayout.cellSize = paragraphButtonCellSize;
            buttonLayout.spacing = paragraphButtonSpacing;
            buttonLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            buttonLayout.constraintCount = 2;

            foreach (var option in paragraph.EditOptions)
            {
                var button = CreateButton(option.Label, buttons);
                button.name = $"EditButton_{paragraph.Id}_{option.Id}";
                button.onClick.AddListener(() => SelectParagraphEditOption(paragraph.Id, option.Id));
                AttachHover(
                    button.gameObject,
                    () => option.Label,
                    () => BuildEditOptionTooltip(paragraph, option));
                _editButtons.Add(button);
            }
        }

        private void BuildTargetPanel(Transform parent)
        {
            var panel = CreatePanel("TargetPanel", parent, new Color(0.16f, 0.17f, 0.18f, 1f));
            AddLayoutElement(panel, targetPanelPreferredWidth, -1);

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 14, 14);
            layout.spacing = 12;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var heading = CreateText("Heading", panel, 22, FontStyles.Bold, TextAlignmentOptions.Left);
            heading.text = "제출 대상";

            foreach (var target in scenario.SubmitTargets)
            {
                BuildTargetCard(panel, target);
            }

            var submit = CreateButton("선택 대상에게 제출", panel);
            submit.name = "SubmitButton";
            AddLayoutElement((RectTransform)submit.transform, -1, submitButtonHeight);
            submit.onClick.AddListener(SubmitSelectedTarget);
        }

        private void BuildTargetCard(Transform parent, SubmitTargetDefinition target)
        {
            var card = CreatePanel($"Target_{target.Id}", parent, new Color(0.22f, 0.23f, 0.24f, 1f));
            AddLayoutElement(card, -1, targetCardHeight);
            AttachHover(
                card.gameObject,
                () => target.DisplayName,
                () => BuildTargetTooltip(target));
            _targetBackgrounds.Add(target.Id, card.GetComponent<Image>());

            var button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = card.GetComponent<Image>();
            button.onClick.AddListener(() => SelectSubmitTarget(target.Id));

            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 10, 10);
            layout.spacing = 6;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;

            var name = CreateText("Name", card, 20, FontStyles.Bold, TextAlignmentOptions.Left);
            name.text = target.DisplayName;

            var description = CreateText("Description", card, 15, FontStyles.Normal, TextAlignmentOptions.Left);
            description.text = target.Description;
            description.textWrappingMode = TextWrappingModes.Normal;
            description.color = new Color(0.78f, 0.8f, 0.82f, 1f);

            var reputation = CreateText("Reputation", card, 16, FontStyles.Bold, TextAlignmentOptions.Left);
            reputation.name = $"TargetReputation_{target.Id}";
            _targetReputations.Add(target.Id, reputation);
        }

        private void BuildFooter(Transform parent)
        {
            var footer = CreatePanel("ResultPanel", parent, new Color(0.13f, 0.14f, 0.16f, 1f));
            AddLayoutElement(footer, -1, footerHeight);

            var layout = footer.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 12, 12);
            layout.spacing = 4;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = true;

            _resultText = CreateText("ResultText", footer, 18, FontStyles.Normal, TextAlignmentOptions.Left);
            _resultText.text = "아직 제출하지 않았습니다.";
        }

        private void BuildTooltip(Transform parent)
        {
            _tooltipPanel = CreatePanel(TooltipName, parent, new Color(0.07f, 0.075f, 0.08f, 0.96f));
            _tooltipCanvasGroup = _tooltipPanel.gameObject.AddComponent<CanvasGroup>();
            _tooltipCanvasGroup.alpha = 0f;
            _tooltipPanel.SetAsLastSibling();
            _tooltipPanel.anchorMin = Vector2.zero;
            _tooltipPanel.anchorMax = Vector2.zero;
            _tooltipPanel.pivot = new Vector2(0, 1);
            _tooltipPanel.sizeDelta = tooltipSize;
            _tooltipPanel.localScale = Vector3.one * 0.96f;

            var layoutElement = _tooltipPanel.gameObject.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            var layout = _tooltipPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 12, 12);
            layout.spacing = 8;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            _tooltipTitle = CreateText("TooltipTitle", _tooltipPanel, 18, FontStyles.Bold, TextAlignmentOptions.Left);
            _tooltipBody = CreateText("TooltipBody", _tooltipPanel, 15, FontStyles.Normal, TextAlignmentOptions.Left);
            _tooltipBody.textWrappingMode = TextWrappingModes.Normal;
            _tooltipBody.color = new Color(0.82f, 0.84f, 0.86f, 1f);

            _tooltipPanel.gameObject.SetActive(false);
        }

        private void ApplyReport(ReportViewModel viewModel)
        {
            _titleText.text = viewModel.Title;
            _summaryText.text = viewModel.Summary;

            foreach (var paragraph in viewModel.Paragraphs)
            {
                if (_paragraphTexts.TryGetValue(paragraph.Id, out var text))
                {
                    text.text = paragraph.Text;
                }

                if (_paragraphStats.TryGetValue(paragraph.Id, out var stat))
                {
                    stat.text = $"가치 {paragraph.InformationValue} / 민감도 {paragraph.Sensitivity} / 무결성 {paragraph.Integrity} / 노출 {paragraph.Exposure} / {ToKoreanAction(paragraph.ActionType)}";
                }
            }

            foreach (var target in viewModel.Targets)
            {
                if (_targetReputations.TryGetValue(target.Id, out var reputation))
                {
                    reputation.text = $"평판 {target.Reputation:+#;-#;0}";
                }

                if (_targetBackgrounds.TryGetValue(target.Id, out var background))
                {
                    var targetColor = target.IsSelected
                        ? new Color(0.35f, 0.42f, 0.34f, 1f)
                        : new Color(0.22f, 0.23f, 0.24f, 1f);
                    AnimateTargetBackground(target.Id, background, targetColor);
                }
            }
        }

        private void ApplySubmissionResult(SubmissionResultViewModel viewModel)
        {
            var sign = viewModel.ReputationDelta >= 0 ? "+" : string.Empty;
            _resultText.text = $"{viewModel.TargetName} 제출 결과: 점수 {viewModel.TotalScore}, 평판 {sign}{viewModel.ReputationDelta}";
            _resultTween?.Kill();
            _resultText.rectTransform.localScale = Vector3.one;
            _resultTween = _resultText.rectTransform
                .DOPunchScale(new Vector3(0.04f, 0.04f, 0f), 0.22f, 8, 0.7f);
        }

        private void SelectParagraphEditOption(string paragraphId, string editOptionId)
        {
            if (_presenter.SelectParagraphEditOption(paragraphId, editOptionId))
            {
                ApplyReport(_presenter.CurrentReport);
            }
        }

        private void SelectSubmitTarget(string targetId)
        {
            if (_presenter.SelectSubmitTarget(targetId))
            {
                ApplyReport(_presenter.CurrentReport);
            }
        }

        private void SubmitSelectedTarget()
        {
            var result = _presenter.SubmitSelectedTarget();
            ApplySubmissionResult(result);
            ApplyReport(_presenter.CurrentReport);
        }

        private string BuildParagraphTooltip(ParagraphDefinition paragraph)
        {
            var state = _presenter?.Session?.GetParagraph(paragraph.Id);
            if (state == null)
            {
                return paragraph.OriginalText;
            }

            return
                $"정보 가치: {paragraph.InformationValue}\n" +
                $"민감도: {paragraph.Sensitivity}\n" +
                $"현재 무결성: {state.CurrentIntegrity}\n" +
                $"현재 노출도: {state.CurrentExposure}\n" +
                $"현재 처리: {ToKoreanAction(state.CurrentActionType)}\n\n" +
                state.CurrentText;
        }

        private static string BuildEditOptionTooltip(ParagraphDefinition paragraph, ParagraphEditOption option)
        {
            return
                $"대상 단락: {paragraph.Title}\n" +
                $"행동: {ToKoreanAction(option.ActionType)}\n" +
                $"무결성 변화: {option.IntegrityDelta}\n" +
                $"노출도 변화: {option.ExposureDelta}\n" +
                $"왜곡 패널티: {option.DistortionPenalty}\n\n" +
                option.ResultingText;
        }

        private string BuildTargetTooltip(SubmitTargetDefinition target)
        {
            var preview = _presenter?.PreviewSubmission(target.Id);
            if (preview == null)
            {
                return target.Description;
            }

            var sign = preview.ReputationDelta >= 0 ? "+" : string.Empty;
            return
                $"{target.Description}\n\n" +
                $"현재 문서 예상 점수: {preview.TotalScore}\n" +
                $"예상 평판 변화: {sign}{preview.ReputationDelta}";
        }

        private void AttachHover(GameObject target, Func<string> titleFactory, Func<string> bodyFactory)
        {
            var hover = target.GetComponent<Project9HoverSource>();
            if (hover == null)
            {
                hover = target.AddComponent<Project9HoverSource>();
            }

            hover.Configure(
                eventData => ShowTooltip(titleFactory(), bodyFactory(), eventData),
                MoveTooltip,
                HideTooltip);
        }

        private void ShowTooltip(string title, string body, PointerEventData eventData)
        {
            if (_tooltipPanel == null)
            {
                return;
            }

            _tooltipTitle.text = title;
            _tooltipBody.text = body;
            _tooltipPanel.gameObject.SetActive(true);
            _tooltipPanel.SetAsLastSibling();
            MoveTooltip(eventData);

            _tooltipTween?.Kill();
            _tooltipCanvasGroup.alpha = 0f;
            _tooltipPanel.localScale = Vector3.one * 0.96f;
            _tooltipTween = DOTween.Sequence()
                .Join(_tooltipCanvasGroup.DOFade(1f, 0.12f))
                .Join(_tooltipPanel.DOScale(1f, 0.12f).SetEase(Ease.OutQuad));
        }

        private void MoveTooltip(PointerEventData eventData)
        {
            if (_tooltipPanel == null || !_tooltipPanel.gameObject.activeSelf)
            {
                return;
            }

            var canvasRect = GetCanvasRect();
            if (canvasRect == null)
            {
                return;
            }

            if (_tooltipPanel.parent != canvasRect)
            {
                _tooltipPanel.SetParent(canvasRect, false);
                _tooltipPanel.SetAsLastSibling();
            }

            NormalizeTooltipRect();

            var camera = eventData.enterEventCamera != null
                ? eventData.enterEventCamera
                : _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, camera, out var localPoint))
            {
                return;
            }

            var pivotOffset = new Vector2(
                canvasRect.rect.width * canvasRect.pivot.x,
                canvasRect.rect.height * canvasRect.pivot.y);
            var bottomLeftPoint = localPoint + pivotOffset;

            _tooltipPanel.anchoredPosition = bottomLeftPoint + new Vector2(18, -18);
        }

        private void HideTooltip()
        {
            if (_tooltipPanel == null || !_tooltipPanel.gameObject.activeSelf)
            {
                return;
            }

            _tooltipTween?.Kill();
            _tooltipTween = _tooltipCanvasGroup
                .DOFade(0f, 0.08f)
                .OnComplete(() => _tooltipPanel.gameObject.SetActive(false));
        }

        private void AnimateTargetBackground(string targetId, Image background, Color targetColor)
        {
            if (!Application.isPlaying)
            {
                background.color = targetColor;
                return;
            }

            if (_targetColorTweens.TryGetValue(targetId, out var tween))
            {
                tween.Kill();
            }

            _targetColorTweens[targetId] = background
                .DOColor(targetColor, 0.16f)
                .SetEase(Ease.OutQuad);
        }

        private void KillTargetColorTweens()
        {
            foreach (var tween in _targetColorTweens.Values)
            {
                tween?.Kill();
            }

            _targetColorTweens.Clear();
        }

        private void ClearExistingRuntimeUi()
        {
            var child = transform.Find(RuntimeUiName);
            if (child != null)
            {
                DestroyGeneratedObject(child.gameObject);
            }

            var canvasRect = GetCanvasRect();
            if (canvasRect == null)
            {
                return;
            }

            for (var i = canvasRect.childCount - 1; i >= 0; i--)
            {
                var canvasChild = canvasRect.GetChild(i);
                if (canvasChild.name == TooltipName)
                {
                    DestroyGeneratedObject(canvasChild.gameObject);
                }
            }
        }

        private static void DestroyGeneratedObject(GameObject target)
        {
            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }

        private static RectTransform CreatePanel(string name, Transform parent, Color color)
        {
            var rect = CreateRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return rect;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = 5;
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);
            rect.localScale = Vector3.one;
            return rect;
        }

        private static TextMeshProUGUI CreateText(
            string name,
            Transform parent,
            float fontSize,
            FontStyles fontStyle,
            TextAlignmentOptions alignment)
        {
            var rect = CreateRect(name, parent);
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = new Color(0.92f, 0.93f, 0.94f, 1f);
            text.raycastTarget = false;
            var sceneRoot = parent.GetComponentInParent<Project9SceneRoot>();
            if (sceneRoot != null)
            {
                sceneRoot.EnsureFontAsset();
                if (sceneRoot.fontAsset != null)
                {
                    text.font = sceneRoot.fontAsset;
                }
            }
            return text;
        }

        private static Button CreateButton(string label, Transform parent)
        {
            var rect = CreatePanel($"Button_{label}", parent, new Color(0.28f, 0.3f, 0.32f, 1f));
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();

            var text = CreateText("Label", rect, 15, FontStyles.Bold, TextAlignmentOptions.Center);
            Stretch(text.rectTransform);
            text.text = label;

            return button;
        }

        private static void AddLayoutElement(
            RectTransform rect,
            float preferredWidth,
            float preferredHeight,
            float flexibleWidth = 0,
            float flexibleHeight = 0)
        {
            var layoutElement = rect.gameObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = preferredWidth;
            layoutElement.preferredHeight = preferredHeight;
            layoutElement.flexibleWidth = flexibleWidth;
            layoutElement.flexibleHeight = flexibleHeight;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void EnsureEventSystem()
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                var eventSystemObject = new GameObject("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            foreach (var oldModule in eventSystem.GetComponents<StandaloneInputModule>())
            {
                if (Application.isPlaying)
                {
                    Destroy(oldModule);
                }
                else
                {
                    DestroyImmediate(oldModule);
                }
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }

        private static string ToKoreanAction(ParagraphActionType actionType)
        {
            return actionType switch
            {
                ParagraphActionType.Keep => "유지",
                ParagraphActionType.PartialMask => "부분 마스킹",
                ParagraphActionType.FullMask => "전체 마스킹",
                ParagraphActionType.RewritePreserveMeaning => "의미 보존 수정",
                ParagraphActionType.RewriteDistortMeaning => "의미 왜곡 수정",
                _ => "알 수 없음"
            };
        }

        private void BindExistingLayout()
        {
            if (scenario == null)
            {
                Debug.LogWarning("Project9SceneRoot needs a scenario asset.", this);
                return;
            }

            EnsureFontAsset();
            EnsureEventSystem();

            var root = uiRoot != null ? uiRoot : transform.Find(RuntimeUiName) as RectTransform;
            if (root == null)
            {
                Debug.LogWarning("POC9 UI is not prebuilt. Use the context menu 'Build Project9 UI' in edit mode.", this);
                return;
            }

            uiRoot = root;
            _canvas = GetComponentInParent<Canvas>();
            _paragraphTexts.Clear();
            _paragraphStats.Clear();
            _targetReputations.Clear();
            _targetBackgrounds.Clear();
            _editButtons.Clear();
            KillTargetColorTweens();

            _titleText = FindText(root, "Title");
            _summaryText = FindText(root, "Summary");
            _resultText = FindText(root, "ResultText");
            var tooltipSearchRoot = GetTooltipParent(root);
            _tooltipPanel = FindRect(tooltipSearchRoot, TooltipName) ?? FindRect(root, TooltipName);
            if (_tooltipPanel != null)
            {
                var canvasRect = GetCanvasRect();
                if (canvasRect != null && _tooltipPanel.parent != canvasRect)
                {
                    _tooltipPanel.SetParent(canvasRect, false);
                }

                NormalizeTooltipRect();
                _tooltipPanel.SetAsLastSibling();
                _tooltipCanvasGroup = _tooltipPanel.GetComponent<CanvasGroup>();
                if (_tooltipCanvasGroup == null)
                {
                    _tooltipCanvasGroup = _tooltipPanel.gameObject.AddComponent<CanvasGroup>();
                }

                _tooltipTitle = FindText(_tooltipPanel, "TooltipTitle");
                _tooltipBody = FindText(_tooltipPanel, "TooltipBody");
            }

            _presenter?.Dispose();
            _presenter = new Project9Presenter(
                new ReportSessionFactory(),
                new SubmissionScoringSystem(),
                new ReputationSystem());

            foreach (var paragraph in scenario.Report.Paragraphs)
            {
                var row = FindRect(root, $"Paragraph_{paragraph.Id}");
                if (row != null)
                {
                    AttachHover(row.gameObject, () => paragraph.Title, () => BuildParagraphTooltip(paragraph));
                }

                var body = FindText(root, $"ParagraphBody_{paragraph.Id}");
                if (body != null)
                {
                    _paragraphTexts[paragraph.Id] = body;
                }

                var stats = FindText(root, $"ParagraphStats_{paragraph.Id}");
                if (stats != null)
                {
                    _paragraphStats[paragraph.Id] = stats;
                }

                foreach (var option in paragraph.EditOptions)
                {
                    var button = FindButton(root, $"EditButton_{paragraph.Id}_{option.Id}");
                    if (button == null)
                    {
                        continue;
                    }

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => SelectParagraphEditOption(paragraph.Id, option.Id));
                    AttachHover(button.gameObject, () => option.Label, () => BuildEditOptionTooltip(paragraph, option));
                    _editButtons.Add(button);
                }
            }

            foreach (var target in scenario.SubmitTargets)
            {
                var card = FindRect(root, $"Target_{target.Id}");
                if (card != null)
                {
                    var background = card.GetComponent<Image>();
                    if (background != null)
                    {
                        _targetBackgrounds[target.Id] = background;
                    }

                    var button = card.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(() => SelectSubmitTarget(target.Id));
                    }

                    AttachHover(card.gameObject, () => target.DisplayName, () => BuildTargetTooltip(target));
                }

                var reputation = FindText(root, $"TargetReputation_{target.Id}");
                if (reputation != null)
                {
                    _targetReputations[target.Id] = reputation;
                }
            }

            var submit = FindButton(root, "SubmitButton");
            if (submit != null)
            {
                submit.onClick.RemoveAllListeners();
                submit.onClick.AddListener(SubmitSelectedTarget);
            }

            ApplyFont(root);
            _presenter.Initialize(scenario);
            ApplyReport(_presenter.CurrentReport);
        }

        private void EnsureFontAsset()
        {
            if (fontAsset != null)
            {
                return;
            }

            fontAsset = Resources.Load<TMP_FontAsset>(DefaultFontResourcePath);
        }

        private void ApplyFont(Transform root)
        {
            EnsureFontAsset();
            if (fontAsset == null || root == null)
            {
                return;
            }

            foreach (var text in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                text.font = fontAsset;
            }
        }

        private Transform GetTooltipParent(RectTransform fallback)
        {
            var canvasRect = GetCanvasRect();
            return canvasRect != null ? canvasRect : fallback;
        }

        private RectTransform GetCanvasRect()
        {
            if (_canvas == null)
            {
                _canvas = GetComponentInParent<Canvas>();
            }

            return _canvas != null ? (RectTransform)_canvas.transform : null;
        }

        private void NormalizeTooltipRect()
        {
            if (_tooltipPanel == null)
            {
                return;
            }

            _tooltipPanel.anchorMin = Vector2.zero;
            _tooltipPanel.anchorMax = Vector2.zero;
            _tooltipPanel.pivot = new Vector2(0, 1);
        }

        private static TextMeshProUGUI FindText(Transform root, string objectName)
        {
            var found = FindChild(root, objectName);
            return found != null ? found.GetComponent<TextMeshProUGUI>() : null;
        }

        private static Button FindButton(Transform root, string objectName)
        {
            var found = FindChild(root, objectName);
            return found != null ? found.GetComponent<Button>() : null;
        }

        private static RectTransform FindRect(Transform root, string objectName)
        {
            var found = FindChild(root, objectName);
            return found as RectTransform;
        }

        private static Transform FindChild(Transform root, string objectName)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == objectName)
            {
                return root;
            }

            for (var i = 0; i < root.childCount; i++)
            {
                var found = FindChild(root.GetChild(i), objectName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

#if UNITY_EDITOR
        private void BuildEditorUiIfNeeded()
        {
            EditorApplication.delayCall -= BuildEditorUiIfNeeded;

            if (this == null || Application.isPlaying || !buildInEditMode || scenario == null)
            {
                return;
            }

            var root = transform.Find(RuntimeUiName);
            if (root != null)
            {
                BindExistingLayout();
                return;
            }

            Build();
        }
#endif
    }
}
