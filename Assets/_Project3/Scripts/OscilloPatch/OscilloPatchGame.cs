using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Project3.OscilloPatch
{
    [ExecuteAlways]
    public sealed class OscilloPatchGame : MonoBehaviour
    {
        private const int SlotCount = 3;
        private const float ScopeScale = 0.022f;
        private const float PatchOpenY = 0f;
        private const float PatchClosedY = -200f;

        private readonly SignalCompiler signalCompiler = new SignalCompiler();
        private readonly LissajousPointMaker pointMaker = new LissajousPointMaker();
        private readonly ChannelCircuit xChannel = new ChannelCircuit(SlotCount);
        private readonly ChannelCircuit yChannel = new ChannelCircuit(SlotCount);

        private Mission mission;
        private List<SignalPart> inventory;
        private SignalPart selectedPart;
        private int selectedInventoryIndex = -1;

        private TMP_FontAsset font;
        private LineRenderer lissajousLine;
        private LineRenderer electronDot;
        private TMP_Text[] conditionTexts;
        private Image[] conditionLights;
        private TMP_Text statusText;
        private TMP_Text guideText;
        private TMP_Text patchToggleText;
        private Button repairButton;
        private RectTransform patchPanel;

        private readonly List<Button> inventoryButtons = new List<Button>();
        private readonly List<Button> xSlotButtons = new List<Button>();
        private readonly List<Button> ySlotButtons = new List<Button>();

        private InputActionAsset uiInputActions;
        private float electronTime;
        private bool isPatchOpen = true;
        private bool isRepaired;

        private void Awake()
        {
            BuildGame();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                BuildGame();
            }
        }

        public void RebuildSceneObjectsForEditor()
        {
            BuildGame();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            RefreshScopeOnly();
            AnimatePatchPanel();
        }

        private void BuildGame()
        {
            ClearGeneratedObjects();
            font = Resources.Load<TMP_FontAsset>("Fonts & Materials/KoPubWorld Batang Bold 2");
            mission = new Mission(
                "\uc624\uc2e4\ub85c \ud328\uce58 : \uc2e0\ud638 \uc9c1\uc870\uae30",
                "\uace0\uc7a5: \uc11c\ubcf4 \ubaa8\ud130 \ub3d9\uae30 \ubd88\ub7c9. \uc548\uc815\uc801\uc778 \ub9ac\uc0ac\uc8fc \uc81c\uc5b4 \uc2e0\ud638\ub97c \ubcf5\uad6c\ud558\uc138\uc694.",
                new[] { new Vector2(90f, -90f), new Vector2(-90f, 90f) });

            inventory = MakeInventory();
            PrepareCamera();
            PrepareEventSystem();
            CreateOscilloscope();
            CreateUi();
            RefreshGame();
        }

        private void ClearGeneratedObjects()
        {
            DestroyInputActions();

            for (int index = transform.childCount - 1; index >= 0; index--)
            {
                Transform child = transform.GetChild(index);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            inventoryButtons.Clear();
            xSlotButtons.Clear();
            ySlotButtons.Clear();
            selectedPart = null;
            selectedInventoryIndex = -1;
            electronTime = 0f;
            isPatchOpen = true;
            isRepaired = false;
        }

        private List<SignalPart> MakeInventory()
        {
            return new List<SignalPart>
            {
                new SignalPart("OSC_1", "OSC", "x1", new Color(0.85f, 0.85f, 0.85f), frequencyAdd: 1),
                new SignalPart("OSC_2", "OSC", "x2", new Color(0.85f, 0.85f, 0.85f), frequencyAdd: 2),
                new SignalPart("AMP_A", "\uc99d\ud3ed", "+50", new Color(1f, 0.75f, 0.15f), amplitudeAdd: 50f),
                new SignalPart("AMP_B", "\uc99d\ud3ed", "+50", new Color(1f, 0.75f, 0.15f), amplitudeAdd: 50f),
                new SignalPart("ATT", "\uac10\uc1e0", "-40", new Color(1f, 0.45f, 0.1f), amplitudeAdd: -40f),
                new SignalPart("PH_90", "\ucf54\uc77c", "+90", new Color(0.2f, 0.85f, 1f), phaseDegreesAdd: 90f),
                new SignalPart("PH_45", "\ucf54\uc77c", "+45", new Color(0.15f, 0.45f, 1f), phaseDegreesAdd: 45f),
                new SignalPart("INV", "\ubc18\uc804", "180", new Color(0.85f, 0.25f, 1f), phaseDegreesAdd: 180f),
            };
        }

        private void PrepareCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                camera = new GameObject("Main Camera").AddComponent<Camera>();
                camera.tag = "MainCamera";
                camera.gameObject.AddComponent<AudioListener>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 5.4f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = new Color(0.02f, 0.02f, 0.035f);
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

        private void PrepareEventSystem()
        {
            EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            }

            StandaloneInputModule oldInputModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (oldInputModule != null)
            {
                DestroyComponent(oldInputModule);
            }

            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            if (Application.isPlaying)
            {
                AssignUiInputActions(inputModule);
            }
        }

        private void AssignUiInputActions(InputSystemUIInputModule inputModule)
        {
            DestroyInputActions();

            uiInputActions = ScriptableObject.CreateInstance<InputActionAsset>();
            InputActionMap map = new InputActionMap("UI");

            InputAction point = map.AddAction("Point", InputActionType.PassThrough, "<Pointer>/position");
            InputAction leftClick = map.AddAction("LeftClick", InputActionType.PassThrough, "<Pointer>/press");
            InputAction scroll = map.AddAction("ScrollWheel", InputActionType.PassThrough, "<Pointer>/scroll");
            InputAction submit = map.AddAction("Submit", InputActionType.Button, "<Keyboard>/enter");
            InputAction cancel = map.AddAction("Cancel", InputActionType.Button, "<Keyboard>/escape");
            InputAction move = map.AddAction("Move", InputActionType.PassThrough);
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            uiInputActions.AddActionMap(map);
            inputModule.actionsAsset = uiInputActions;
            inputModule.point = InputActionReference.Create(point);
            inputModule.leftClick = InputActionReference.Create(leftClick);
            inputModule.scrollWheel = InputActionReference.Create(scroll);
            inputModule.submit = InputActionReference.Create(submit);
            inputModule.cancel = InputActionReference.Create(cancel);
            inputModule.move = InputActionReference.Create(move);
            uiInputActions.Enable();
        }

        private void DestroyInputActions()
        {
            if (uiInputActions == null)
            {
                return;
            }

            uiInputActions.Disable();
            DestroyComponent(uiInputActions);
            uiInputActions = null;
        }

        private void DestroyComponent(Object target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(target);
            }
            else
            {
                DestroyImmediate(target);
            }
        }

        private void CreateOscilloscope()
        {
            CreateGrid();
            CreateDangerBox();
            CreateTargetNodes();
            lissajousLine = CreateLine("Lissajous Line", 0.035f, new Color(0f, 0.75f, 1f));
            electronDot = CreateLine("Electron Dot", 0.12f, Color.white);
            electronDot.loop = true;
        }

        private void CreateGrid()
        {
            for (int index = -4; index <= 4; index++)
            {
                LineRenderer vertical = CreateLine("Grid V", 0.01f, new Color(0f, 0.8f, 0.7f, 0.12f));
                vertical.positionCount = 2;
                vertical.SetPosition(0, new Vector3(index, -3.4f, 0f));
                vertical.SetPosition(1, new Vector3(index, 3.4f, 0f));

                LineRenderer horizontal = CreateLine("Grid H", 0.01f, new Color(0f, 0.8f, 0.7f, 0.12f));
                horizontal.positionCount = 2;
                horizontal.SetPosition(0, new Vector3(-4.4f, index * 0.75f, 0f));
                horizontal.SetPosition(1, new Vector3(4.4f, index * 0.75f, 0f));
            }
        }

        private void CreateDangerBox()
        {
            LineRenderer box = CreateLine("Overvoltage Limit", 0.025f, new Color(1f, 0.15f, 0.18f, 0.42f));
            float limit = 150f * ScopeScale;
            SetBoxPoints(box, limit, limit);
        }

        private void CreateTargetNodes()
        {
            foreach (Vector2 targetNode in mission.TargetNodes)
            {
                LineRenderer ring = CreateLine("Target Contact", 0.035f, new Color(0f, 1f, 0.8f, 0.75f));
                ring.loop = true;
                ring.positionCount = 32;

                Vector2 center = targetNode * ScopeScale;
                for (int index = 0; index < ring.positionCount; index++)
                {
                    float angle = index / (float)ring.positionCount * Mathf.PI * 2f;
                    Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.18f;
                    ring.SetPosition(index, new Vector3(point.x, point.y, 0f));
                }
            }
        }

        private LineRenderer CreateLine(string objectName, float width, Color color)
        {
            GameObject lineObject = new GameObject(objectName);
            lineObject.transform.SetParent(transform, false);
            LineRenderer line = lineObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = color;
            line.endColor = color;
            line.startWidth = width;
            line.endWidth = width;
            line.useWorldSpace = true;
            line.numCapVertices = 5;
            return line;
        }

        private void SetBoxPoints(LineRenderer line, float halfWidth, float halfHeight)
        {
            line.loop = true;
            line.positionCount = 4;
            line.SetPosition(0, new Vector3(-halfWidth, -halfHeight, 0f));
            line.SetPosition(1, new Vector3(-halfWidth, halfHeight, 0f));
            line.SetPosition(2, new Vector3(halfWidth, halfHeight, 0f));
            line.SetPosition(3, new Vector3(halfWidth, -halfHeight, 0f));
        }

        private void CreateUi()
        {
            Canvas canvas = CreateCanvas();
            GameObject topPanel = CreatePanel("Mission Panel", canvas.transform, AnchorTop(), new Color(0.035f, 0.05f, 0.065f, 0.96f));
            CreateMissionUi(topPanel.transform);

            GameObject bottomPanel = CreatePanel("Patch Panel", canvas.transform, AnchorBottom(), new Color(0.035f, 0.035f, 0.055f, 0.98f));
            patchPanel = bottomPanel.GetComponent<RectTransform>();
            CreatePatchUi(bottomPanel.transform);
        }

        private Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Oscillo Patch UI", typeof(RectTransform));
            canvasObject.transform.SetParent(transform, false);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private RectTransformPreset AnchorTop()
        {
            return new RectTransformPreset(new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 144f));
        }

        private RectTransformPreset AnchorBottom()
        {
            return new RectTransformPreset(new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), Vector2.zero, new Vector2(0f, 240f));
        }

        private GameObject CreatePanel(string objectName, Transform parent, RectTransformPreset preset, Color color)
        {
            GameObject panel = new GameObject(objectName, typeof(RectTransform));
            panel.transform.SetParent(parent, false);
            Image image = panel.AddComponent<Image>();
            image.color = color;
            ApplyPreset(panel.GetComponent<RectTransform>(), preset);
            return panel;
        }

        private void CreateMissionUi(Transform parent)
        {
            TMP_Text title = CreateText("Title", parent, mission.Title, 26, Color.cyan, TextAlignmentOptions.TopLeft);
            SetRect(title.rectTransform, new Vector2(24f, -14f), new Vector2(620f, 34f), new Vector2(0f, 1f));

            TMP_Text description = CreateText("Description", parent, mission.Description, 15, new Color(0.72f, 0.76f, 0.78f), TextAlignmentOptions.TopLeft);
            SetRect(description.rectTransform, new Vector2(24f, -50f), new Vector2(780f, 28f), new Vector2(0f, 1f));

            conditionTexts = new TMP_Text[mission.Conditions.Count];
            conditionLights = new Image[mission.Conditions.Count];

            for (int index = 0; index < mission.Conditions.Count; index++)
            {
                float x = 36f + index * 360f;
                Image light = CreateImage("Condition Light", parent, Color.red);
                SetRect(light.rectTransform, new Vector2(x, -102f), new Vector2(14f, 14f), new Vector2(0f, 1f));
                conditionLights[index] = light;

                TMP_Text label = CreateText("Condition", parent, mission.Conditions[index].Description, 14, Color.gray, TextAlignmentOptions.MidlineLeft);
                SetRect(label.rectTransform, new Vector2(x + 24f, -110f), new Vector2(310f, 30f), new Vector2(0f, 1f));
                conditionTexts[index] = label;
            }

            repairButton = CreateButton("Repair Button", parent, "\uc218\ub9ac \uc644\ub8cc", new Color(0.12f, 0.13f, 0.15f), Color.gray, 18);
            SetRect(repairButton.GetComponent<RectTransform>(), new Vector2(-130f, -42f), new Vector2(210f, 54f), new Vector2(1f, 1f));
            repairButton.onClick.AddListener(Repair);
        }

        private void CreatePatchUi(Transform parent)
        {
            Button toggleButton = CreateButton("Patch Toggle", parent, "\ubd80\ud488\ucc3d \uc811\uae30 \u25be", new Color(0f, 0.55f, 0.5f), Color.black, 15);
            SetRect(toggleButton.GetComponent<RectTransform>(), new Vector2(-214f, 34f), new Vector2(210f, 34f), new Vector2(1f, 1f));
            toggleButton.onClick.AddListener(TogglePatchPanel);
            patchToggleText = toggleButton.GetComponentInChildren<TMP_Text>();

            TMP_Text inventoryTitle = CreateText("Inventory Title", parent, "\ubd80\ud488 \ubcf4\uad00\ud568", 15, Color.gray, TextAlignmentOptions.TopLeft);
            SetRect(inventoryTitle.rectTransform, new Vector2(24f, -18f), new Vector2(220f, 24f), new Vector2(0f, 1f));

            guideText = CreateText("Guide", parent, "\ubd80\ud488\uc744 \ub204\ub974\uace0 X/Y \uc2ac\ub86f\uc744 \ub204\ub974\uc138\uc694. \ubc1c\uc9c4\uae30\ub97c \uc591\ucabd\uc5d0 \uaf42\uc544\uc57c \uc2e0\ud638\uac00 \ub098\uc635\ub2c8\ub2e4.", 14, new Color(0.6f, 0.68f, 0.72f), TextAlignmentOptions.TopLeft);
            SetRect(guideText.rectTransform, new Vector2(24f, -198f), new Vector2(640f, 24f), new Vector2(0f, 1f));

            for (int index = 0; index < inventory.Count; index++)
            {
                int capturedIndex = index;
                Button button = CreateButton("Inventory Part", parent, "", new Color(0.09f, 0.1f, 0.14f), Color.white, 13);
                SetRect(button.GetComponent<RectTransform>(), new Vector2(24f + index * 94f, -66f), new Vector2(84f, 62f), new Vector2(0f, 1f));
                button.onClick.AddListener(() => SelectInventory(capturedIndex));
                inventoryButtons.Add(button);
            }

            CreateChannelUi(parent, "X", xSlotButtons, 1120f, -54f, true);
            CreateChannelUi(parent, "Y", ySlotButtons, 1120f, -140f, false);

            statusText = CreateText("Status", parent, "\uc2e0\ud638 \uc5c6\uc74c / X\uc640 Y\uc5d0 \ubc1c\uc9c4\uae30\ub97c \uaf42\uc73c\uc138\uc694", 18, new Color(0.6f, 0.68f, 0.72f), TextAlignmentOptions.MidlineRight);
            SetRect(statusText.rectTransform, new Vector2(-420f, -28f), new Vector2(380f, 40f), new Vector2(1f, 1f));
        }

        private void CreateChannelUi(Transform parent, string channelName, List<Button> buttons, float x, float y, bool isX)
        {
            Color channelColor = isX ? new Color(0f, 0.75f, 1f) : new Color(1f, 0.25f, 0.45f);
            TMP_Text label = CreateText(channelName + " Label", parent, channelName, 32, channelColor, TextAlignmentOptions.Center);
            SetRect(label.rectTransform, new Vector2(x, y), new Vector2(48f, 62f), new Vector2(0f, 1f));

            for (int index = 0; index < SlotCount; index++)
            {
                int capturedIndex = index;
                Button button = CreateButton(channelName + " Slot", parent, "\ube44\uc5b4 \uc788\uc74c", new Color(0.045f, 0.05f, 0.07f), Color.gray, 13);
                SetRect(button.GetComponent<RectTransform>(), new Vector2(x + 58f + index * 116f, y), new Vector2(104f, 62f), new Vector2(0f, 1f));
                button.onClick.AddListener(() => ClickSlot(isX, capturedIndex));
                buttons.Add(button);
            }
        }

        private TMP_Text CreateText(string objectName, Transform parent, string text, int size, Color color, TextAlignmentOptions alignment)
        {
            GameObject textObject = new GameObject(objectName, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);
            TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();
            label.font = font;
            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.alignment = alignment;
            label.raycastTarget = false;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            return label;
        }

        private Image CreateImage(string objectName, Transform parent, Color color)
        {
            GameObject imageObject = new GameObject(objectName, typeof(RectTransform));
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private Button CreateButton(string objectName, Transform parent, string text, Color background, Color textColor, int textSize)
        {
            GameObject buttonObject = new GameObject(objectName, typeof(RectTransform));
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.AddComponent<Image>();
            image.color = background;

            Button button = buttonObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.highlightedColor = background + new Color(0.08f, 0.08f, 0.08f, 0f);
            colors.pressedColor = background * 0.75f;
            button.colors = colors;

            TMP_Text label = CreateText("Label", buttonObject.transform, text, textSize, textColor, TextAlignmentOptions.Center);
            ApplyPreset(label.rectTransform, RectTransformPreset.Fill());
            return button;
        }

        private void TogglePatchPanel()
        {
            isPatchOpen = !isPatchOpen;
            if (patchToggleText != null)
            {
                patchToggleText.text = isPatchOpen ? "\ubd80\ud488\ucc3d \uc811\uae30 \u25be" : "\ubd80\ud488\ucc3d \uc5f4\uae30 \u25b4";
            }
        }

        private void AnimatePatchPanel()
        {
            if (patchPanel == null)
            {
                return;
            }

            float targetY = isPatchOpen ? PatchOpenY : PatchClosedY;
            Vector2 position = patchPanel.anchoredPosition;
            position.y = Mathf.Lerp(position.y, targetY, Time.deltaTime * 14f);
            if (Mathf.Abs(position.y - targetY) < 0.5f)
            {
                position.y = targetY;
            }

            patchPanel.anchoredPosition = position;
        }

        private void SelectInventory(int index)
        {
            if (isRepaired || inventory[index] == null)
            {
                return;
            }

            if (selectedInventoryIndex == index)
            {
                selectedPart = null;
                selectedInventoryIndex = -1;
                SetGuide("\ubd80\ud488 \uc120\ud0dd\uc744 \ud574\uc81c\ud588\uc2b5\ub2c8\ub2e4.");
            }
            else
            {
                selectedPart = inventory[index];
                selectedInventoryIndex = index;
                SetGuide($"{selectedPart.Name} {selectedPart.ValueText} \uc120\ud0dd\ub428. X \ub610\ub294 Y \uc2ac\ub86f\uc5d0 \uaf42\uc73c\uc138\uc694.");
            }

            RefreshGame();
        }

        private void ClickSlot(bool isX, int slotIndex)
        {
            if (isRepaired)
            {
                return;
            }

            ChannelCircuit channel = isX ? xChannel : yChannel;
            SignalPart currentPart = channel.GetPart(slotIndex);

            if (currentPart != null)
            {
                ReturnToInventory(currentPart);
                channel.SetPart(slotIndex, null);
                SetGuide($"{(isX ? "X" : "Y")} \ucc44\ub110\uc5d0\uc11c {currentPart.Name}\uc744 \ube7c\ub0c8\uc2b5\ub2c8\ub2e4.");
            }
            else if (selectedPart != null)
            {
                channel.SetPart(slotIndex, selectedPart);
                inventory[selectedInventoryIndex] = null;
                SetGuide($"{selectedPart.Name} {selectedPart.ValueText}\uc744 {(isX ? "X" : "Y")} \ucc44\ub110\uc5d0 \uc7a5\ucc29\ud588\uc2b5\ub2c8\ub2e4.");
                selectedPart = null;
                selectedInventoryIndex = -1;
            }
            else
            {
                SetGuide("\uba3c\uc800 \ubd80\ud488 \ubcf4\uad00\ud568\uc5d0\uc11c \ubd80\ud488\uc744 \uc120\ud0dd\ud558\uc138\uc694.");
            }

            RefreshGame();
        }

        private void SetGuide(string message)
        {
            if (guideText != null)
            {
                guideText.text = message;
            }
        }

        private void ReturnToInventory(SignalPart part)
        {
            int emptyIndex = inventory.IndexOf(null);
            if (emptyIndex >= 0)
            {
                inventory[emptyIndex] = part;
            }
            else
            {
                inventory.Add(part);
            }
        }

        private void RefreshGame()
        {
            SignalPair signals = signalCompiler.Compile(xChannel, yChannel);
            List<Vector2> points = pointMaker.MakePoints(signals);

            RefreshInventoryButtons();
            RefreshSlotButtons();
            RefreshConditions(signals, points);
            DrawSignal(points);
            DrawElectron(signals, points);
        }

        private void RefreshScopeOnly()
        {
            SignalPair signals = signalCompiler.Compile(xChannel, yChannel);
            List<Vector2> points = pointMaker.MakePoints(signals);
            DrawElectron(signals, points);
        }

        private void RefreshInventoryButtons()
        {
            for (int index = 0; index < inventoryButtons.Count; index++)
            {
                SignalPart part = inventory[index];
                Button button = inventoryButtons[index];
                TMP_Text label = button.GetComponentInChildren<TMP_Text>();

                if (part == null)
                {
                    label.text = "";
                    button.image.color = new Color(0.035f, 0.038f, 0.05f, 0.55f);
                    button.interactable = false;
                    continue;
                }

                label.text = part.Name + "\n" + part.ValueText;
                label.color = part.Color;
                button.interactable = true;
                button.image.color = selectedInventoryIndex == index
                    ? new Color(0f, 0.24f, 0.24f)
                    : new Color(0.09f, 0.1f, 0.14f);
            }
        }

        private void RefreshSlotButtons()
        {
            RefreshChannelButtons(xChannel, xSlotButtons);
            RefreshChannelButtons(yChannel, ySlotButtons);
        }

        private void RefreshChannelButtons(ChannelCircuit channel, List<Button> buttons)
        {
            for (int index = 0; index < buttons.Count; index++)
            {
                SignalPart part = channel.GetPart(index);
                TMP_Text label = buttons[index].GetComponentInChildren<TMP_Text>();

                if (part == null)
                {
                    label.text = "\ube44\uc5b4 \uc788\uc74c";
                    label.color = Color.gray;
                    buttons[index].image.color = new Color(0.045f, 0.05f, 0.07f);
                }
                else
                {
                    label.text = part.Name + "\n" + part.ValueText;
                    label.color = part.Color;
                    buttons[index].image.color = new Color(0.05f, 0.1f, 0.13f);
                }
            }
        }

        private void RefreshConditions(SignalPair signals, IReadOnlyList<Vector2> points)
        {
            bool allMet = true;

            for (int index = 0; index < mission.Conditions.Count; index++)
            {
                bool isMet = mission.Conditions[index].IsMet(signals, points);
                allMet &= isMet;
                conditionLights[index].color = isMet ? Color.cyan : new Color(1f, 0.18f, 0.18f);
                conditionTexts[index].color = isMet ? Color.cyan : Color.gray;
            }

            repairButton.interactable = allMet && !isRepaired;
            repairButton.image.color = allMet ? Color.cyan : new Color(0.12f, 0.13f, 0.15f);
            repairButton.GetComponentInChildren<TMP_Text>().color = allMet ? Color.black : Color.gray;

            if (!signals.IsValid)
            {
                statusText.text = "\uc2e0\ud638 \uc5c6\uc74c";
            }
            else if (allMet)
            {
                statusText.text = "\uc2e0\ud638 \uc7a0\uae08 \uac00\ub2a5";
            }
            else
            {
                statusText.text = $"X {signals.X.Frequency} / Y {signals.Y.Frequency} / \ucd9c\ub825 {signals.MaxAmplitude:0}";
            }
        }

        private void DrawSignal(IReadOnlyList<Vector2> points)
        {
            lissajousLine.positionCount = points.Count;

            for (int index = 0; index < points.Count; index++)
            {
                Vector2 point = points[index] * ScopeScale;
                lissajousLine.SetPosition(index, new Vector3(point.x, point.y, 0f));
            }
        }

        private void DrawElectron(SignalPair signals, IReadOnlyList<Vector2> points)
        {
            if (!signals.IsValid)
            {
                electronDot.positionCount = 0;
                return;
            }

            electronTime += Time.deltaTime * 1.9f;
            Vector2 center = new Vector2(
                Mathf.Sin(signals.X.Frequency * electronTime + signals.X.PhaseRadians) * signals.X.Amplitude,
                Mathf.Sin(signals.Y.Frequency * electronTime + signals.Y.PhaseRadians) * signals.Y.Amplitude) * ScopeScale;

            electronDot.positionCount = 18;
            for (int index = 0; index < electronDot.positionCount; index++)
            {
                float angle = index / (float)electronDot.positionCount * Mathf.PI * 2f;
                Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.07f;
                electronDot.SetPosition(index, new Vector3(point.x, point.y, 0f));
            }
        }

        private void Repair()
        {
            if (!repairButton.interactable)
            {
                return;
            }

            isRepaired = true;
            statusText.text = "\uc2dc\uc2a4\ud15c \uc218\ub9ac \uc644\ub8cc";
            statusText.color = Color.cyan;
        }

        private void SetRect(RectTransform rect, Vector2 position, Vector2 size, Vector2 anchor)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private void ApplyPreset(RectTransform rect, RectTransformPreset preset)
        {
            rect.anchorMin = preset.AnchorMin;
            rect.anchorMax = preset.AnchorMax;
            rect.pivot = preset.Pivot;
            rect.anchoredPosition = preset.Position;
            rect.sizeDelta = preset.Size;
        }

        private readonly struct RectTransformPreset
        {
            public Vector2 AnchorMin { get; }
            public Vector2 AnchorMax { get; }
            public Vector2 Pivot { get; }
            public Vector2 Position { get; }
            public Vector2 Size { get; }

            public RectTransformPreset(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size)
            {
                AnchorMin = anchorMin;
                AnchorMax = anchorMax;
                Pivot = pivot;
                Position = position;
                Size = size;
            }

            public static RectTransformPreset Fill()
            {
                return new RectTransformPreset(Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            }
        }
    }
}
