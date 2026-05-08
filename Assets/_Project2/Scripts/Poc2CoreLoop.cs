using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Project2
{
    public sealed class Poc2CoreLoop : MonoBehaviour
    {
        private enum GameState
        {
            Ready,
            Playing,
            Success
        }

        [System.Serializable]
        public sealed class GearOption
        {
            public int frequencyX = 1;
            public int frequencyY = 1;
            public Button button;
            public Text label;
        }

        [System.Serializable]
        public sealed class WeldNode
        {
            public Vector2 normalizedPosition;
            public Transform view;
            public Text label;
            public Renderer renderer;
            [HideInInspector] public float progress;
            [HideInInspector] public bool fixedDone;
        }

        [Header("Curves")]
        [SerializeField] private LineRenderer redCurve;
        [SerializeField] private LineRenderer blueCurve;
        [SerializeField] private Transform curveRoot;
        [SerializeField] private int targetFrequencyX = 3;
        [SerializeField] private int targetFrequencyY = 2;
        [SerializeField] private int curveResolution = 300;
        [SerializeField] private float amplitude = 3.2f;

        [Header("Player")]
        [SerializeField] private int playerFrequencyX = 1;
        [SerializeField] private int playerFrequencyY = 2;
        [SerializeField] private float startPhase = 0.7853982f;
        [SerializeField] private float dragSensitivity = 0.015f;
        [SerializeField] private float smoothSpeed = 12f;

        [Header("Sparks")]
        [SerializeField] private Transform[] sparkViews;
        [SerializeField] private float sparkCellSize = 0.08f;
        [SerializeField] private float sparkMergeDistance = 0.28f;

        [Header("Welding")]
        [SerializeField] private WeldNode[] nodes;
        [SerializeField] private float weldRadius = 0.35f;
        [SerializeField] private float weldSpeed = 55f;
        [SerializeField] private float coolSpeed = 18f;

        [Header("UI")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Text statusText;
        [SerializeField] private Text resultText;
        [SerializeField] private GearOption[] gearOptions;
        [SerializeField] private RectTransform phaseIndicator;
        [SerializeField] private RectTransform rotationIndicator;
        [SerializeField] private Button startButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button quitButton;

        private readonly List<Vector3> redPoints = new List<Vector3>(320);
        private readonly List<Vector3> bluePoints = new List<Vector3>(320);
        private readonly List<SparkPoint> sparks = new List<SparkPoint>(32);

        private GameState state;
        private float targetPhase;
        private float currentPhase;
        private float targetRotation;
        private float currentRotation;
        private bool isDragging;
        private bool dragPhase;
        private float lastPointerY;

        private void Awake()
        {
            EnsureSceneReady();
            ConnectButtons();
            ResetGame();
            ShowReady();
        }

        private void Update()
        {
            if (state != GameState.Playing)
            {
                return;
            }

            ReadDragInput();
            UpdatePlayerMotion();
            UpdateCurves();
            UpdateSparks();
            UpdateWeldNodes();
            UpdateTrackIndicators();
        }

        public void StartGame()
        {
            state = GameState.Playing;
            startPanel.SetActive(false);
            resultPanel.SetActive(false);
            statusText.text = "Find sparks on all weld nodes";
        }

        public void Retry()
        {
            ResetGame();
            StartGame();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SelectGear(int index)
        {
            if (index < 0 || index >= gearOptions.Length)
            {
                return;
            }

            playerFrequencyX = gearOptions[index].frequencyX;
            playerFrequencyY = gearOptions[index].frequencyY;
            RefreshGearButtons(index);
        }

        private void ConnectButtons()
        {
            startButton.onClick.RemoveAllListeners();
            retryButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGame);
            retryButton.onClick.AddListener(Retry);
            quitButton.onClick.AddListener(Quit);

            for (int i = 0; i < gearOptions.Length; i++)
            {
                int index = i;
                gearOptions[i].button.onClick.RemoveAllListeners();
                gearOptions[i].button.onClick.AddListener(() => SelectGear(index));
                gearOptions[i].label.text = $"{gearOptions[i].frequencyX}:{gearOptions[i].frequencyY}";
            }
        }

        private void ResetGame()
        {
            state = GameState.Ready;
            targetPhase = startPhase;
            currentPhase = startPhase;
            targetRotation = 0f;
            currentRotation = 0f;
            playerFrequencyX = 1;
            playerFrequencyY = 2;

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].progress = 0f;
                nodes[i].fixedDone = false;
                SetNodeVisual(nodes[i], i);
            }

            RefreshGearButtons(1);
            HideAllSparks();
            UpdateCurves();
            UpdateTrackIndicators();
        }

        private void ShowReady()
        {
            startPanel.SetActive(true);
            resultPanel.SetActive(false);
            statusText.text = "Resonance Welding";
        }

        private void ReadDragInput()
        {
            Pointer pointer = Pointer.current;
            if (pointer == null)
            {
                return;
            }

            bool pressedThisFrame = pointer.press.wasPressedThisFrame;
            bool releasedThisFrame = pointer.press.wasReleasedThisFrame;
            bool isPressed = pointer.press.isPressed;
            Vector2 pointerPosition = pointer.position.ReadValue();

            if (pressedThisFrame)
            {
                if (pointerPosition.y < 120f)
                {
                    return;
                }

                isDragging = true;
                dragPhase = pointerPosition.x < Screen.width * 0.5f;
                lastPointerY = pointerPosition.y;
            }

            if (releasedThisFrame)
            {
                isDragging = false;
            }

            if (!isDragging || !isPressed)
            {
                return;
            }

            float pointerY = pointerPosition.y;
            float deltaY = pointerY - lastPointerY;
            if (dragPhase)
            {
                targetPhase -= deltaY * dragSensitivity;
            }
            else
            {
                targetRotation -= deltaY * dragSensitivity;
            }

            lastPointerY = pointerY;
        }

        private void UpdatePlayerMotion()
        {
            float lerp = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
            currentPhase = Mathf.Lerp(currentPhase, targetPhase, lerp);
            currentRotation = Mathf.Lerp(currentRotation, targetRotation, lerp);
        }

        private void UpdateCurves()
        {
            Poc2LissajousMath.FillPoints(redPoints, targetFrequencyX, targetFrequencyY, 0f, 0f, amplitude, curveResolution);
            Poc2LissajousMath.FillPoints(bluePoints, playerFrequencyX, playerFrequencyY, currentPhase, currentRotation, amplitude, curveResolution);

            ApplyLine(redCurve, redPoints);
            ApplyLine(blueCurve, bluePoints);
        }

        private void ApplyLine(LineRenderer line, List<Vector3> points)
        {
            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());
        }

        private void UpdateSparks()
        {
            Poc2LissajousMath.FindSparks(redPoints, bluePoints, sparks, sparkCellSize, sparkMergeDistance);

            for (int i = 0; i < sparkViews.Length; i++)
            {
                bool visible = i < sparks.Count;
                sparkViews[i].gameObject.SetActive(visible);
                if (visible)
                {
                    sparkViews[i].localPosition = sparks[i].Position;
                }
            }
        }

        private void HideAllSparks()
        {
            for (int i = 0; i < sparkViews.Length; i++)
            {
                sparkViews[i].gameObject.SetActive(false);
            }
        }

        private void UpdateWeldNodes()
        {
            bool allFixed = true;
            bool anyWelding = false;

            for (int i = 0; i < nodes.Length; i++)
            {
                WeldNode node = nodes[i];
                node.view.localPosition = node.normalizedPosition * amplitude;

                if (!node.fixedDone)
                {
                    allFixed = false;
                    bool hit = IsSparkNear(node.view.localPosition, weldRadius);
                    if (hit)
                    {
                        node.progress += weldSpeed * Time.deltaTime;
                        anyWelding = true;
                    }
                    else
                    {
                        node.progress -= coolSpeed * Time.deltaTime;
                    }

                    node.progress = Mathf.Clamp(node.progress, 0f, 100f);
                    if (node.progress >= 100f)
                    {
                        node.fixedDone = true;
                    }
                }

                SetNodeVisual(node, i);
            }

            statusText.text = anyWelding ? "Welding..." : "Tune phase and rotation";

            if (allFixed)
            {
                CompleteGame();
            }
        }

        private bool IsSparkNear(Vector2 point, float radius)
        {
            float radiusSqr = radius * radius;
            for (int i = 0; i < sparks.Count; i++)
            {
                if ((sparks[i].Position - point).sqrMagnitude <= radiusSqr)
                {
                    return true;
                }
            }

            return false;
        }

        private void SetNodeVisual(WeldNode node, int index)
        {
            float size = node.fixedDone ? 0.28f : Mathf.Lerp(0.18f, 0.32f, node.progress / 100f);
            node.view.localScale = new Vector3(size, size, size);
            node.label.text = node.fixedDone ? "OK" : $"T{index + 1}";

            Color color = node.fixedDone
                ? new Color(0f, 1f, 0.8f, 1f)
                : Color.Lerp(new Color(0.12f, 0.12f, 0.14f, 1f), new Color(0f, 0.8f, 1f, 1f), node.progress / 100f);

            node.renderer.material.color = color;
        }

        private void CompleteGame()
        {
            state = GameState.Success;
            playerFrequencyX = targetFrequencyX;
            playerFrequencyY = targetFrequencyY;
            targetPhase = 0f;
            targetRotation = 0f;
            currentPhase = 0f;
            currentRotation = 0f;

            UpdateCurves();
            HideAllSparks();
            resultText.text = "SYSTEM SYNCHRONIZED";
            resultPanel.SetActive(true);
            statusText.text = "All weld nodes complete";
        }

        private void RefreshGearButtons(int selectedIndex)
        {
            for (int i = 0; i < gearOptions.Length; i++)
            {
                ColorBlock colors = gearOptions[i].button.colors;
                colors.normalColor = i == selectedIndex ? new Color(0f, 0.65f, 1f, 1f) : new Color(0.16f, 0.16f, 0.2f, 1f);
                colors.highlightedColor = i == selectedIndex ? new Color(0.1f, 0.8f, 1f, 1f) : new Color(0.25f, 0.25f, 0.3f, 1f);
                colors.pressedColor = new Color(0.05f, 0.5f, 0.75f, 1f);
                gearOptions[i].button.colors = colors;
            }
        }

        private void UpdateTrackIndicators()
        {
            MoveIndicator(phaseIndicator, targetPhase);
            MoveIndicator(rotationIndicator, targetRotation);
        }

        private void MoveIndicator(RectTransform indicator, float value)
        {
            Vector2 anchored = indicator.anchoredPosition;
            anchored.y = Mathf.Sin(value) * 130f;
            indicator.anchoredPosition = anchored;
        }

        private void EnsureSceneReady()
        {
            if (redCurve != null)
            {
                return;
            }

            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.02f, 0.02f, 0.03f, 1f);
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);

            curveRoot = new GameObject("CurveRoot").transform;
            curveRoot.SetParent(transform, false);

            redCurve = CreateRuntimeCurve("Red_TargetCurve", new Color(1f, 0.18f, 0.38f, 0.75f), 0.055f);
            blueCurve = CreateRuntimeCurve("Blue_PlayerCurve", new Color(0f, 0.78f, 1f, 0.85f), 0.05f);

            Material sparkMaterial = CreateRuntimeMaterial(Color.white);
            Transform sparkRoot = new GameObject("Sparks").transform;
            sparkRoot.SetParent(curveRoot, false);
            sparkViews = new Transform[28];
            for (int i = 0; i < sparkViews.Length; i++)
            {
                GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spark.name = $"Spark_{i + 1:00}";
                spark.transform.SetParent(sparkRoot, false);
                spark.transform.localScale = Vector3.one * 0.08f;
                spark.GetComponent<Renderer>().material = sparkMaterial;
                Destroy(spark.GetComponent<Collider>());
                spark.SetActive(false);
                sparkViews[i] = spark.transform;
            }

            nodes = CreateRuntimeNodes();
            CreateRuntimeUi();
        }

        private LineRenderer CreateRuntimeCurve(string curveName, Color color, float width)
        {
            GameObject curveObject = new GameObject(curveName);
            curveObject.transform.SetParent(curveRoot, false);
            LineRenderer line = curveObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.material = CreateRuntimeMaterial(color);
            line.startWidth = width;
            line.endWidth = width;
            line.numCapVertices = 4;
            line.numCornerVertices = 4;
            return line;
        }

        private WeldNode[] CreateRuntimeNodes()
        {
            Vector2[] positions =
            {
                new Vector2(-0.6f, -0.5f),
                new Vector2(0.6f, 0.5f),
                new Vector2(0f, 0.8f)
            };

            WeldNode[] createdNodes = new WeldNode[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject nodeObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                nodeObject.name = $"WeldNode_T{i + 1}";
                nodeObject.transform.SetParent(curveRoot, false);
                nodeObject.transform.localPosition = positions[i] * amplitude;
                Destroy(nodeObject.GetComponent<Collider>());

                Renderer nodeRenderer = nodeObject.GetComponent<Renderer>();
                nodeRenderer.material = CreateRuntimeMaterial(new Color(0.12f, 0.12f, 0.14f, 1f));

                Text label = CreateWorldLabel(nodeObject.transform, $"T{i + 1}");
                createdNodes[i] = new WeldNode
                {
                    normalizedPosition = positions[i],
                    view = nodeObject.transform,
                    renderer = nodeRenderer,
                    label = label
                };
            }

            return createdNodes;
        }

        private Text CreateWorldLabel(Transform parent, string textValue)
        {
            GameObject canvasObject = new GameObject("LabelCanvas");
            canvasObject.transform.SetParent(parent, false);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(1f, 0.4f);
            canvasRect.localScale = Vector3.one * 0.01f;

            Text text = CreateText("Label", canvasObject.transform, textValue, 24, TextAnchor.MiddleCenter);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return text;
        }

        private void CreateRuntimeUi()
        {
            Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<InputSystemUIInputModule>();
            }

            statusText = CreateText("StatusText", canvas.transform, "Resonance Welding", 26, TextAnchor.MiddleCenter);
            SetRect(statusText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -46f), new Vector2(800f, 52f));

            phaseIndicator = CreateSideTrack(canvas.transform, "PhaseTrack", true);
            rotationIndicator = CreateSideTrack(canvas.transform, "RotationTrack", false);

            GameObject gearPanel = CreatePanel("GearPanel", canvas.transform, new Color(0f, 0f, 0f, 0f));
            SetRect(gearPanel.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 42f), new Vector2(540f, 64f));
            HorizontalLayoutGroup layout = gearPanel.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 10f;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            gearOptions = CreateGearOptions(gearPanel.transform);

            startPanel = CreateOverlayPanel(canvas.transform, "StartPanel");
            Text title = CreateText("Title", startPanel.transform, "RESONANCE WELDING", 34, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 74f), new Vector2(720f, 70f));
            Text body = CreateText("Body", startPanel.transform, "Drag left side for phase, right side for rotation.", 20, TextAnchor.MiddleCenter);
            SetRect(body.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 22f), new Vector2(720f, 40f));
            startButton = CreateButton("StartButton", startPanel.transform, "START", new Vector2(0f, -58f), new Vector2(190f, 54f));

            resultPanel = CreateOverlayPanel(canvas.transform, "ResultPanel");
            resultText = CreateText("ResultText", resultPanel.transform, "SYSTEM SYNCHRONIZED", 32, TextAnchor.MiddleCenter);
            SetRect(resultText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 54f), new Vector2(720f, 70f));
            retryButton = CreateButton("RetryButton", resultPanel.transform, "RETRY", new Vector2(-100f, -40f), new Vector2(170f, 52f));
            quitButton = CreateButton("QuitButton", resultPanel.transform, "QUIT", new Vector2(100f, -40f), new Vector2(170f, 52f));
        }

        private GearOption[] CreateGearOptions(Transform parent)
        {
            Vector2Int[] values =
            {
                new Vector2Int(1, 1),
                new Vector2Int(1, 2),
                new Vector2Int(2, 3),
                new Vector2Int(3, 2),
                new Vector2Int(3, 4)
            };

            GearOption[] options = new GearOption[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                Button button = CreateButton($"Gear_{values[i].x}_{values[i].y}", parent, $"{values[i].x}:{values[i].y}", Vector2.zero, new Vector2(92f, 52f));
                LayoutElement layout = button.gameObject.AddComponent<LayoutElement>();
                layout.preferredWidth = 92f;
                layout.preferredHeight = 52f;
                options[i] = new GearOption
                {
                    frequencyX = values[i].x,
                    frequencyY = values[i].y,
                    button = button,
                    label = button.GetComponentInChildren<Text>()
                };
            }

            return options;
        }

        private RectTransform CreateSideTrack(Transform parent, string trackName, bool left)
        {
            GameObject track = CreatePanel(trackName, parent, new Color(1f, 1f, 1f, 0.035f));
            Vector2 anchor = left ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f);
            SetRect(track.GetComponent<RectTransform>(), anchor, anchor, left ? new Vector2(26f, 0f) : new Vector2(-26f, 0f), new Vector2(42f, 360f));

            GameObject indicator = CreatePanel("Indicator", track.transform, new Color(0f, 0.78f, 1f, 0.75f));
            RectTransform indicatorRect = indicator.GetComponent<RectTransform>();
            SetRect(indicatorRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(22f, 58f));
            return indicatorRect;
        }

        private GameObject CreateOverlayPanel(Transform parent, string panelName)
        {
            GameObject panel = CreatePanel(panelName, parent, new Color(0f, 0f, 0f, 0.72f));
            SetRect(panel.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return panel;
        }

        private GameObject CreatePanel(string panelName, Transform parent, Color color)
        {
            GameObject panel = new GameObject(panelName);
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.localScale = Vector3.one;
            Image image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private Button CreateButton(string buttonName, Transform parent, string textValue, Vector2 position, Vector2 size)
        {
            GameObject buttonObject = CreatePanel(buttonName, parent, new Color(0.16f, 0.16f, 0.2f, 1f));
            SetRect(buttonObject.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, size);

            Button button = buttonObject.AddComponent<Button>();
            Text label = CreateText("Text", buttonObject.transform, textValue, 20, TextAnchor.MiddleCenter);
            SetRect(label.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return button;
        }

        private Text CreateText(string objectName, Transform parent, string textValue, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent, false);
            Text text = textObject.AddComponent<Text>();
            text.text = textValue;
            text.font = GetDefaultFont();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        private Font GetDefaultFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        private Material CreateRuntimeMaterial(Color color)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            Material material = new Material(shader);
            material.color = color;
            return material;
        }

        private void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
        }
    }
}
