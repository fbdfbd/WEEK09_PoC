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
using UnityEngine.UI;

namespace Project9.UI
{
    [DisallowMultipleComponent]
    public sealed class Project9SceneRoot : MonoBehaviour
    {
        [SerializeField] private Project9ScenarioDefinition scenario;
        [SerializeField] private RectTransform uiRoot;
        [SerializeField] private bool rebuildOnStart = true;

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

        private void Start()
        {
            if (!rebuildOnStart)
            {
                return;
            }

            Build();
        }

        [ContextMenu("Build Project9 UI")]
        public void Build()
        {
            if (scenario == null)
            {
                Debug.LogWarning("Project9SceneRoot needs a scenario asset.", this);
                return;
            }

            EnsureEventSystem();
            ClearExistingRuntimeUi();

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

            _presenter.Initialize(scenario);
            ApplyReport(_presenter.CurrentReport);
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

            var existing = transform.Find("POC9_RuntimeUI");
            if (existing != null && existing.TryGetComponent(out RectTransform existingRect))
            {
                uiRoot = existingRect;
                return uiRoot;
            }

            var root = CreateRect("POC9_RuntimeUI", transform);
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
            vertical.padding = new RectOffset(28, 28, 24, 24);
            vertical.spacing = 16;
            vertical.childControlWidth = true;
            vertical.childControlHeight = true;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            BuildHeader(root);
            BuildBody(root);
            BuildFooter(root);
            BuildTooltip(root);
        }

        private void BuildHeader(Transform parent)
        {
            var header = CreatePanel("Header", parent, new Color(0.13f, 0.14f, 0.16f, 1f));
            AddLayoutElement(header, -1, 130);

            var layout = header.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 14, 14);
            layout.spacing = 8;

            _titleText = CreateText("Title", header, 30, FontStyles.Bold, TextAlignmentOptions.Left);
            _summaryText = CreateText("Summary", header, 18, FontStyles.Normal, TextAlignmentOptions.Left);
            _summaryText.color = new Color(0.78f, 0.8f, 0.82f, 1f);
        }

        private void BuildBody(Transform parent)
        {
            var body = CreateRect("Body", parent);
            AddLayoutElement(body, -1, 760, flexibleHeight: 1);

            var horizontal = body.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontal.spacing = 16;
            horizontal.childControlWidth = true;
            horizontal.childControlHeight = true;
            horizontal.childForceExpandWidth = true;
            horizontal.childForceExpandHeight = true;

            BuildReportPanel(body);
            BuildTargetPanel(body);
        }

        private void BuildReportPanel(Transform parent)
        {
            var panel = CreatePanel("ReportPanel", parent, new Color(0.16f, 0.17f, 0.18f, 1f));
            AddLayoutElement(panel, 1280, -1, flexibleWidth: 1);

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 14, 14);
            layout.spacing = 10;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;

            foreach (var paragraph in scenario.Report.Paragraphs)
            {
                BuildParagraphRow(panel, paragraph);
            }
        }

        private void BuildParagraphRow(Transform parent, ParagraphDefinition paragraph)
        {
            var row = CreatePanel($"Paragraph_{paragraph.Id}", parent, new Color(0.22f, 0.23f, 0.24f, 1f));
            AddLayoutElement(row, -1, 116);
            AttachHover(
                row.gameObject,
                () => paragraph.Title,
                () => BuildParagraphTooltip(paragraph));

            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 10, 10);
            layout.spacing = 12;
            layout.childControlWidth = true;
            layout.childControlHeight = true;

            var textBox = CreateRect("TextBox", row);
            AddLayoutElement(textBox, 760, -1, flexibleWidth: 1);

            var textLayout = textBox.gameObject.AddComponent<VerticalLayoutGroup>();
            textLayout.spacing = 4;
            textLayout.childControlHeight = true;

            var title = CreateText("Title", textBox, 18, FontStyles.Bold, TextAlignmentOptions.Left);
            title.text = paragraph.Title;

            var body = CreateText("Body", textBox, 16, FontStyles.Normal, TextAlignmentOptions.Left);
            body.textWrappingMode = TextWrappingModes.Normal;
            _paragraphTexts.Add(paragraph.Id, body);

            var stat = CreateText("Stats", textBox, 14, FontStyles.Normal, TextAlignmentOptions.Left);
            stat.color = new Color(0.74f, 0.76f, 0.78f, 1f);
            _paragraphStats.Add(paragraph.Id, stat);

            var buttons = CreateRect("EditButtons", row);
            AddLayoutElement(buttons, 440, -1);
            var buttonLayout = buttons.gameObject.AddComponent<GridLayoutGroup>();
            buttonLayout.cellSize = new Vector2(210, 40);
            buttonLayout.spacing = new Vector2(8, 8);
            buttonLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            buttonLayout.constraintCount = 2;

            foreach (var option in paragraph.EditOptions)
            {
                var button = CreateButton(option.Label, buttons);
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
            AddLayoutElement(panel, 460, -1);

            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 14, 14);
            layout.spacing = 12;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;

            var heading = CreateText("Heading", panel, 22, FontStyles.Bold, TextAlignmentOptions.Left);
            heading.text = "제출 대상";

            foreach (var target in scenario.SubmitTargets)
            {
                BuildTargetCard(panel, target);
            }

            var submit = CreateButton("선택 대상에게 제출", panel);
            AddLayoutElement((RectTransform)submit.transform, -1, 52);
            submit.onClick.AddListener(SubmitSelectedTarget);
        }

        private void BuildTargetCard(Transform parent, SubmitTargetDefinition target)
        {
            var card = CreatePanel($"Target_{target.Id}", parent, new Color(0.22f, 0.23f, 0.24f, 1f));
            AddLayoutElement(card, -1, 138);
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

            var name = CreateText("Name", card, 20, FontStyles.Bold, TextAlignmentOptions.Left);
            name.text = target.DisplayName;

            var description = CreateText("Description", card, 15, FontStyles.Normal, TextAlignmentOptions.Left);
            description.text = target.Description;
            description.textWrappingMode = TextWrappingModes.Normal;
            description.color = new Color(0.78f, 0.8f, 0.82f, 1f);

            var reputation = CreateText("Reputation", card, 16, FontStyles.Bold, TextAlignmentOptions.Left);
            _targetReputations.Add(target.Id, reputation);
        }

        private void BuildFooter(Transform parent)
        {
            var footer = CreatePanel("ResultPanel", parent, new Color(0.13f, 0.14f, 0.16f, 1f));
            AddLayoutElement(footer, -1, 96);

            var layout = footer.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 12, 12);
            layout.spacing = 4;

            _resultText = CreateText("ResultText", footer, 18, FontStyles.Normal, TextAlignmentOptions.Left);
            _resultText.text = "아직 제출하지 않았습니다.";
        }

        private void BuildTooltip(Transform parent)
        {
            _tooltipPanel = CreatePanel("HoverTooltip", parent, new Color(0.07f, 0.075f, 0.08f, 0.96f));
            _tooltipCanvasGroup = _tooltipPanel.gameObject.AddComponent<CanvasGroup>();
            _tooltipCanvasGroup.alpha = 0f;
            _tooltipPanel.SetAsLastSibling();
            _tooltipPanel.anchorMin = new Vector2(0, 1);
            _tooltipPanel.anchorMax = new Vector2(0, 1);
            _tooltipPanel.pivot = new Vector2(0, 1);
            _tooltipPanel.sizeDelta = new Vector2(380, 210);
            _tooltipPanel.localScale = Vector3.one * 0.96f;

            var layoutElement = _tooltipPanel.gameObject.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            var layout = _tooltipPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(14, 14, 12, 12);
            layout.spacing = 8;
            layout.childControlHeight = true;
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
                    stat.text = $"가치 {paragraph.InformationValue} / 민감도 {paragraph.Sensitivity} / 무결성 {paragraph.Integrity} / 노출 {paragraph.Exposure} / {paragraph.ActionType}";
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
                $"현재 처리: {state.CurrentActionType}\n\n" +
                state.CurrentText;
        }

        private static string BuildEditOptionTooltip(ParagraphDefinition paragraph, ParagraphEditOption option)
        {
            return
                $"대상 단락: {paragraph.Title}\n" +
                $"행동: {option.ActionType}\n" +
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

            var root = uiRoot != null ? uiRoot : (RectTransform)transform;
            var camera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(root, eventData.position, camera, out var localPoint))
            {
                return;
            }

            _tooltipPanel.anchoredPosition = localPoint + new Vector2(18, -18);
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
            var child = transform.Find("POC9_RuntimeUI");
            if (child == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
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
            if (EventSystem.current != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }
}
