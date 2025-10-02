using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 屋上プレイヤーの操作・UI制御を行うクラス
public class OkuzyouPlay : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 1.8f; // プレイヤーの移動速度
    private Rigidbody2D rb; // プレイヤーのRigidbody2D
    private bool canMove = true; // 移動可能かどうか

    [Header("UI References")]
    [SerializeField] private GameObject messagePanel; // メッセージパネル
    [SerializeField] private GameObject aibouPic;     // 相棒の画像
    [SerializeField] private GameObject akanaiText;   // 開かないテキスト
    [SerializeField] private GameObject sikabaneText; // 屍テキスト
    [SerializeField] private GameObject zihankiText;  // 自販機テキスト

    [Header("Collision References")]
    [SerializeField] private GameObject finishObject;      // Finishオブジェクト
    [SerializeField] private GameObject shikabaneObject;   // ObShikabaneオブジェクト
    [SerializeField] private GameObject zihankiObject;     // ObjZihankiオブジェクト

    [Header("表示制御オブジェクト")]
    [SerializeField] private GameObject showObject; // トリガー中に表示するオブジェクト

    // 各オブジェクトとの接触状態を管理
    private bool isInFinish = false;    // Finishタグとの接触中
    private bool isInShikabane = false; // ObShikabaneとの接触中
    private bool isInZihanki = false;   // ObjZihankiとの接触中

    // 初期化処理
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D取得
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
        }
    }

    // プレイヤーの移動処理（物理演算）
    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero; // 移動不可時は停止
            return;
        }

        Vector2 input = Vector2.zero;
        // 矢印キー入力で移動方向を決定
        if (Input.GetKey(KeyCode.UpArrow)) input.y += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) input.y -= 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) input.x -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) input.x += 1f;

        rb.linearVelocity = input.normalized * speed; // 速度を設定
    }

    // 毎フレームの入力・UI制御
    private void Update()
    {
        // Enterキーで各オブジェクトに応じたメッセージ表示
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isInFinish)
            {
                akanaiText.SetActive(true);
                sikabaneText.SetActive(false);
                zihankiText.SetActive(false);
                ShowPanel();
            }
            else if (isInShikabane)
            {
                sikabaneText.SetActive(true);
                akanaiText.SetActive(false);
                zihankiText.SetActive(false);
                ShowPanel();
            }
            else if (isInZihanki)
            {
                zihankiText.SetActive(true);
                akanaiText.SetActive(false);
                sikabaneText.SetActive(false);
                ShowPanel();
            }
        }
        // Escapeキーでメッセージパネルを閉じる
        if (Input.GetKey(KeyCode.Escape))
        {
            OnButtonClick();
        }
    }

    // イベント用オブジェクトのトリガー判定（IsTrigger = true の場合）
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"OnTriggerEnter2D: {other.name} のトリガーに入りました"); // すべてのトリガーでLog
        if (finishObject == null || shikabaneObject == null || zihankiObject == null)
        {
            Debug.LogWarning("イベント用オブジェクトがInspectorで正しくアタッチされていません");
        }
        if (other.gameObject == finishObject) {
            isInFinish = true;
            Debug.Log("Finishオブジェクトのトリガーに入りました");
        }
        if (other.gameObject == shikabaneObject) {
            isInShikabane = true;
            Debug.Log("ObShikabaneオブジェクトのトリガーに入りました");
        }
        if (other.gameObject == zihankiObject) {
            isInZihanki = true;
            Debug.Log("ObjZihankiオブジェクトのトリガーに入りました");
        }
        // どれか1つでもtrueなら表示
        if (showObject != null && (isInFinish || isInShikabane || isInZihanki))
        {
            showObject.SetActive(true);
        }
    }
    // イベント用オブジェクトのトリガーから出たときの処理
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == finishObject) isInFinish = false;
        if (other.gameObject == shikabaneObject) isInShikabane = false;
        if (other.gameObject == zihankiObject) isInZihanki = false;
        // すべてfalseなら非表示
        if (showObject != null && !(isInFinish || isInShikabane || isInZihanki))
        {
            showObject.SetActive(false);
        }
    }

    // メッセージパネルを表示し、移動を停止する
    private void ShowPanel()
    {
        messagePanel.SetActive(true);
        aibouPic.SetActive(true);
        canMove = false;
        rb.linearVelocity = Vector2.zero;
    }

    // メッセージパネルを閉じて移動を再開する
    public void OnButtonClick()
    {
        canMove = true;
        messagePanel.SetActive(false);
        aibouPic.SetActive(false);
    }
}