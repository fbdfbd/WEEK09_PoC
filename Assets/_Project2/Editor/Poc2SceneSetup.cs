using System.Collections.Generic;
using Project2;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project2.Editor
{
    [InitializeOnLoad]
    public static class Poc2SceneSetup
    {
        private const string ScenePath = "Assets/_Project2/POC2.unity";
        private const string AutoSetupKey = "Project2.POC2.AutoSetupDoneV2";

        static Poc2SceneSetup()
        {
            EditorApplication.delayCall += RunAutoSetupOnce;
        }

        private static void RunAutoSetupOnce()
        {
            if (Application.isBatchMode || EditorPrefs.GetBool(AutoSetupKey, false))
            {
                return;
            }

            if (!System.IO.File.Exists(ScenePath))
            {
                EditorPrefs.SetBool(AutoSetupKey, true);
                Debug.Log("POC2 auto setup: scene file missing, creating scene.");
                SetupScene();
                return;
            }

            string sceneText = System.IO.File.ReadAllText(ScenePath);
            if (!sceneText.Contains("Red_TargetCurve") || !sceneText.Contains("StartPanel"))
            {
                EditorPrefs.SetBool(AutoSetupKey, true);
                Debug.Log("POC2 auto setup: scene is incomplete, rebuilding scene.");
                SetupScene();
            }
            else
            {
                EditorPrefs.SetBool(AutoSetupKey, true);
            }
        }

        [MenuItem("Project2/Setup POC2 Scene")]
        public static void SetupScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "POC2";

            Material redMaterial = CreateMaterial("POC2_RedCurve", new Color(1f, 0.18f, 0.38f, 0.75f));
            Material blueMaterial = CreateMaterial("POC2_BlueCurve", new Color(0f, 0.78f, 1f, 0.85f));
            Material sparkMaterial = CreateMaterial("POC2_Spark", Color.white);
            Material nodeMaterial = CreateMaterial("POC2_Node", new Color(0.12f, 0.12f, 0.14f, 1f));

            Camera camera = CreateCamera();
            GameObject root = new GameObject("POC2_Core");
            Poc2CoreLoop core = root.AddComponent<Poc2CoreLoop>();

            Transform curveRoot = new GameObject("CurveRoot").transform;
            curveRoot.SetParent(root.transform, false);

            LineRenderer redCurve = CreateCurve("Red_TargetCurve", curveRoot, redMaterial, 0.055f);
            LineRenderer blueCurve = CreateCurve("Blue_PlayerCurve", curveRoot, blueMaterial, 0.05f);

            Transform sparkRoot = new GameObject("Sparks").transform;
            sparkRoot.SetParent(curveRoot, false);
            Transform[] sparkViews = CreateSparks(sparkRoot, sparkMaterial, 28);

            Poc2CoreLoop.WeldNode[] nodes = CreateNodes(curveRoot, nodeMaterial);

            Canvas canvas = CreateCanvas();
            Text statusText = CreateText("StatusText", canvas.transform, "Resonance Welding", 26, TextAnchor.MiddleCenter);
            SetRect(statusText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -46f), new Vector2(800f, 52f));

            RectTransform phaseIndicator = CreateSideTrack(canvas.transform, "PhaseTrack", true);
            RectTransform rotationIndicator = CreateSideTrack(canvas.transform, "RotationTrack", false);

            GameObject gearPanel = CreatePanel("GearPanel", canvas.transform, new Color(0f, 0f, 0f, 0f));
            RectTransform gearRect = gearPanel.GetComponent<RectTransform>();
            SetRect(gearRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 42f), new Vector2(540f, 64f));
            HorizontalLayoutGroup gearLayout = gearPanel.AddComponent<HorizontalLayoutGroup>();
            gearLayout.childAlignment = TextAnchor.MiddleCenter;
            gearLayout.spacing = 10f;
            gearLayout.childControlWidth = false;
            gearLayout.childControlHeight = false;

            Poc2CoreLoop.GearOption[] gears = CreateGearButtons(gearPanel.transform);

            GameObject startPanel = CreateOverlayPanel(canvas.transform, "StartPanel");
            Text startTitle = CreateText("Title", startPanel.transform, "RESONANCE WELDING", 34, TextAnchor.MiddleCenter);
            SetRect(startTitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 74f), new Vector2(720f, 70f));
            Text startBody = CreateText("Body", startPanel.transform, "Drag left side for phase, right side for rotation.", 20, TextAnchor.MiddleCenter);
            SetRect(startBody.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 22f), new Vector2(720f, 40f));
            Button startButton = CreateButton("StartButton", startPanel.transform, "START", new Vector2(0f, -58f), new Vector2(190f, 54f));

            GameObject resultPanel = CreateOverlayPanel(canvas.transform, "ResultPanel");
            Text resultText = CreateText("ResultText", resultPanel.transform, "SYSTEM SYNCHRONIZED", 32, TextAnchor.MiddleCenter);
            SetRect(resultText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 54f), new Vector2(720f, 70f));
            Button retryButton = CreateButton("RetryButton", resultPanel.transform, "RETRY", new Vector2(-100f, -40f), new Vector2(170f, 52f));
            Button quitButton = CreateButton("QuitButton", resultPanel.transform, "QUIT", new Vector2(100f, -40f), new Vector2(170f, 52f));

            AssignCore(core, redCurve, blueCurve, curveRoot, sparkViews, nodes, startPanel, resultPanel, statusText, resultText, gears, phaseIndicator, rotationIndicator, startButton, retryButton, quitButton);

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);

            Selection.activeObject = root;
            Debug.Log($"POC2 scene setup complete: {ScenePath}");
        }

        private static Camera CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.02f, 0.02f, 0.03f, 1f);
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            return camera;
        }

        private static LineRenderer CreateCurve(string name, Transform parent, Material material, float width)
        {
            GameObject curveObject = new GameObject(name);
            curveObject.transform.SetParent(parent, false);
            LineRenderer line = curveObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = false;
            line.material = material;
            line.startWidth = width;
            line.endWidth = width;
            line.numCapVertices = 4;
            line.numCornerVertices = 4;
            line.positionCount = 0;
            return line;
        }

        private static Transform[] CreateSparks(Transform parent, Material material, int count)
        {
            Transform[] sparks = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spark.name = $"Spark_{i + 1:00}";
                spark.transform.SetParent(parent, false);
                spark.transform.localScale = Vector3.one * 0.08f;
                spark.GetComponent<Renderer>().sharedMaterial = material;
                Object.DestroyImmediate(spark.GetComponent<Collider>());
                spark.SetActive(false);
                sparks[i] = spark.transform;
            }

            return sparks;
        }

        private static Poc2CoreLoop.WeldNode[] CreateNodes(Transform parent, Material material)
        {
            Vector2[] positions =
            {
                new Vector2(-0.6f, -0.5f),
                new Vector2(0.6f, 0.5f),
                new Vector2(0f, 0.8f)
            };

            Poc2CoreLoop.WeldNode[] nodes = new Poc2CoreLoop.WeldNode[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject nodeObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                nodeObject.name = $"WeldNode_T{i + 1}";
                nodeObject.transform.SetParent(parent, false);
                nodeObject.transform.localPosition = positions[i] * 3.2f;
                Object.DestroyImmediate(nodeObject.GetComponent<Collider>());

                Material instanceMaterial = Object.Instantiate(material);
                instanceMaterial.name = $"POC2_Node_T{i + 1}";
                Renderer renderer = nodeObject.GetComponent<Renderer>();
                renderer.sharedMaterial = instanceMaterial;

                Text label = CreateWorldLabel(nodeObject.transform, $"T{i + 1}");

                nodes[i] = new Poc2CoreLoop.WeldNode
                {
                    normalizedPosition = positions[i],
                    view = nodeObject.transform,
                    renderer = renderer,
                    label = label
                };
            }

            return nodes;
        }

        private static Text CreateWorldLabel(Transform parent, string value)
        {
            GameObject canvasObject = new GameObject("LabelCanvas");
            canvasObject.transform.SetParent(parent, false);
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(1f, 0.4f);
            canvasRect.localScale = Vector3.one * 0.01f;

            Text text = CreateText("Label", canvasObject.transform, value, 24, TextAnchor.MiddleCenter);
            text.color = Color.white;
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return text;
        }

        private static Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, 1080f);
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
            return canvas;
        }

        private static RectTransform CreateSideTrack(Transform parent, string name, bool left)
        {
            GameObject track = CreatePanel(name, parent, new Color(1f, 1f, 1f, 0.035f));
            RectTransform trackRect = track.GetComponent<RectTransform>();
            Vector2 anchor = left ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f);
            SetRect(trackRect, anchor, anchor, left ? new Vector2(26f, 0f) : new Vector2(-26f, 0f), new Vector2(42f, 360f));

            GameObject indicator = CreatePanel("Indicator", track.transform, new Color(0f, 0.78f, 1f, 0.75f));
            RectTransform indicatorRect = indicator.GetComponent<RectTransform>();
            SetRect(indicatorRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(22f, 58f));
            return indicatorRect;
        }

        private static Poc2CoreLoop.GearOption[] CreateGearButtons(Transform parent)
        {
            Vector2Int[] values =
            {
                new Vector2Int(1, 1),
                new Vector2Int(1, 2),
                new Vector2Int(2, 3),
                new Vector2Int(3, 2),
                new Vector2Int(3, 4)
            };

            Poc2CoreLoop.GearOption[] gears = new Poc2CoreLoop.GearOption[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                Button button = CreateButton($"Gear_{values[i].x}_{values[i].y}", parent, $"{values[i].x}:{values[i].y}", Vector2.zero, new Vector2(92f, 52f));
                LayoutElement layout = button.gameObject.AddComponent<LayoutElement>();
                layout.preferredWidth = 92f;
                layout.preferredHeight = 52f;
                Text label = button.GetComponentInChildren<Text>();
                gears[i] = new Poc2CoreLoop.GearOption
                {
                    frequencyX = values[i].x,
                    frequencyY = values[i].y,
                    button = button,
                    label = label
                };
            }

            return gears;
        }

        private static GameObject CreateOverlayPanel(Transform parent, string name)
        {
            GameObject panel = CreatePanel(name, parent, new Color(0f, 0f, 0f, 0.72f));
            SetRect(panel.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return panel;
        }

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.localScale = Vector3.one;
            Image image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private static Button CreateButton(string name, Transform parent, string labelText, Vector2 position, Vector2 size)
        {
            GameObject buttonObject = CreatePanel(name, parent, new Color(0.16f, 0.16f, 0.2f, 1f));
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            SetRect(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, size);

            Button button = buttonObject.AddComponent<Button>();
            Text label = CreateText("Text", buttonObject.transform, labelText, 20, TextAnchor.MiddleCenter);
            SetRect(label.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return button;
        }

        private static Text CreateText(string name, Transform parent, string value, int size, TextAnchor anchor)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            Text text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = GetDefaultFont();
            text.fontSize = size;
            text.alignment = anchor;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        private static Font GetDefaultFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        private static Material CreateMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            Material material = new Material(shader)
            {
                name = name,
                color = color
            };
            return material;
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
        }

        private static void AssignCore(
            Poc2CoreLoop core,
            LineRenderer redCurve,
            LineRenderer blueCurve,
            Transform curveRoot,
            Transform[] sparkViews,
            Poc2CoreLoop.WeldNode[] nodes,
            GameObject startPanel,
            GameObject resultPanel,
            Text statusText,
            Text resultText,
            Poc2CoreLoop.GearOption[] gears,
            RectTransform phaseIndicator,
            RectTransform rotationIndicator,
            Button startButton,
            Button retryButton,
            Button quitButton)
        {
            SerializedObject serialized = new SerializedObject(core);
            serialized.FindProperty("redCurve").objectReferenceValue = redCurve;
            serialized.FindProperty("blueCurve").objectReferenceValue = blueCurve;
            serialized.FindProperty("curveRoot").objectReferenceValue = curveRoot;
            SetObjectArray(serialized.FindProperty("sparkViews"), sparkViews);
            SetWeldNodeArray(serialized.FindProperty("nodes"), nodes);
            serialized.FindProperty("startPanel").objectReferenceValue = startPanel;
            serialized.FindProperty("resultPanel").objectReferenceValue = resultPanel;
            serialized.FindProperty("statusText").objectReferenceValue = statusText;
            serialized.FindProperty("resultText").objectReferenceValue = resultText;
            SetGearArray(serialized.FindProperty("gearOptions"), gears);
            serialized.FindProperty("phaseIndicator").objectReferenceValue = phaseIndicator;
            serialized.FindProperty("rotationIndicator").objectReferenceValue = rotationIndicator;
            serialized.FindProperty("startButton").objectReferenceValue = startButton;
            serialized.FindProperty("retryButton").objectReferenceValue = retryButton;
            serialized.FindProperty("quitButton").objectReferenceValue = quitButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectArray(SerializedProperty property, Transform[] values)
        {
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }

        private static void SetWeldNodeArray(SerializedProperty property, Poc2CoreLoop.WeldNode[] values)
        {
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                item.FindPropertyRelative("normalizedPosition").vector2Value = values[i].normalizedPosition;
                item.FindPropertyRelative("view").objectReferenceValue = values[i].view;
                item.FindPropertyRelative("label").objectReferenceValue = values[i].label;
                item.FindPropertyRelative("renderer").objectReferenceValue = values[i].renderer;
            }
        }

        private static void SetGearArray(SerializedProperty property, Poc2CoreLoop.GearOption[] values)
        {
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                item.FindPropertyRelative("frequencyX").intValue = values[i].frequencyX;
                item.FindPropertyRelative("frequencyY").intValue = values[i].frequencyY;
                item.FindPropertyRelative("button").objectReferenceValue = values[i].button;
                item.FindPropertyRelative("label").objectReferenceValue = values[i].label;
            }
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i].path == scenePath)
                {
                    scenes[i] = new EditorBuildSettingsScene(scenePath, true);
                    EditorBuildSettings.scenes = scenes.ToArray();
                    return;
                }
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
