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
        public string choice3Text; // 3択用
        public Message[] branch1;
        public Message[] branch2;
        public Message[] branch3; // 3択用
    }

    [SerializeField] private Text textBox; // テキスト欄（UIのTextを割り当て）
    [SerializeField] private float charInterval = 0.1f; // 1文字ごとの間隔（秒）
    [SerializeField] private Button nextButton; // 次のセリフ表示用ボタン
    [SerializeField] private GameObject[] characterObjects; // キャラごとの表示オブジェクト（配列でInspectorに割り当て）
    [Header("選択肢ボタン")]
    [SerializeField] private Button choiceButton1; // 選択肢ボタン1
    [SerializeField] private Button choiceButton2; // 選択肢ボタン2
    [SerializeField] private Button choiceButton3; // 選択肢ボタン3
    [SerializeField] private Text choiceButton1Text;
    [SerializeField] private Text choiceButton2Text;
    [SerializeField] private Text choiceButton3Text;

    // メインシナリオ
    private Message[] messages = {
        new Message { characterId = 0, text = "んん？これって..." },
        new Message { characterId = 0, text = "ごまに関する商品多いな..." },
        new Message { characterId = 0, text = "これは...この脱出ゲームのヒントかな..." },
        new Message { characterId = 0, text = "お前何かわかるか？" }
    };
    // 選択肢
    private Choice[] choices = {
        // 1回目の選択肢
        new Choice {
            timingIndex = 3, // 4つ目のセリフの後
            choice1Text = "呪文に関係してるのかも",
            choice2Text = "ごま食べろってことじゃない？",
            branch1 = new Message[] {
                new Message { characterId = 0, text = "呪文か...何かヒントがあるかもな。" },
                new Message { characterId = 0, text = "もう少し調べてみよう。" },
                new Message { characterId = 0, text = "この後どうするんだ？" }
            },
            branch2 = new Message[] {
                new Message { characterId = 0, text = "ごま食べろってことか？" },
                new Message { characterId = 0, text = "それはちょっと違う気がするけど..." },
                new Message { characterId = 0, text = "この後どうするんだ？" }
            }
        },
        // 2回目の選択肢（分岐後の最後のセリフ後）
        new Choice {
            timingIndex = 2, // 分岐後の3つ目（index=2）
            choice1Text = "屋上の出入り口前に行く",
            choice2Text = "早乙女と話す",
            choice3Text = "自販機の前に行く",
            branch1 = new Message[] {
                new Message { characterId = -1, text = "屋上の出入り口前に向かった。" },
                new Message { characterId = -1, text = "呪文を唱えれば開くタイプのドア、童話にあったな" }
            },
            branch2 = new Message[] {
                new Message { characterId = -1, text = "早乙女と話し始めた。" },
                new Message { characterId = 0, text = "こいつの話はおもんない" }
            },
            branch3 = new Message[] {
                new Message { characterId = -1, text = "自販機の前に行った。" },
                new Message { characterId = -1, text = "ごま、なんか呪文あった気がする" }
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
        if (choiceButton3 != null)
        {
            choiceButton3.onClick.AddListener(OnChoice3);
            choiceButton3.gameObject.SetActive(false);
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
        int charId = currentMessages[index].characterId;
        // キャラ表示切替
        for (int i = 0; i < characterObjects.Length; i++)
        {
            if (characterObjects[i] != null)
                characterObjects[i].SetActive(charId != -1 && i == charId);
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
        if (choiceStep < choices.Length && currentIndex == choices[choiceStep].timingIndex)
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
        if (!string.IsNullOrEmpty(choice.choice3Text) && choiceButton3 != null && choiceButton3Text != null)
        {
            choiceButton3Text.text = choice.choice3Text;
            choiceButton3.gameObject.SetActive(true);
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
    private void OnChoice3()
    {
        StartBranch(choices[choiceStep].branch3);
    }
    private void StartBranch(Message[] branch)
    {
        currentMessages = branch;
        currentIndex = 0;
        choiceStep++;
        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);
        if (choiceButton3 != null) choiceButton3.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        ShowMessage(currentIndex);
    }
}