using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Adds Scenes-tab to toolbar to quickly change between main scenes
/// </summary>
static class ChangeSceneMenus
{
    [MenuItem("Scenes/Main Menu")]
    static void ToggleInspectorLock()
    {
        FindAndOpenScene("MainMenu");
    }

    [MenuItem("Scenes/Level 001 AR")]
    static void LoadLevel001() {
        FindAndOpenScene("Level_001 AR");
    }

    static void FindAndOpenScene(string sceneName) {
        string[] guids = AssetDatabase.FindAssets(sceneName + " t:Scene");

        if (guids.Length > 0) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            if (assetPath != "") {
                EditorSceneManager.SaveOpenScenes();
                EditorSceneManager.OpenScene(assetPath);
            }
        }
    }

}
 