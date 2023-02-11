using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressItem : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameManager.instance.progressItems++;
            Destroy(gameObject);
        }
    }
}
