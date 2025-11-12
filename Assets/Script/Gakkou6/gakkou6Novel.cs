using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class gakkou6Novel : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData; // ScriptableObject参照
    [SerializeField] private Text textBox;
    [SerializeField] private float charInterval = 0.1f;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject[] characterObjects;
    [Header("選択肢ボタン")]
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Text[] choiceButtonTexts;
    [Header("呪文入力欄")]
    [SerializeField] private InputField spellInputField;

    private DialogueLine currentLine;
    private Coroutine typeCoroutine;
    private bool isTyping = false;

    void Start()
    {
        currentLine = dialogueData.startLine;
        ShowLine(currentLine);
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNext);
        if (spellInputField != null)
        {
            spellInputField.gameObject.SetActive(false);
            spellInputField.onEndEdit.AddListener(OnSpellInputEnd);
        }
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int idx = i;
            choiceButtons[i].onClick.AddListener(() => OnChoice(idx));
            choiceButtons[i].gameObject.SetActive(false);
        }
    }

    private void ShowLine(DialogueLine line)
    {
        if (typeCoroutine != null)
            StopCoroutine(typeCoroutine);

        // キャラ表示
        for (int i = 0; i < characterObjects.Length; i++)
            if (characterObjects[i] != null)
                characterObjects[i].SetActive(line.characterId != -1 && i == line.characterId);

        typeCoroutine = StartCoroutine(TypeText(line.text));

        // 選択肢表示
        if (line.isChoice && line.choices != null)
        {
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < line.choices.Length)
                {
                    choiceButtonTexts[i].text = line.choices[i];
                    choiceButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
            if (nextButton != null) nextButton.gameObject.SetActive(false);
        }
        else
        {
            foreach (var btn in choiceButtons)
                btn.gameObject.SetActive(false);
            if (nextButton != null) nextButton.gameObject.SetActive(true);
        }

        // 呪文入力欄表示
        if (spellInputField != null)
        {
            if (line.isSpellInput)
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

    private void OnNext()
    {
        if (isTyping)
        {
            if (typeCoroutine != null)
            {
                StopCoroutine(typeCoroutine);
                typeCoroutine = null;
            }
            textBox.text = currentLine.text;
            isTyping = false;
            return;
        }
        if (currentLine.nextLines != null && currentLine.nextLines.Length > 0)
        {
            currentLine = currentLine.nextLines[0];
            ShowLine(currentLine);
        }
    }

    private void OnChoice(int idx)
    {
        if (currentLine.nextLines != null && idx < currentLine.nextLines.Length)
        {
            currentLine = currentLine.nextLines[idx];
            ShowLine(currentLine);
        }
    }

    private void OnSpellInputEnd(string input)
    {
        spellInputField.gameObject.SetActive(false);
        if (input.Trim() == "ひらけごま")
        {
            if (currentLine.spellSuccessLine != null)
            {
                currentLine = currentLine.spellSuccessLine;
                ShowLine(currentLine);
            }
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
            spellInputField.text = "";
            spellInputField.gameObject.SetActive(true);
        }
    }
}