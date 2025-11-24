using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    public string scene;
    public string scene2;
    public string scene3;

    // Inspectorでチェック可能なフラグ。チェックされていると PressStart 時に3つ目のエンディングを解禁する
    public bool Kamigata = false;
    // Inspectorでチェック可能なフラグ。チェックされていると PressStart2 時に1つ目のエンディングを解禁する
    public bool Neru = false;
    // 新しいフラグ: Inspectorでチェックされていると PressStart 時に4つ目のエンディングを解禁する
    public bool Kami = false;

    public void PressStart()
    {
        // Kamigataフラグが有効なら3つ目のエンディングを解禁
        if (Kamigata)
        {
            PlayerPrefs.SetInt("Ending_3", 1);
            PlayerPrefs.Save();
            Debug.Log("Unlocked Ending_3 due to Kamigata flag.");
        }

        // Kamiフラグが有効なら4つ目のエンディングを解禁
        if (Kami)
        {
            PlayerPrefs.SetInt("Ending_4", 1);
            PlayerPrefs.Save();
            Debug.Log("Unlocked Ending_4 due to Kami flag.");
        }

        SceneManager.LoadScene(scene);
    }

    public void PressStart2()
    {
        // Neruフラグが有効なら一つ目のエンディングを解禁
        if (Neru)
        {
            PlayerPrefs.SetInt("Ending_1", 1);
            PlayerPrefs.Save();
            Debug.Log("Unlocked Ending_1 due to Neru flag.");
        }
        SceneManager.LoadScene(scene2);
    }

    public void PressStart3()
    {
        SceneManager.LoadScene(scene3);
    }
}
