using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MizuScript : MonoBehaviour
{
    private float mizuSpeed = 1.3f;

    void FixedUpdate()
    {
        transform.Translate(0, -mizuSpeed, 0);

        if (transform.position.y < -100.0f)
        {
            Destroy(gameObject);
            Debug.Log("destroy");
        }
    }
}


