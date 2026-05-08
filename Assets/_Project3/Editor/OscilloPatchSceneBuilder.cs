#if UNITY_EDITOR
using Project3.OscilloPatch;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project3.OscilloPatchEditor
{
    public static class OscilloPatchSceneBuilder
    {
        [MenuItem("Project3/Rebuild Oscillo Patch Scenes")]
        public static void BuildAll()
        {
            BuildScene("Assets/_Project/Scenes/POC3.unity");
            BuildScene("Assets/_Project3/Scenes/POC3.unity");
        }

        private static void BuildScene(string path)
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            OscilloPatchGame game = Object.FindFirstObjectByType<OscilloPatchGame>();

            if (game == null)
            {
                GameObject gameObject = new GameObject("Oscillo Patch Game");
                game = gameObject.AddComponent<OscilloPatchGame>();
            }

            game.RebuildSceneObjectsForEditor();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}
#endif
