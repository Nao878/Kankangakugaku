using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 1.8f;
    private Rigidbody2D rb = null;
    private bool up = false;
    private bool down = false;
    private bool left = false;
    private bool right = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow) || up)
        {
            rb.linearVelocity = new Vector2(0, speed);
        }

        if (Input.GetKey(KeyCode.DownArrow) || down)
        {
            rb.linearVelocity = new Vector2(0, -speed);
        }

        if (Input.GetKey(KeyCode.LeftArrow) || left)
        {
            rb.linearVelocity = new Vector2(-speed, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow) || right)
        {
            rb.linearVelocity = new Vector2(speed, 0);
        }

        if (!Input.anyKey && !up && !down && !left && !right)
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
    }

    public void Up()
    {
        up = true;
        down = false;
        right = false;
        left = false;
    }
    public void Down()
    {
        up = false;
        down = true;
        right = false;
        left = false;
    }
    public void Right()
    {
        up = false;
        down = false;
        right = true;
        left = false;
    }
    public void Left()
    {
        up = false;
        down = false;
        right = false;
        left = true;
    }
    public void Stop()
    {
        up = false;
        down = false;
        right = false;
        left = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("��ɓ�������");
        SceneManager.LoadScene("GameOver");
    }
}