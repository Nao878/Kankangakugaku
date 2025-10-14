using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class gakkou6Novel : MonoBehaviour
{
    [SerializeField] private Text textBox; // テキスト欄（UIのTextを割り当て）
    [SerializeField] private float interval = 2f; // セリフ間隔（秒）

    private string[] messages = {
        "こんにちは！",
        "今日はいい天気ですね。",
        "冒険に出かけましょう！",
        "準備はできていますか？"
    };

    private int currentIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textBox != null)
        {
            StartCoroutine(TalkCoroutine());
        }
    }

    private IEnumerator TalkCoroutine()
    {
        while (currentIndex < messages.Length)
        {
            textBox.text = messages[currentIndex];
            currentIndex++;
            yield return new WaitForSeconds(interval);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
