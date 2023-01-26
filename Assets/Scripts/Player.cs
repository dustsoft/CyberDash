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
    bool _isRunning;
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
        _isRunning = _rb.velocity.x != 0;
        _anim.SetBool("isRunning", _isRunning);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - _groundCheckDistance));
    }
}
