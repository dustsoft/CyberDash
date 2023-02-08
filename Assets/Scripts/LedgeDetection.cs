using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] float _radius;
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] Player _player;

    bool _canDetect;

    BoxCollider2D _boxC2D => GetComponent<BoxCollider2D>();

    void Update()
    {
        if (_canDetect == true)
            _player.ledgeDetected = Physics2D.OverlapCircle(transform.position, _radius, _whatIsGround);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            _canDetect = false;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(_boxC2D.bounds.center, _boxC2D.size, 0);

        foreach (var hit in colliders)
        {
            //if (hit.GetComponent<PlatformerController>() != nulll)
                //return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            _canDetect = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
