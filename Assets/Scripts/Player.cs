using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Info")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;

    [Header("Collision Info")]
    [SerializeField] float _groundCheckDistance;
    [SerializeField] LayerMask _whatIsGround;

    bool _isGrounded;
    bool _runStarted;

    Rigidbody2D _rb;
    Animator _anim;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorController();
        CheckCollision();
        CheckInput();
    }

    void CheckInput()
    {
        //Starts The Run
        if (Input.GetButtonDown("Jump"))
        {
            _runStarted = true;
        }

        if (_runStarted == true)
        {
            //Auto Run
            _rb.velocity = new Vector2(_moveSpeed, _rb.velocity.y);

            //Jump Input
            if (Input.GetButtonDown("Jump") && _isGrounded == true)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
            }
        }
    }

    void CheckCollision()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckDistance, _whatIsGround);
    }

    void AnimatorController()
    {
        _anim.SetBool("isGrounded", _isGrounded);
        _anim.SetFloat("xVelocity", _rb.velocity.x);
        _anim.SetFloat("yVelocity", _rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - _groundCheckDistance));
    }
}
