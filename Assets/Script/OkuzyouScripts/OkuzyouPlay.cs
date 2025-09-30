using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OkuzyouPlay : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 1.8f;
    private Rigidbody2D rb;
    private bool canMove = true;

    [Header("UI References")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private GameObject aibouPic;
    [SerializeField] private GameObject akanaiText;
    [SerializeField] private GameObject sikabaneText;
    [SerializeField] private GameObject zihankiText;

    // �e�G���A���Ƃ̐ڐG�t���O
    private bool isInFinish = false;
    private bool isInShikabane = false;
    private bool isInZihanki = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
        }
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) input.y += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) input.y -= 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) input.x -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) input.x += 1f;

        rb.linearVelocity = input.normalized * speed;
    }

    private void Update()
    {
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
        if (Input.GetKey(KeyCode.Escape))
        {
            OnButtonClick();
        }
    }

    // �ڐG�J�n
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject;
        if (other.CompareTag("Finish")) isInFinish = true;
        if (other.name == "ObShikabane") isInShikabane = true;
        if (other.name == "ObjZihanki") isInZihanki = true;
    }
    // �ڐG�I��
    private void OnCollisionExit2D(Collision2D collision)
    {
        var other = collision.gameObject;
        if (other.CompareTag("Finish")) isInFinish = false;
        if (other.name == "ObShikabane") isInShikabane = false;
        if (other.name == "ObjZihanki") isInZihanki = false;
    }

    private void ShowPanel()
    {
        messagePanel.SetActive(true);
        aibouPic.SetActive(true);
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        Invoke(nameof(SetAibouStop), 0.1f);
    }

    public void OnButtonClick()
    {
        canMove = true;
        messagePanel.SetActive(false);
        aibouPic.SetActive(false);
    }

    private void SetAibouStop()
    {
        AibouController.aibouStop = true;
    }
}