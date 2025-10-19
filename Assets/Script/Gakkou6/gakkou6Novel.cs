using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class gakkou6Novel : MonoBehaviour
{
    [System.Serializable]
    public class Message
    {
        public int characterId; // キャラID（0:キャラ1, 1:キャラ2, ...）
        public string text;
    }

    [SerializeField] private Text textBox; // テキスト欄（UIのTextを割り当て）
    [SerializeField] private float charInterval = 0.1f; // 1文字ごとの間隔（秒）
    [SerializeField] private Button nextButton; // 次のセリフ表示用ボタン
    [SerializeField] private GameObject[] characterObjects; // キャラごとの表示オブジェクト（配列でInspectorに割り当て）

    private Message[] messages = {
        new Message { characterId = 0, text = "んん？これって..." },
        new Message { characterId = 0, text = "ごまに関する商品多いな..." },
        new Message { characterId = 0, text = "これは...この脱出ゲームのヒントかな..." },
        new Message { characterId = 0, text = "お前何かわかるか？" }
    };

    private int currentIndex = 0;
    private Coroutine typeCoroutine;
    private bool isTyping = false;

    void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextMessage);
        }
        ShowMessage(currentIndex);
    }

    private void ShowMessage(int index)
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
        }
        // キャラ表示切替
        for (int i = 0; i < characterObjects.Length; i++)
        {
            if (characterObjects[i] != null)
                characterObjects[i].SetActive(i == messages[index].characterId);
        }
        typeCoroutine = StartCoroutine(TypeText(messages[index].text));
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        textBox.text = "";
        foreach (char c in message)
        {
            textBox.text += c;
            yield return new WaitForSeconds(charInterval);
        }
        isTyping = false;
    }

    public void NextMessage()
    {
        if (isTyping) return; // タイピング中は無視
        if (currentIndex < messages.Length - 1)
        {
            currentIndex++;
            ShowMessage(currentIndex);
        }
    }
}
