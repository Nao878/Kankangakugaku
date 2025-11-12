using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class gakkou6Novel : MonoBehaviour
{
    [System.Serializable]
    public class Message
    {
        public int characterId;
        public string text;
    }
    [System.Serializable]
    public class Choice
    {
        public int timingIndex;
        public string choice1Text;
        public string choice2Text;
        public string choice3Text;
        public string choice4Text;
        public Message[] branch1;
        public Message[] branch2;
        public Message[] branch3;
        public Message[] branch4;
    }

    [SerializeField] private Text textBox;
    [SerializeField] private float charInterval = 0.1f;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject[] characterObjects;
    [Header("選択肢ボタン")]
    [SerializeField] private Button choiceButton1;
    [SerializeField] private Button choiceButton2;
    [SerializeField] private Button choiceButton3;
    [SerializeField] private Button choiceButton4;
    [SerializeField] private Text choiceButton1Text;
    [SerializeField] private Text choiceButton2Text;
    [SerializeField] private Text choiceButton3Text;
    [SerializeField] private Text choiceButton4Text;
    [Header("呪文入力欄")]
    [SerializeField] private InputField spellInputField;

    private Message[] messages = {
        new Message { characterId = 0, text = "んん？これって..." },
        new Message { characterId = 0, text = "ごまに関する商品多いな..." },
        new Message { characterId = 0, text = "これは...この脱出ゲームのヒントかな..." },
        new Message { characterId = 0, text = "お前何かわかるか？" }
    };

    private Message[] branch1 = {
        new Message { characterId = -1, text = "屋上の出入り口前に向かった。" },
        new Message { characterId = 0, text = "呪文を唱えれば開くタイプのドア、童話にあったな" }
    };
    private Message[] branch2 = {
        new Message { characterId = -1, text = "早乙女と話し始めた。" },
        new Message { characterId = 0, text = "こいつの話はおもんない" }
    };
    private Message[] branch3 = {
        new Message { characterId = -1, text = "自販機の前に行った。" },
        new Message { characterId = -1, text = "ごま、なんか呪文あった気がする" }
    };
    private Message[] branch4 = {
        new Message { characterId = -1, text = "ひらめいた！" },
        new Message { characterId = 0, text = "何か思いついたか？" },
        new Message { characterId = -1, text = "ドアの前で呪文を唱えれば良いんだ！" },
        new Message { characterId = -1, text = "※唱える呪文をひらがな入力してください" }
    };
    private Message[] spellSuccess = {
        new Message { characterId = 0, text = "ドアが開いた！" }
    };

    private int currentIndex = 0;
    private Coroutine typeCoroutine;
    private bool isTyping = false;
    private Message[] currentMessages;
    private string currentFullText = "";

    private bool[] branchSelected = new bool[3];
    private int branchCount = 0;
    private bool showChoiceLoop = false;

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
        if (choiceButton4 != null)
        {
            choiceButton4.onClick.AddListener(OnChoice4);
            choiceButton4.gameObject.SetActive(false);
        }
        if (spellInputField != null)
        {
            spellInputField.gameObject.SetActive(false);
            spellInputField.onEndEdit.AddListener(OnSpellInputEnd);
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
        // 呪文入力欄表示判定
        if (spellInputField != null)
        {
            if (currentFullText == "※唱える呪文をひらがな入力してください")
            {
                spellInputField.text = "";
                spellInputField.gameObject.SetActive(true);
                if (nextButton != null) nextButton.gameObject.SetActive(false);
            }
            else
            {
                spellInputField.gameObject.SetActive(false);
            }
        }
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
        // 最初の選択肢タイミング
        if (!showChoiceLoop && currentMessages == messages && currentIndex == messages.Length - 1)
        {
            ShowChoiceLoop();
            return;
        }
        // 3択分岐のセリフを読み終えたら再び3択
        if (showChoiceLoop && currentIndex == currentMessages.Length - 1)
        {
            branchCount++;
            if (branchCount < 3)
            {
                ShowChoiceLoop();
                return;
            }
            else
            {
                ShowFinalChoice();
                return;
            }
        }
        if (currentIndex < currentMessages.Length - 1)
        {
            currentIndex++;
            ShowMessage(currentIndex);
        }
    }

    // 3択選択肢表示
    private void ShowChoiceLoop()
    {
        showChoiceLoop = true;
        if (choiceButton1 != null && choiceButton1Text != null)
        {
            choiceButton1Text.text = "屋上の出入り口前に行く";
            choiceButton1.gameObject.SetActive(!branchSelected[0]);
        }
        if (choiceButton2 != null && choiceButton2Text != null)
        {
            choiceButton2Text.text = "早乙女と話す";
            choiceButton2.gameObject.SetActive(!branchSelected[1]);
        }
        if (choiceButton3 != null && choiceButton3Text != null)
        {
            choiceButton3Text.text = "自販機の前に行く";
            choiceButton3.gameObject.SetActive(!branchSelected[2]);
        }
        if (nextButton != null) nextButton.gameObject.SetActive(false);
    }

    // 4つ目の「ひらめいた！」ボタン表示
    private void ShowFinalChoice()
    {
        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);
        if (choiceButton3 != null) choiceButton3.gameObject.SetActive(false);
        if (choiceButton4 != null && choiceButton4Text != null)
        {
            choiceButton4Text.text = "ひらめいた！";
            choiceButton4.gameObject.SetActive(true);
        }
        if (nextButton != null) nextButton.gameObject.SetActive(false);
    }

    private void OnChoice1()
    {
        branchSelected[0] = true;
        currentMessages = branch1;
        currentIndex = 0;
        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);
        if (choiceButton3 != null) choiceButton3.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        ShowMessage(currentIndex);
    }
    private void OnChoice2()
    {
        branchSelected[1] = true;
        currentMessages = branch2;
        currentIndex = 0;
        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);
        if (choiceButton3 != null) choiceButton3.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        ShowMessage(currentIndex);
    }
    private void OnChoice3()
    {
        branchSelected[2] = true;
        currentMessages = branch3;
        currentIndex = 0;
        if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
        if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);
        if (choiceButton3 != null) choiceButton3.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        ShowMessage(currentIndex);
    }
    private void OnChoice4()
    {
        currentMessages = branch4;
        currentIndex = 0;
        if (choiceButton4 != null) choiceButton4.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(true);
        ShowMessage(currentIndex);
    }
    private void OnSpellInputEnd(string input)
    {
        if (spellInputField != null) spellInputField.gameObject.SetActive(false);
        if (input.Trim() == "ひらけごま")
        {
            currentMessages = spellSuccess;
            currentIndex = 0;
            if (nextButton != null) nextButton.gameObject.SetActive(true);
            ShowMessage(currentIndex);
        }
        else
        {
            // 間違いなら即座にエラーメッセージ表示し、再度入力欄を表示
            if (typeCoroutine != null)
            {
                StopCoroutine(typeCoroutine);
                typeCoroutine = null;
            }
            textBox.text = "呪文が違うようだ...";
            if (spellInputField != null)
            {
                spellInputField.text = "";
                spellInputField.gameObject.SetActive(true);
            }
        }
    }
}