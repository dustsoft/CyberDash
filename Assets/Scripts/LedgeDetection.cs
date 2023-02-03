using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [SerializeField] float _radius;
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] Player _player;

    bool _canDetect;

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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            _canDetect = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
