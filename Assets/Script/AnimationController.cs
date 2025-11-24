using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationController : MonoBehaviour
{
    public string nextSceneName = "gakkou3";

    // Inspectorで設定できるフラグ。チェックされているとアニメ終了時に3つ目のエンディングを解禁する
    public bool Kamigata = false;

    // 新しいフラグ: チェックされているとアニメ終了時に4つ目のエンディングを解禁する
    public bool Infelno = false;

    public void OnAnimationEnd()
    {
        // Kamigataフラグが有効ならエンディング3を解禁
        if (Kamigata)
        {
            PlayerPrefs.SetInt("Ending_3", 1);
            PlayerPrefs.Save();
            Debug.Log("Unlocked Ending_3 due to Kamigata flag in AnimationController.");
        }

        // Infelnoフラグが有効ならエンディング4を解禁
        if (Infelno)
        {
            PlayerPrefs.SetInt("Ending_4", 1);
            PlayerPrefs.Save();
            Debug.Log("Unlocked Ending_4 due to Infelno flag in AnimationController.");
        }

        SceneManager.LoadScene(nextSceneName);
    }
}