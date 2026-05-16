using Project8.Domain.Data;
using Project8.Infrastructure.Installers;
using Project8.Infrastructure.UnityAdapters;
using Project8.Presentation.Views;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project8.Editor
{
    public static class Project8SceneSetup
    {
        private const string RootPath = "Assets/_Project8";
        private const string ScenePath = RootPath + "/Scenes/TteokbokkiPoC.unity";
        private const string ConfigPath = RootPath + "/ScriptableObjects/Config/SO_GameConfig.asset";
        private const string IngredientPath = RootPath + "/ScriptableObjects/Ingredients";
        private const string OrderPath = RootPath + "/ScriptableObjects/Orders";
        private const string FontPath = "Assets/Packages/TextMesh Pro/Resources/Fonts & Materials/PyeojinGothic-Bold SDF.asset";

        private static TMP_FontAsset _font;

        [MenuItem("Project8/Setup Playable PoC Scene")]
        public static void BuildPlayableSceneAndAssets()
        {
            EnsureFolders();
            _font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);

            var config = CreateGameConfig();
            var ingredients = CreateIngredients();
            var orders = CreateOrders();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            scene.name = "TteokbokkiPoC";
            EditorSceneManager.SetActiveScene(scene);

            var canvas = CreateCanvas();
            CreateEventSystem();

            var hudView = CreateHud(canvas.transform);
            var potView = CreatePotPanel(canvas.transform);
            var ingredientPanelView = CreateIngredientPanel(canvas.transform);
            var orderQueueView = CreateOrderQueue(canvas.transform);

            var scope = new GameObject("GameLifetimeScope").AddComponent<GameLifetimeScope>();
            AssignLifetimeScope(scope, config, ingredients, orders, potView, ingredientPanelView, orderQueueView, hudView);

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorSceneManager.CloseScene(scene, true);
            AddSceneToBuildSettings(ScenePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureFolders()
        {
            EnsureFolder(RootPath + "/Editor");
            EnsureFolder(RootPath + "/ScriptableObjects");
            EnsureFolder(RootPath + "/ScriptableObjects/Config");
            EnsureFolder(IngredientPath);
            EnsureFolder(OrderPath);
            EnsureFolder(RootPath + "/Scenes");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = path.Substring(0, path.LastIndexOf('/'));
            var name = path.Substring(path.LastIndexOf('/') + 1);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static SO_GameConfig CreateGameConfig()
        {
            var config = LoadOrCreate<SO_GameConfig>(ConfigPath);
            var serialized = new SerializedObject(config);

            SetTaste(serialized.FindProperty("_initialTaste"), 40f, 40f, 40f);
            serialized.FindProperty("_initialVolume").floatValue = 60f;
            serialized.FindProperty("_maxVolume").floatValue = 100f;
            serialized.FindProperty("_initialFoodType").enumValueIndex = (int)FoodType.Tteokbokki;
            serialized.FindProperty("_thickIncreasePerSecond").floatValue = 2f;
            serialized.FindProperty("_volumeDecreasePerSecond").floatValue = 0.5f;
            serialized.FindProperty("_friedRiceThreshold").floatValue = 85f;
            serialized.FindProperty("_burnPenaltyThreshold").floatValue = 95f;
            serialized.FindProperty("_maxActiveOrders").intValue = 6;
            serialized.FindProperty("_orderSpawnIntervalMin").floatValue = 4f;
            serialized.FindProperty("_orderSpawnIntervalMax").floatValue = 7f;
            serialized.FindProperty("_gameSeconds").floatValue = 120f;
            serialized.FindProperty("_servingVolumeCost").floatValue = 20f;
            serialized.FindProperty("_minimumServingVolume").floatValue = 20f;

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(config);
            return config;
        }

        private static SO_IngredientDefinition[] CreateIngredients()
        {
            return new[]
            {
                CreateIngredient("spicy_powder", "고춧가루", 20f, 0f, 0f, 0f, false),
                CreateIngredient("sugar", "설탕", 0f, 20f, 0f, 0f, false),
                CreateIngredient("water", "물", -5f, -5f, -25f, 10f, false),
                CreateIngredient("rice_cake", "떡", 0f, 0f, 5f, 10f, false),
                CreateIngredient("fish_cake", "어묵", 0f, 5f, 5f, 10f, false),
                CreateIngredient("rice", "밥", 0f, 0f, 0f, 0f, true)
            };
        }

        private static SO_IngredientDefinition CreateIngredient(string id, string displayName, float spicy, float sweet, float thick, float volume, bool isRice)
        {
            var asset = LoadOrCreate<SO_IngredientDefinition>(IngredientPath + "/SO_Ingredient_" + id + ".asset");
            var serialized = new SerializedObject(asset);

            serialized.FindProperty("_id").stringValue = id;
            serialized.FindProperty("_displayName").stringValue = displayName;
            SetTaste(serialized.FindProperty("_tasteDelta"), spicy, sweet, thick);
            serialized.FindProperty("_volumeDelta").floatValue = volume;
            serialized.FindProperty("_isRice").boolValue = isRice;

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static SO_OrderDefinition[] CreateOrders()
        {
            return new[]
            {
                CreateOrder("mild_tteokbokki", "순한 떡볶이", FoodType.Tteokbokki, 20f, 45f, 35f, 65f, 35f, 65f),
                CreateOrder("spicy_tteokbokki", "매운 떡볶이", FoodType.Tteokbokki, 65f, 95f, 20f, 55f, 40f, 75f),
                CreateOrder("sweet_tteokbokki", "달콤 떡볶이", FoodType.Tteokbokki, 30f, 65f, 65f, 95f, 35f, 70f),
                CreateOrder("thick_tteokbokki", "꾸덕 떡볶이", FoodType.Tteokbokki, 40f, 75f, 35f, 70f, 70f, 90f),
                CreateOrder("fried_rice", "볶음밥", FoodType.FriedRice, 40f, 90f, 20f, 70f, 85f, 100f)
            };
        }

        private static SO_OrderDefinition CreateOrder(string id, string displayName, FoodType foodType, float spicyMin, float spicyMax, float sweetMin, float sweetMax, float thickMin, float thickMax)
        {
            var asset = LoadOrCreate<SO_OrderDefinition>(OrderPath + "/SO_Order_" + id + ".asset");
            var serialized = new SerializedObject(asset);

            serialized.FindProperty("_id").stringValue = id;
            serialized.FindProperty("_displayName").stringValue = displayName;
            serialized.FindProperty("_foodType").enumValueIndex = (int)foodType;
            SetRange(serialized.FindProperty("_spicyRange"), spicyMin, spicyMax);
            SetRange(serialized.FindProperty("_sweetRange"), sweetMin, sweetMax);
            SetRange(serialized.FindProperty("_thickRange"), thickMin, thickMax);
            serialized.FindProperty("_patienceSeconds").floatValue = 40f;
            serialized.FindProperty("_baseScore").intValue = 100;

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        private static void CreateEventSystem()
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule), typeof(NewInputSystemEventModuleGuard));
        }

        private static HudView CreateHud(Transform parent)
        {
            var panel = CreatePanel(parent, "HUD", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(0f, -20f), new Vector2(0f, 90f));
            var scoreText = CreateText(panel.transform, "ScoreText", "점수 0", new Vector2(20f, -20f), new Vector2(220f, 40f), 28, TextAlignmentOptions.MidlineLeft);
            var timeText = CreateText(panel.transform, "TimeText", "남은 시간 120", new Vector2(260f, -20f), new Vector2(260f, 40f), 28, TextAlignmentOptions.MidlineLeft);
            var stateText = CreateText(panel.transform, "StateText", "떡볶이 PoC", new Vector2(560f, -20f), new Vector2(520f, 40f), 28, TextAlignmentOptions.MidlineLeft);

            var view = panel.AddComponent<HudView>();
            AssignObject(view, "_scoreText", scoreText);
            AssignObject(view, "_timeText", timeText);
            AssignObject(view, "_gameStateText", stateText);
            return view;
        }

        private static PotView CreatePotPanel(Transform parent)
        {
            var panel = CreatePanel(parent, "Pot Panel", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(30f, 40f), new Vector2(500f, 680f));
            CreateText(panel.transform, "Title", "냄비", new Vector2(20f, -20f), new Vector2(220f, 44f), 32, TextAlignmentOptions.MidlineLeft);
            var foodTypeText = CreateText(panel.transform, "FoodTypeText", "떡볶이", new Vector2(240f, -20f), new Vector2(220f, 44f), 26, TextAlignmentOptions.MidlineRight);

            var spicyText = CreateText(panel.transform, "SpicyText", "매운맛 40", new Vector2(20f, -100f), new Vector2(180f, 34f), 24, TextAlignmentOptions.MidlineLeft);
            var spicySlider = CreateSlider(panel.transform, "SpicySlider", new Vector2(210f, -100f), new Vector2(250f, 28f));
            var sweetText = CreateText(panel.transform, "SweetText", "단맛 40", new Vector2(20f, -170f), new Vector2(180f, 34f), 24, TextAlignmentOptions.MidlineLeft);
            var sweetSlider = CreateSlider(panel.transform, "SweetSlider", new Vector2(210f, -170f), new Vector2(250f, 28f));
            var thickText = CreateText(panel.transform, "ThickText", "농도 40", new Vector2(20f, -240f), new Vector2(180f, 34f), 24, TextAlignmentOptions.MidlineLeft);
            var thickSlider = CreateSlider(panel.transform, "ThickSlider", new Vector2(210f, -240f), new Vector2(250f, 28f));
            var volumeText = CreateText(panel.transform, "VolumeText", "양 60", new Vector2(20f, -310f), new Vector2(180f, 34f), 24, TextAlignmentOptions.MidlineLeft);
            var volumeSlider = CreateSlider(panel.transform, "VolumeSlider", new Vector2(210f, -310f), new Vector2(250f, 28f));

            var view = panel.AddComponent<PotView>();
            AssignObject(view, "_foodTypeText", foodTypeText);
            AssignObject(view, "_spicySlider", spicySlider);
            AssignObject(view, "_sweetSlider", sweetSlider);
            AssignObject(view, "_thickSlider", thickSlider);
            AssignObject(view, "_volumeSlider", volumeSlider);
            AssignObject(view, "_spicyText", spicyText);
            AssignObject(view, "_sweetText", sweetText);
            AssignObject(view, "_thickText", thickText);
            AssignObject(view, "_volumeText", volumeText);
            return view;
        }

        private static IngredientPanelView CreateIngredientPanel(Transform parent)
        {
            var panel = CreatePanel(parent, "Ingredient Panel", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(780f, 300f));
            CreateText(panel.transform, "Title", "재료", new Vector2(20f, -20f), new Vector2(240f, 40f), 30, TextAlignmentOptions.MidlineLeft);

            var ids = new[] { "spicy_powder", "sugar", "water", "rice_cake", "fish_cake", "rice" };
            var labels = new[] { "고춧가루", "설탕", "물", "떡", "어묵", "밥" };
            var buttons = new Button[ids.Length];
            var texts = new TMP_Text[ids.Length];

            for (var i = 0; i < ids.Length; i++)
            {
                var x = 30f + (i % 3) * 240f;
                var y = -90f - (i / 3) * 80f;
                buttons[i] = CreateButton(panel.transform, labels[i] + " Button", labels[i], new Vector2(x, y), new Vector2(210f, 54f), out texts[i]);
            }

            var messageText = CreateText(panel.transform, "MessageText", "재료를 선택하세요.", new Vector2(30f, -250f), new Vector2(700f, 34f), 22, TextAlignmentOptions.MidlineLeft);

            var view = panel.AddComponent<IngredientPanelView>();
            AssignIngredientButtons(view, ids, buttons, texts);
            AssignObject(view, "_messageText", messageText);
            return view;
        }

        private static OrderQueueView CreateOrderQueue(Transform parent)
        {
            var panel = CreatePanel(parent, "Order Queue", new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-30f, 40f), new Vector2(620f, 680f));
            CreateText(panel.transform, "Title", "주문", new Vector2(20f, -20f), new Vector2(220f, 44f), 32, TextAlignmentOptions.MidlineLeft);

            var slotRoots = new GameObject[3];
            var nameTexts = new TMP_Text[3];
            var tasteTexts = new TMP_Text[3];
            var timeTexts = new TMP_Text[3];
            var buttons = new Button[3];

            for (var i = 0; i < 3; i++)
            {
                var slot = CreatePanel(panel.transform, "Order Slot " + (i + 1), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(20f, -90f - i * 170f), new Vector2(-40f, 140f));
                slotRoots[i] = slot;
                nameTexts[i] = CreateText(slot.transform, "NameText", "주문", new Vector2(16f, -14f), new Vector2(330f, 32f), 22, TextAlignmentOptions.MidlineLeft);
                tasteTexts[i] = CreateText(slot.transform, "TasteText", "매 / 단 / 농", new Vector2(16f, -54f), new Vector2(380f, 30f), 18, TextAlignmentOptions.MidlineLeft);
                timeTexts[i] = CreateText(slot.transform, "TimeText", "40초", new Vector2(420f, -14f), new Vector2(80f, 32f), 22, TextAlignmentOptions.MidlineRight);
                buttons[i] = CreateButton(slot.transform, "Serve Button", "서빙", new Vector2(420f, -74f), new Vector2(130f, 44f), out _);
            }

            var messageText = CreateText(panel.transform, "MessageText", "조건에 맞는 주문을 서빙하세요.", new Vector2(20f, -620f), new Vector2(560f, 34f), 22, TextAlignmentOptions.MidlineLeft);

            var view = panel.AddComponent<OrderQueueView>();
            AssignOrderSlots(view, slotRoots, nameTexts, tasteTexts, timeTexts, buttons);
            AssignObject(view, "_messageText", messageText);
            return view;
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);

            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;

            panel.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.12f, 0.88f);
            return panel;
        }

        private static TMP_Text CreateText(Transform parent, string name, string value, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize, TextAlignmentOptions alignment)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);

            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;

            var text = textObject.GetComponent<TMP_Text>();
            text.text = value;
            text.font = _font;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;

            return text;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, Vector2 sizeDelta, out TMP_Text labelText)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            var rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;

            var image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.85f, 0.28f, 0.18f, 1f);

            var button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            labelText = CreateText(buttonObject.transform, "Label", label, Vector2.zero, sizeDelta, 20, TextAlignmentOptions.Center);
            labelText.rectTransform.anchorMin = Vector2.zero;
            labelText.rectTransform.anchorMax = Vector2.one;
            labelText.rectTransform.offsetMin = Vector2.zero;
            labelText.rectTransform.offsetMax = Vector2.zero;

            return button;
        }

        private static Slider CreateSlider(Transform parent, string name, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var sliderObject = new GameObject(name, typeof(RectTransform), typeof(Slider));
            sliderObject.transform.SetParent(parent, false);

            var rect = sliderObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;

            var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(sliderObject.transform, false);
            Stretch(background.GetComponent<RectTransform>());
            background.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 1f);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderObject.transform, false);
            Stretch(fillArea.GetComponent<RectTransform>());

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);
            Stretch(fill.GetComponent<RectTransform>());
            fill.GetComponent<Image>().color = new Color(0.98f, 0.75f, 0.22f, 1f);

            var slider = sliderObject.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.value = 0f;
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.targetGraphic = fill.GetComponent<Image>();

            return slider;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static T LoadOrCreate<T>(string path)
            where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void SetTaste(SerializedProperty property, float spicy, float sweet, float thick)
        {
            property.FindPropertyRelative("Spicy").floatValue = spicy;
            property.FindPropertyRelative("Sweet").floatValue = sweet;
            property.FindPropertyRelative("Thick").floatValue = thick;
        }

        private static void SetRange(SerializedProperty property, float min, float max)
        {
            property.FindPropertyRelative("Min").floatValue = min;
            property.FindPropertyRelative("Max").floatValue = max;
        }

        private static void AssignObject(Object target, string propertyName, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).objectReferenceValue = value;
            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private static void AssignLifetimeScope(GameLifetimeScope scope, SO_GameConfig config, SO_IngredientDefinition[] ingredients, SO_OrderDefinition[] orders, PotView potView, IngredientPanelView ingredientPanelView, OrderQueueView orderQueueView, HudView hudView)
        {
            var serialized = new SerializedObject(scope);
            serialized.FindProperty("_gameConfig").objectReferenceValue = config;
            AssignObjectArray(serialized.FindProperty("_ingredients"), ingredients);
            AssignObjectArray(serialized.FindProperty("_orders"), orders);
            serialized.FindProperty("_potView").objectReferenceValue = potView;
            serialized.FindProperty("_ingredientPanelView").objectReferenceValue = ingredientPanelView;
            serialized.FindProperty("_orderQueueView").objectReferenceValue = orderQueueView;
            serialized.FindProperty("_hudView").objectReferenceValue = hudView;
            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(scope);
        }

        private static void AssignIngredientButtons(IngredientPanelView view, string[] ids, Button[] buttons, TMP_Text[] labels)
        {
            var serialized = new SerializedObject(view);
            var array = serialized.FindProperty("_buttons");
            array.arraySize = ids.Length;

            for (var i = 0; i < ids.Length; i++)
            {
                var item = array.GetArrayElementAtIndex(i);
                item.FindPropertyRelative("_ingredientId").stringValue = ids[i];
                item.FindPropertyRelative("_button").objectReferenceValue = buttons[i];
                item.FindPropertyRelative("_label").objectReferenceValue = labels[i];
            }

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(view);
        }

        private static void AssignOrderSlots(OrderQueueView view, GameObject[] roots, TMP_Text[] names, TMP_Text[] tastes, TMP_Text[] times, Button[] buttons)
        {
            var serialized = new SerializedObject(view);
            var array = serialized.FindProperty("_slots");
            array.arraySize = roots.Length;

            for (var i = 0; i < roots.Length; i++)
            {
                var item = array.GetArrayElementAtIndex(i);
                item.FindPropertyRelative("_root").objectReferenceValue = roots[i];
                item.FindPropertyRelative("_nameText").objectReferenceValue = names[i];
                item.FindPropertyRelative("_tasteText").objectReferenceValue = tastes[i];
                item.FindPropertyRelative("_timeText").objectReferenceValue = times[i];
                item.FindPropertyRelative("_serveButton").objectReferenceValue = buttons[i];
            }

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(view);
        }

        private static void AssignObjectArray<T>(SerializedProperty array, T[] values)
            where T : Object
        {
            array.arraySize = values.Length;

            for (var i = 0; i < values.Length; i++)
            {
                array.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
        }
    }
}
