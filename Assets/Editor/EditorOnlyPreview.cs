#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class EditorOnlyPreview
{
    private const string prefabPath = "Assets/FPS-Game/Prefabs/Player.prefab";
    private const string childNameToToggle = "PlayerUI";

    private static string _previousScenePath;

    static EditorOnlyPreview()
    {
        // EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                // Trước khi chạy:
                SaveCurrentScenePath();
                // ToggleChildInPrefab(prefabPath, childNameToToggle, false); // TẮT
                AutoLoadScene0();
                break;

            case PlayModeStateChange.EnteredEditMode:
                // Sau khi STOP chạy:
                // ToggleChildInPrefab(prefabPath, childNameToToggle, true); // BẬT lại
                RestorePreviousScene();
                break;
        }
    }

    static void ToggleChildInPrefab(string path, string childName, bool enable)
    {
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefabAsset == null)
        {
            Debug.LogWarning($"Không tìm thấy prefab tại đường dẫn: {path}");
            return;
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        Transform targetChild = prefabRoot.transform.Find(childName);

        if (targetChild != null)
        {
            targetChild.gameObject.SetActive(enable);
            string state = enable ? "bật" : "tắt";
            Debug.Log($"[EditorOnlyPreview] Đã {state} '{childName}' trong prefab '{path}'");

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy đối tượng con '{childName}' trong prefab '{path}'");
        }

        PrefabUtility.UnloadPrefabContents(prefabRoot);
    }

    static void AutoLoadScene0()
    {
        if (EditorBuildSettings.scenes.Length == 0)
        {
            Debug.LogWarning("[EditorOnlyPreview] Không có scene nào trong Build Settings.");
            return;
        }

        string scenePath = EditorBuildSettings.scenes[0].path;

        if (SceneManager.GetActiveScene().path != scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
                Debug.Log($"[EditorOnlyPreview] Đã chuyển sang Scene 0: {scenePath}");
            }
            else
            {
                Debug.LogWarning("[EditorOnlyPreview] Người dùng hủy lưu scene hiện tại.");
                EditorApplication.isPlaying = false;
            }
        }
    }

    static void SaveCurrentScenePath()
    {
        _previousScenePath = SceneManager.GetActiveScene().path;
    }

    static void RestorePreviousScene()
    {
        if (!string.IsNullOrEmpty(_previousScenePath) && SceneManager.GetActiveScene().path != _previousScenePath)
        {
            EditorSceneManager.OpenScene(_previousScenePath);
            Debug.Log($"[EditorOnlyPreview] Quay về scene trước: {_previousScenePath}");
        }
    }
}
#endif
