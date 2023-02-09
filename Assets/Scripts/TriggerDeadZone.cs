using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDeadZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            GameManager.instance.RestartLevel();
    }
}
