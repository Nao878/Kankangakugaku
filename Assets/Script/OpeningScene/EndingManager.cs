using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// エンディング達成状況を管理し、見たエンディングだけテキストオブジェクトをアクティブにする
/// </summary>
public class EndingManager : MonoBehaviour
{
    [System.Serializable]
    public class EndingInfo
    {
        public string endingName;         // エンディング名（ユニーク）
        public GameObject endingTextObj;  // エンディング説明用テキストオブジェクト
    }

    [Header("エンディング一覧（Inspectorで設定）")]
    public List<EndingInfo> endings;

    [Header("テスト設定")]
    [Tooltip("チェックすると Start 時に PlayerPrefs をリセットします（テスト用）")]
    public bool resetPlayerPrefsOnStart = false;

    void Start()
    {
        // テスト時にPlayerPrefsをリセットするオプション
        if (resetPlayerPrefsOnStart)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs deleted by EndingManager (resetPlayerPrefsOnStart=true)");
        }

        foreach (var ending in endings)
        {
            // PlayerPrefsで達成済みならアクティブ、未達成なら非表示
            bool seen = PlayerPrefs.GetInt("Ending_" + ending.endingName, 0) == 1;
            if (ending.endingTextObj != null)
                ending.endingTextObj.SetActive(seen);
        }
    }

    /// <summary>
    /// エンディング達成時に呼び出す（他のスクリプトから）
    /// </summary>
    public void MarkEndingAsSeen(string endingName)
    {
        PlayerPrefs.SetInt("Ending_" + endingName, 1);
        PlayerPrefs.Save();
        // 該当テキストオブジェクトをアクティブに
        foreach (var ending in endings)
        {
            if (ending.endingName == endingName && ending.endingTextObj != null)
                ending.endingTextObj.SetActive(true);
        }
    }

    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}