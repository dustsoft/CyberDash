using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] protected float _chanceToSpawn = 100;

    protected virtual void Start()
    {
        bool canSpawn = _chanceToSpawn >= Random.Range(0, 100);

        if (canSpawn == false)
            Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            collision.GetComponent<Player>().Damange();
    }
}
