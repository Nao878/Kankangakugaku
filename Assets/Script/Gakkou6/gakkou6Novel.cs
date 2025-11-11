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
    [Header("選択肢ボタン")]
    [SerializeField] private Button choiceButton1; // 選択肢ボタン1
    [SerializeField] private Button choiceButton2; // 選択肢ボタン2

    // メインシナリオ
    private Message[] messages = {
        new Message { characterId = 0, text = "んん？これって..." },
        new Message { characterId = 0, text = "ごまに関する商品多いな..." },
        new Message { characterId = 0, text = "これは...この脱出ゲームのヒントかな..." },
        new Message { characterId = 0, text = "お前何かわかるか？" }
    };
    // 選択肢1の展開
    private Message[] branch1 = {
        new Message { characterId = 0, text = "ボタン1を選んだんだな。" },
        new Message { characterId = 0, text = "よし、次に進もう！" }
    };
    // 選択肢2の展開
    private Message[] branch2 = {
        new Message { characterId = 0, text = "ボタン2を選んだのか。" },
        new Message { characterId = 0, text = "慎重に進もう。" }
    };

    private int currentIndex = 0;
    private Coroutine typeCoroutine;
    private bool isTyping = false;
    private Message[] currentMessages;

    void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextMessage);
        }
        if (choiceButton1 != null)
        {
            choiceButton1.onClick.AddListener(OnChoice1);
            choiceButton1.gameObject.SetActive(false);
        }
        if (choiceButton2 != null)
        {
            choiceButton2.onClick.AddListener(OnChoice2);
            choiceButton2.gameObject.SetActive(false);
        }
        currentMessages = messages;
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
                characterObjects[i].SetActive(i == currentMessages[index].characterId);
        }
        typeCoroutine = StartCoroutine(TypeText(currentMessages[index].text));
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
        if (currentIndex < currentMessages.Length - 1)
        {
            currentIndex++;
            ShowMessage(currentIndex);
        }
        else if (currentMessages == messages) // メインシナリオ終了時
        {
            if (choiceButton1 != null) choiceButton1.gameObject.SetActive(true);
            if (choiceButton2 != null) choiceButton2.gameObject.SetActive(true);
            if (nextButton != null) nextButton.gameObject.SetActive(false);
        }
    }

    private void OnChoice1()
    {
        StartBranch(branch1);
    }
    private void OnChoice2()
    {
        StartBranch(branch2);
    }
    private void StartBranch(Message[] branch)
    {
        currentMessages = branch;
        currentIndex = 0;
        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        ShowMessage(currentIndex);
    }
}
