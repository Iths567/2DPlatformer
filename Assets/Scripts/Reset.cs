using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    Vector3 originalPos;
    private void Start()
    {
        originalPos = gameObject.transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "spike")
        {
            gameObject.transform.position = originalPos;
        }
    }
}
