using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InsekiPlayerController2: MonoBehaviour
{
    public float speed = 1.8f;
    private Rigidbody2D rb = null;
    //[SerializeField] string nextScene;
    [SerializeField] GameObject insekiPrefab;
    [SerializeField] Transform shotPoint;
    private string enemyTag = "enemy";
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

        Instantiate(insekiPrefab, shotPoint.position, shotPoint.rotation);
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == enemyTag)
        {
            Debug.Log("��ɓ�������");
            //SceneManager.LoadScene(nextScene);
        }
    }
}