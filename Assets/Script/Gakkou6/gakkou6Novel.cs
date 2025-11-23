using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// ノベルゲーム風の会話・選択肢・背景切替・呪文入力をCSVで管理するスクリプト
public class gakkou6Novel : MonoBehaviour
{
    // CSVファイル名（Resources/dialogue.csv）
    [SerializeField] private string csvFileName = "dialogue";
    // セリフ表示用テキスト
    [SerializeField] private Text textBox;
    // 1文字ごとの表示間隔
    [SerializeField] private float charInterval = 0.1f;
    // 次のセリフへ進むボタン
    [SerializeField] private Button nextButton;
    // キャラクター画像オブジェクト配列（キャラID順）
    [SerializeField] private GameObject[] characterObjects;
    // 選択肢ボタン配列
    [Header("選択肢ボタン")]
    [SerializeField] private Button[] choiceButtons;
    // 選択肢ボタンのテキスト配列
    [SerializeField] private Text[] choiceButtonTexts;
    // 呪文入力欄
    [Header("呪文入力欄")]
    [SerializeField] private InputField spellInputField;
    // 背景画像表示用Image
    [Header("背景画像表示用")]
    [SerializeField] private Image backgroundImage;
    // 背景切替時に非表示にするオブジェクト
    [Header("背景切替時に非表示にするオブジェクト")]
    [SerializeField] private GameObject hideOnBackgroundChange;
    // オーディオソース（BGM/効果音などを再生する）
    [Header("音声再生用 AudioSource（未設定なら自動作成）")]
    [SerializeField] private AudioSource audioSource;

    // CSV1行分のデータ構造
    private class DialogueEntry
    {
        public int index; // セリフID
        public int characterId; // キャラID
        public string text; // セリフ本文
        public bool isChoice; // 選択肢かどうか
        public string[] choices; // 選択肢テキスト
        public int[] nextIndices; // 選択肢ごとの分岐先ID
        public bool isSpellInput; // 呪文入力欄表示
        public int spellSuccessIndex; // 呪文正解時の分岐先ID（1つめ）
        // 追加の呪文回答（2つめ）とその分岐先
        public string spellAnswer2; // 2つめの正解ワード
        public int spellSuccessIndex2; // 2つめの正解ワードの分岐先ID
        // 追加の呪文回答（3つめ）とその分岐先
        public string spellAnswer3; // 3つめの正解ワード
        public int spellSuccessIndex3; // 3つめの正解ワードの分岐先ID
        // ループ選択肢用
        public bool isLoopChoice; // ループ選択肢か
        public int loopChoiceCount; // ループ選択肢の数
        public string finalChoiceText; // 4つ目の選択肢テキスト
        public int finalChoiceIndex; // 4つ目の分岐先ID
        // 背景画像名（Resources内Sprite名）
        public string backgroundImageName;
        // シーン名（セリフ終了後に読み込む）
        public string sceneName;
        // 呪文入力欄の正解ワード（1つめ）
        public string spellAnswer;
        // エンディング解禁用の名前（CSVの新列）
        public string unlockEndingName;
        // 新しい列: 再生する音源ファイル名（Resources内のAudioClip名）
        public string audioName;
    }

    // 全会話データ（index→DialogueEntry）
    private Dictionary<int, DialogueEntry> dialogueDict = new Dictionary<int, DialogueEntry>();
    // 現在表示中の会話データ
    private DialogueEntry currentEntry;
    // タイピングコルーチン
    private Coroutine typeCoroutine;
    // タイピング中フラグ
    private bool isTyping = false;
    // ループ選択肢履歴
    private bool[] loopChoiceSelected;
    // ループ選択肢表示中フラグ
    private bool loopChoiceActive = false;
    // 背景画像切替済みフラグ
    private bool backgroundChangedOnce = false;

    // 初期化処理
    void Start()
    {
        LoadDialogueCSV(); // CSV読み込み
        if (dialogueDict.ContainsKey(0))
        {
            currentEntry = dialogueDict[0]; // 最初のセリフ
            ShowEntry(currentEntry);
        }
        if (nextButton != null)
            nextButton.onClick.AddListener(OnNext);
        if (spellInputField != null)
        {
            spellInputField.gameObject.SetActive(false);
            spellInputField.onEndEdit.AddListener(OnSpellInputEnd);
        }
        // 選択肢ボタン初期化
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int idx = i;
            choiceButtons[i].onClick.AddListener(() => OnChoice(idx));
            choiceButtons[i].gameObject.SetActive(false);
        }
        // 背景画像は最初非表示
        if (backgroundImage != null)
            backgroundImage.gameObject.SetActive(false);

        // AudioSourceが未設定なら追加して初期化
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }
    }

    // CSVファイル読み込み・パース
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
            entry.backgroundImageName = cols.Length > 18 ? cols[18] : "";
            entry.sceneName = cols.Length > 19 ? cols[19] : "";
            // 既存のspellAnswer（1つめ）
            entry.spellAnswer = cols.Length > 20 ? cols[20] : "";
            // 追加のspellAnswer（2つめ）とその成功Index（任意の列がCSVに追加されている場合に読み込む）
            entry.spellAnswer2 = cols.Length > 21 ? cols[21] : "";
            entry.spellSuccessIndex2 = cols.Length > 22 && !string.IsNullOrEmpty(cols[22]) ? int.Parse(cols[22]) : -1;
            // さらに追加のspellAnswer（3つめ）とその成功Index
            entry.spellAnswer3 = cols.Length > 23 ? cols[23] : "";
            entry.spellSuccessIndex3 = cols.Length > 24 && !string.IsNullOrEmpty(cols[24]) ? int.Parse(cols[24]) : -1;
            // 新しい列: エンディング解禁用の名前
            entry.unlockEndingName = cols.Length > 25 ? cols[25].Trim() : "";
            // 新しい列: 音源ファイル名（Resources内のAudioClip名）
            entry.audioName = cols.Length > 26 ? cols[26].Trim() : "";
            dialogueDict[entry.index] = entry;
        }
    }

    // セリフ・選択肢・背景画像・呪文入力の表示処理
    private void ShowEntry(DialogueEntry entry)
    {
        if (typeCoroutine != null)
            StopCoroutine(typeCoroutine);
        // キャラ画像表示
        for (int i = 0; i < characterObjects.Length; i++)
            if (characterObjects[i] != null)
                characterObjects[i].SetActive(entry.characterId != -1 && i == entry.characterId);
        typeCoroutine = StartCoroutine(TypeText(entry.text));
        // 背景画像切り替え（初回のみhideOnBackgroundChangeを非表示）
        if (backgroundImage != null && !string.IsNullOrEmpty(entry.backgroundImageName))
        {
            var sprite = Resources.Load<Sprite>(entry.backgroundImageName);
            if (sprite != null)
            {
                backgroundImage.sprite = sprite;
                if (!backgroundChangedOnce)
                {
                    backgroundImage.gameObject.SetActive(true);
                    if (hideOnBackgroundChange != null)
                        hideOnBackgroundChange.SetActive(false);
                    backgroundChangedOnce = true;
                }
            }
        }

        // 新しい機能: 行にunlockEndingNameがあれば、そのエンディングを解禁
        if (!string.IsNullOrEmpty(entry.unlockEndingName))
        {
            var key = "Ending_" + entry.unlockEndingName;
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
            Debug.Log($"Unlocked ending: {entry.unlockEndingName}");
        }

        // 新しい機能: 行にaudioNameがあれば現在の音源を停止して新しい音源を再生
        if (!string.IsNullOrEmpty(entry.audioName))
        {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
            var clip = Resources.Load<AudioClip>(entry.audioName);
            if (clip != null)
            {
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                }
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"Audio clip not found in Resources: {entry.audioName}");
            }
        }

        // シーン遷移（sceneNameが指定されていれば）
        if (!string.IsNullOrEmpty(entry.sceneName))
        {
            SceneManager.LoadScene(entry.sceneName);
            return;
        }
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

    // セリフを1文字ずつ表示するコルーチン
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

    // 次のセリフへ進む処理
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

    // 選択肢ボタン押下時の処理
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

    // ループ選択肢が全て選択済みか判定
    private bool AllLoopChoicesSelected()
    {
        if (loopChoiceSelected == null) return false;
        for (int i = 0; i < loopChoiceSelected.Length; i++)
            if (!loopChoiceSelected[i]) return false;
        return true;
    }

    // 呪文入力欄の入力完了時処理
    private void OnSpellInputEnd(string input)
    {
        spellInputField.gameObject.SetActive(false);
        // 入力を正規化（前後の空白を除き、大文字小文字を区別しない）
        var trimmed = input.Trim();
        var normalized = trimmed; // そのままの比較だが、必要ならToLower()等で正規化

        // spellAnswer列で指定された正解ワードと比較
        if (!string.IsNullOrEmpty(currentEntry.spellAnswer) && normalized == currentEntry.spellAnswer && currentEntry.spellSuccessIndex != -1)
        {
            currentEntry = dialogueDict[currentEntry.spellSuccessIndex];
            ShowEntry(currentEntry);
            return;
        }
        // 追加の正解ワードがある場合はそちらもチェック
        if (!string.IsNullOrEmpty(currentEntry.spellAnswer2) && normalized == currentEntry.spellAnswer2 && currentEntry.spellSuccessIndex2 != -1)
        {
            currentEntry = dialogueDict[currentEntry.spellSuccessIndex2];
            ShowEntry(currentEntry);
            return;
        }
        // 3つ目の正解ワードがある場合はそちらもチェック
        if (!string.IsNullOrEmpty(currentEntry.spellAnswer3) && normalized == currentEntry.spellAnswer3 && currentEntry.spellSuccessIndex3 != -1)
        {
            currentEntry = dialogueDict[currentEntry.spellSuccessIndex3];
            ShowEntry(currentEntry);
            return;
        }

        // 間違いなら即座にエラーメッセージ表示し、再度入力欄を表示
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
            typeCoroutine = null;
        }
        textBox.text = "答えが違うようだ...";
        spellInputField.text = "";
        spellInputField.gameObject.SetActive(true);
    }
}