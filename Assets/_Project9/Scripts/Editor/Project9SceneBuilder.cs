using Project9.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Project9.Editor
{
    [InitializeOnLoad]
    public static class Project9SceneBuilder
    {
        private const string ScenePath = "Assets/_Project9/Scenes/POC9.unity";

        static Project9SceneBuilder()
        {
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        [MenuItem("Project9/Build POC9 Scene UI")]
        public static void BuildAndSavePoc9Scene()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath);
            BuildAndSaveLoadedScene(scene, exitOnFailure: true);
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (scene.path != ScenePath)
            {
                return;
            }

            EditorApplication.delayCall += () => BuildAndSaveLoadedScene(scene, exitOnFailure: false);
        }

        private static void BuildAndSaveLoadedScene(Scene scene, bool exitOnFailure)
        {
            var root = Object.FindFirstObjectByType<Project9SceneRoot>();
            if (root == null)
            {
                Debug.LogError("Project9SceneRoot was not found in POC9 scene.");
                if (exitOnFailure)
                {
                    EditorApplication.Exit(1);
                }
                return;
            }

            NormalizeEventSystems();
            root.Build();
            NormalizeEventSystems();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("POC9 scene UI built and saved.");
        }

        private static void NormalizeEventSystems()
        {
            var eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.InstanceID);
            EventSystem keep = null;

            foreach (var eventSystem in eventSystems)
            {
                if (keep == null)
                {
                    keep = eventSystem;
                    eventSystem.gameObject.name = "EventSystem";
                    continue;
                }

                Object.DestroyImmediate(eventSystem.gameObject);
            }

            if (keep == null)
            {
                var go = new GameObject("EventSystem");
                keep = go.AddComponent<EventSystem>();
            }

            foreach (var oldModule in keep.GetComponents<StandaloneInputModule>())
            {
                Object.DestroyImmediate(oldModule);
            }

            if (keep.GetComponent<InputSystemUIInputModule>() == null)
            {
                keep.gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }
    }
}
