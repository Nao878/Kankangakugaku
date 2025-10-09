using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatBallControler : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.position += new Vector3(0, 0.5f, 0);
    }
}
