using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Project8.Infrastructure.UnityAdapters;

namespace Project8.Editor
{
    public static class Project8InputSystemScenePatch
    {
        private const string ScenePath = "Assets/_Project8/Scenes/TteokbokkiPoC.unity";

        [MenuItem("Project8/Patch Scene To New Input System")]
        public static void PatchScene()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
            EditorSceneManager.SetActiveScene(scene);

            var eventSystem = Object.FindFirstObjectByType<EventSystem>();

            if (eventSystem == null)
            {
                eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            }

            var oldModule = eventSystem.GetComponent<StandaloneInputModule>();

            if (oldModule != null)
            {
                Object.DestroyImmediate(oldModule, true);
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            if (eventSystem.GetComponent<NewInputSystemEventModuleGuard>() == null)
            {
                eventSystem.gameObject.AddComponent<NewInputSystemEventModuleGuard>();
            }

            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
