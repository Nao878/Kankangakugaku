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
    [System.Serializable]
    public class Choice
    {
        public int timingIndex; // 選択肢を出すセリフのインデックス
        public string choice1Text;
        public string choice2Text;
        public Message[] branch1;
        public Message[] branch2;
    }

    [SerializeField] private Text textBox; // テキスト欄（UIのTextを割り当て）
    [SerializeField] private float charInterval = 0.1f; // 1文字ごとの間隔（秒）
    [SerializeField] private Button nextButton; // 次のセリフ表示用ボタン
    [SerializeField] private GameObject[] characterObjects; // キャラごとの表示オブジェクト（配列でInspectorに割り当て）
    [Header("選択肢ボタン")]
    [SerializeField] private Button choiceButton1; // 選択肢ボタン1
    [SerializeField] private Button choiceButton2; // 選択肢ボタン2
    [SerializeField] private Text choiceButton1Text;
    [SerializeField] private Text choiceButton2Text;

    // メインシナリオ
    private Message[] messages = {
        new Message { characterId = 0, text = "んん？これって..." },
        new Message { characterId = 0, text = "ごまに関する商品多いな..." },
        new Message { characterId = 0, text = "これは...この脱出ゲームのヒントかな..." },
        new Message { characterId = 0, text = "お前何かわかるか？" }
    };
    // 最初の選択肢
    private Choice[] choices = {
        new Choice {
            timingIndex = 3, // 4つ目のセリフの後
            choice1Text = "呪文に関係してるのかも",
            choice2Text = "ごま食べろってことじゃない？",
            branch1 = new Message[] {
                new Message { characterId = 0, text = "呪文か...何かヒントがあるかもな。" },
                new Message { characterId = 0, text = "もう少し調べてみよう。" }
            },
            branch2 = new Message[] {
                new Message { characterId = 0, text = "ごま食べろってことか？" },
                new Message { characterId = 0, text = "それはちょっと違う気がするけど..." }
            }
        }
    };

    private int currentIndex = 0;
    private Coroutine typeCoroutine;
    private bool isTyping = false;
    private Message[] currentMessages;
    private string currentFullText = "";
    private int choiceStep = 0;

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
        currentFullText = currentMessages[index].text;
        typeCoroutine = StartCoroutine(TypeText(currentFullText));
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
        if (isTyping)
        {
            // タイピング中なら全文表示してコルーチン停止
            if (typeCoroutine != null)
            {
                StopCoroutine(typeCoroutine);
                typeCoroutine = null;
            }
            textBox.text = currentFullText;
            isTyping = false;
            return;
        }
        // 選択肢タイミングか？
        if (choiceStep < choices.Length && currentMessages == messages && currentIndex == choices[choiceStep].timingIndex)
        {
            ShowChoice(choices[choiceStep]);
            return;
        }
        if (currentIndex < currentMessages.Length - 1)
        {
            currentIndex++;
            ShowMessage(currentIndex);
        }
    }

    private void ShowChoice(Choice choice)
    {
        if (choiceButton1 != null && choiceButton1Text != null)
        {
            choiceButton1Text.text = choice.choice1Text;
            choiceButton1.gameObject.SetActive(true);
        }
        if (choiceButton2 != null && choiceButton2Text != null)
        {
            choiceButton2Text.text = choice.choice2Text;
            choiceButton2.gameObject.SetActive(true);
        }
        if (nextButton != null) nextButton.gameObject.SetActive(false);
    }

    private void OnChoice1()
    {
        StartBranch(choices[choiceStep].branch1);
    }
    private void OnChoice2()
    {
        StartBranch(choices[choiceStep].branch2);
    }
    private void StartBranch(Message[] branch)
    {
        currentMessages = branch;
        currentIndex = 0;
        choiceStep++;
        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        ShowMessage(currentIndex);
    }
}