using UnityEngine;
using UnityEngine.SceneManagement;

// シーン読み込み時に "gakkou" シーンが読み込まれ、かつ Ending_6 が解放されている場合に
// 自動で "Yamio1" シーンへ遷移する静的リダイレクタ
public static class GakkouSceneRedirector
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "gakkou")
        {
            bool ending6Unlocked = PlayerPrefs.GetInt("Ending_6", 0) == 1;
            Debug.Log($"GakkouSceneRedirector: 'gakkou' loaded. Ending_6={ending6Unlocked}");
            if (ending6Unlocked)
            {
                Debug.Log("GakkouSceneRedirector: Ending_6 unlocked -> loading 'Yamio1'.");
                SceneManager.LoadScene("Yamio1");
            }
        }
    }
}
