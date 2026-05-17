using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Project9.Editor
{
    public static class Project9TmpBoldDisabler
    {
        [MenuItem("Project9/Disable TMP Bold In Current Scene")]
        public static void DisableBoldInCurrentScene()
        {
            var changedCount = 0;
            var texts = Object.FindObjectsByType<TMP_Text>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            foreach (var text in texts)
            {
                if ((text.fontStyle & FontStyles.Bold) == 0)
                {
                    continue;
                }

                Undo.RecordObject(text, "Disable TMP Bold");
                text.fontStyle &= ~FontStyles.Bold;
                EditorUtility.SetDirty(text);
                changedCount++;
            }

            if (changedCount > 0)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            Debug.Log($"Disabled TMP Bold on {changedCount} text object(s) in the current scene.");
        }
    }
}
