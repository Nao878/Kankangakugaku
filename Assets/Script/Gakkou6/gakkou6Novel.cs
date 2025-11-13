using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class gakkou6Novel : MonoBehaviour
{
    [SerializeField] private string csvFileName = "dialogue"; // Resources/dialogue.csv
    [SerializeField] private Text textBox;
    [SerializeField] private float charInterval = 0.1f;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject[] characterObjects;
    [Header("選択肢ボタン")]
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Text[] choiceButtonTexts;
    [Header("呪文入力欄")]
    [SerializeField] private InputField spellInputField;

    private class DialogueEntry
    {
        public int index;
        public int characterId;
        public string text;
        public bool isChoice;
        public string[] choices;
        public int[] nextIndices;
        public bool isSpellInput;
        public int spellSuccessIndex;
        // ループ選択肢用
        public bool isLoopChoice;
        public int loopChoiceCount;
        public string finalChoiceText;
        public int finalChoiceIndex;
    }

    private Dictionary<int, DialogueEntry> dialogueDict = new Dictionary<int, DialogueEntry>();
    private DialogueEntry currentEntry;
    private Coroutine typeCoroutine;
    private bool isTyping = false;
    // ループ選択肢履歴
    private bool[] loopChoiceSelected;
    private bool loopChoiceActive = false;

    void Start()
    {
        LoadDialogueCSV();
        if (dialogueDict.ContainsKey(0))
        {
            currentEntry = dialogueDict[0];
            ShowEntry(currentEntry);
        }
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

    private void LoadDialogueCSV()
    {
        var textAsset = Resources.Load<TextAsset>(csvFileName);
        if (textAsset == null)
        {
            Debug.LogError($"CSV file not found: Resources/{csvFileName}.csv");
            return;
        }
        var lines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < lines.Length; i++) // skip header
        {
            var cols = lines[i].Split(',');
            var entry = new DialogueEntry();
            entry.index = int.Parse(cols[0]);
            entry.characterId = int.Parse(cols[1]);
            entry.text = cols[2];
            entry.isChoice = cols[3] == "1";
            entry.choices = new string[4];
            for (int c = 0; c < 4; c++)
                entry.choices[c] = cols[4 + c];
            entry.nextIndices = new int[4];
            for (int n = 0; n < 4; n++)
                entry.nextIndices[n] = string.IsNullOrEmpty(cols[8 + n]) ? -1 : int.Parse(cols[8 + n]);
            entry.isSpellInput = cols.Length > 12 && cols[12] == "1";
            entry.spellSuccessIndex = cols.Length > 13 && !string.IsNullOrEmpty(cols[13]) ? int.Parse(cols[13]) : -1;
            entry.isLoopChoice = cols.Length > 14 && cols[14] == "1";
            entry.loopChoiceCount = cols.Length > 15 && !string.IsNullOrEmpty(cols[15]) ? int.Parse(cols[15]) : 0;
            entry.finalChoiceText = cols.Length > 16 ? cols[16] : "";
            entry.finalChoiceIndex = cols.Length > 17 && !string.IsNullOrEmpty(cols[17]) ? int.Parse(cols[17]) : -1;
            dialogueDict[entry.index] = entry;
        }
    }

    private void ShowEntry(DialogueEntry entry)
    {
        if (typeCoroutine != null)
            StopCoroutine(typeCoroutine);
        // キャラ表示
        for (int i = 0; i < characterObjects.Length; i++)
            if (characterObjects[i] != null)
                characterObjects[i].SetActive(entry.characterId != -1 && i == entry.characterId);
        typeCoroutine = StartCoroutine(TypeText(entry.text));
        // 選択肢表示
        if (entry.isChoice)
        {
            // ループ選択肢の場合
            if (entry.isLoopChoice)
            {
                if (loopChoiceSelected == null || loopChoiceSelected.Length != entry.loopChoiceCount)
                    loopChoiceSelected = new bool[entry.loopChoiceCount];
                loopChoiceActive = true;
                int activeCount = 0;
                for (int i = 0; i < entry.loopChoiceCount; i++)
                {
                    if (!loopChoiceSelected[i])
                    {
                        choiceButtonTexts[i].text = entry.choices[i];
                        choiceButtons[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        choiceButtons[i].gameObject.SetActive(false);
                        activeCount++;
                    }
                }
                // 全て選択済みなら4つ目の選択肢表示
                if (activeCount == entry.loopChoiceCount && !string.IsNullOrEmpty(entry.finalChoiceText))
                {
                    choiceButtonTexts[entry.loopChoiceCount].text = entry.finalChoiceText;
                    choiceButtons[entry.loopChoiceCount].gameObject.SetActive(true);
                }
                else if (choiceButtons.Length > entry.loopChoiceCount)
                {
                    choiceButtons[entry.loopChoiceCount].gameObject.SetActive(false);
                }
                if (nextButton != null) nextButton.gameObject.SetActive(false);
            }
            else
            {
                for (int i = 0; i < choiceButtons.Length; i++)
                {
                    if (i < entry.choices.Length && !string.IsNullOrEmpty(entry.choices[i]))
                    {
                        choiceButtonTexts[i].text = entry.choices[i];
                        choiceButtons[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        choiceButtons[i].gameObject.SetActive(false);
                    }
                }
                if (nextButton != null) nextButton.gameObject.SetActive(false);
            }
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
            if (entry.isSpellInput)
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
            textBox.text = currentEntry.text;
            isTyping = false;
            return;
        }
        if (currentEntry.nextIndices != null && currentEntry.nextIndices.Length > 0 && currentEntry.nextIndices[0] != -1)
        {
            currentEntry = dialogueDict[currentEntry.nextIndices[0]];
            ShowEntry(currentEntry);
        }
    }

    private void OnChoice(int idx)
    {
        // ループ選択肢の場合
        if (loopChoiceActive && currentEntry.isLoopChoice)
        {
            if (idx < currentEntry.loopChoiceCount && !loopChoiceSelected[idx])
            {
                loopChoiceSelected[idx] = true;
                currentEntry = dialogueDict[currentEntry.nextIndices[idx]];
                ShowEntry(currentEntry);
            }
            // 4つ目の選択肢（全て選択済み）
            else if (idx == currentEntry.loopChoiceCount && AllLoopChoicesSelected())
            {
                currentEntry = dialogueDict[currentEntry.finalChoiceIndex];
                loopChoiceActive = false;
                ShowEntry(currentEntry);
            }
        }
        else
        {
            if (currentEntry.nextIndices != null && idx < currentEntry.nextIndices.Length && currentEntry.nextIndices[idx] != -1)
            {
                currentEntry = dialogueDict[currentEntry.nextIndices[idx]];
                ShowEntry(currentEntry);
            }
        }
    }

    private bool AllLoopChoicesSelected()
    {
        if (loopChoiceSelected == null) return false;
        for (int i = 0; i < loopChoiceSelected.Length; i++)
            if (!loopChoiceSelected[i]) return false;
        return true;
    }

    private void OnSpellInputEnd(string input)
    {
        spellInputField.gameObject.SetActive(false);
        if (input.Trim() == "ひらけごま" && currentEntry.spellSuccessIndex != -1)
        {
            currentEntry = dialogueDict[currentEntry.spellSuccessIndex];
            ShowEntry(currentEntry);
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