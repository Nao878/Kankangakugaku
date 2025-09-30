using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AibouController : MonoBehaviour
{
    public GameObject targetObj;
    private float speed = 10;
    private Rigidbody2D rb = null;
    public static bool aibouStop = false;

    void Start()
    {
        rb  = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!aibouStop)
        {
            Vector2 dir = (targetObj.transform.position - this.transform.position).normalized;
            float vx = dir.x * speed;
            float vy = dir.y * speed;
            rb.linearVelocity = new Vector2(vx, vy);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Debug.Log("�v���C���[�ɓ�������");
            aibouStop = true;
        }
    }
}
