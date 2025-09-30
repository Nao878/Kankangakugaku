using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InsekiPlayerController : MonoBehaviour
{
    public float speed = 1.8f;
    private Rigidbody2D rb = null;
    private int i = 0;
    public float tspeed = 1.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.linearVelocity = new Vector2(0, speed);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.linearVelocity = new Vector2(0, -speed);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.linearVelocity = new Vector2(-speed, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.linearVelocity = new Vector2(speed, 0);
        }

        if (!Input.anyKey)
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
    }

    public void Up()
    {
        transform.Translate(0, tspeed, 0);
    }
    public void Down()
    {
        transform.Translate(0, -tspeed, 0);
    }
    public void Right()
    {
        transform.Translate(tspeed, 0, 0);
    }
    public void Left()
    {
        transform.Translate(-tspeed, 0, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        i++;
        if (i > 20)
        {
            Debug.Log("#�T�����t");
            //SceneManager.LoadScene("PlayGame2");
        }

        if (other.CompareTag("Finish"))
        {
            Debug.Log("#���C�P���� �C�N���� ���[���� ���� ���� �ʍ� ����");

        }
    }
}