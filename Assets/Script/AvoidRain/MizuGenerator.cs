using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MizuGenerator : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float span = 0.5f;
    float delta = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.delta += Time.deltaTime;

        if (this.delta > this.span)
        {
            this.delta = 0;
            GameObject go = Instantiate(arrowPrefab);
            int px = Random.Range(-100, 100);
            go.transform.position = new Vector3(px, 100, 90);
        }
    }
}


