using System.IO;
using UnityEditor;

namespace Project8.Editor
{
    [InitializeOnLoad]
    public static class Project8AutoSetup
    {
        private const string RequestPath = "Assets/_Project8/Editor/RUN_SETUP.txt";
        private const string InputPatchRequestPath = "Assets/_Project8/Editor/RUN_INPUT_PATCH.txt";

        static Project8AutoSetup()
        {
            EditorApplication.delayCall += TryRunSetup;
        }

        private static void TryRunSetup()
        {
            if (File.Exists(RequestPath))
            {
                File.Delete(RequestPath);
                Project8SceneSetup.BuildPlayableSceneAndAssets();
                return;
            }

            if (File.Exists(InputPatchRequestPath))
            {
                File.Delete(InputPatchRequestPath);
                Project8InputSystemScenePatch.PatchScene();
            }
        }
    }
}
