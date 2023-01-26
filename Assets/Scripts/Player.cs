using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Info")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;

    [Header("Collision Info")]
    public float groundCheckDistance;
    public LayerMask whatIsGround;
    public Rigidbody2D rb;
    bool _isGrounded;

    bool _runStarted;


    void Update()
    {
        //Starts The Run
        if (Input.GetButtonDown("Jump"))
        {
            _runStarted = true;
        }

        CheckCollision();
        CheckInput();
    }

    void CheckInput()
    {
        if (_runStarted == true)
        {
            //Auto Run
            rb.velocity = new Vector2(_moveSpeed, rb.velocity.y);

            //Jump Input
            if (Input.GetButtonDown("Jump") && _isGrounded == true)
            {
                rb.velocity = new Vector2(rb.velocity.x, _jumpForce);
            }
        }
    }

    void CheckCollision()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
    }
}
