using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region VARIABLES
    [Header("Movement Info")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _doubleJumpForce;

    [Header("Slide Info")]
    [SerializeField] float _slideSpeed;
    [SerializeField] float _slideTime;
    [SerializeField] float _slideCoolDown;
    float _slideCoolDownCounter;
    float _slideTimeCounter;

    [Header("Collision Info")]
    [SerializeField] float _groundCheckDistance;
    [SerializeField] float _ceillingCheckDistance;
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] Transform _wallCheck;
    [SerializeField] Vector2 _wallCheckSize;

    bool _isGrounded;
    bool _isSliding;
    bool _runStarted;
    bool _canDoubleJump;
    bool _wallDetected;
    bool _ceillingDetected;

    Rigidbody2D _rb;
    Animator _anim;
    #endregion

    #region METHODS/FUNCTIONS
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckCollision();

        AnimatorController();

        _slideTimeCounter = _slideTimeCounter - Time.deltaTime;
        _slideCoolDownCounter = _slideCoolDownCounter - Time.deltaTime;

        if (_runStarted == true)
            Movement();

        if (_isGrounded)
            _canDoubleJump = true;

        CheckForSlide();

        CheckInput();
    }

    void CheckForSlide()
    {
        if (_slideTimeCounter < 0 && _ceillingDetected == false)
            _isSliding = false;
    }

    void Movement()
    {
        if (_wallDetected == true)
            return;

        if (_isSliding == true)
            _rb.velocity = new Vector2(_slideSpeed, _rb.velocity.y);
        else
            _rb.velocity = new Vector2(_moveSpeed, _rb.velocity.y);
    }

    void JumpButton()
    {
        if (_isSliding == true)
            return;

        if (_isGrounded == true)
        {
            //Single Jump
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
        }
        else if (_canDoubleJump == true)
        {
            //Double Jump
            _canDoubleJump = false;
            _rb.velocity = new Vector2(_rb.velocity.x, _doubleJumpForce);
        }
    }

    void SlideButton()
    {
        if (_rb.velocity.x != 0 && _slideCoolDownCounter < 0)
        {
            _isSliding = true;
            _slideTimeCounter = _slideTime;
            _slideCoolDownCounter = _slideCoolDown;
        }
    }

    void CheckInput()
    {
        //Starts The Run | Unlocks The Player
        if (Input.GetKeyDown(KeyCode.Z)) //Z is a TEMP button solution, it should be changed later
            _runStarted = true;

        //Jump Input
        if (Input.GetButtonDown("Jump"))
            JumpButton();

        //Slide Input
        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();
    }

    void AnimatorController()
    {
        _anim.SetBool("canDoubleJump", _canDoubleJump);
        _anim.SetBool("isGrounded", _isGrounded);
        _anim.SetBool("isSliding", _isSliding);

        _anim.SetFloat("xVelocity", _rb.velocity.x);
        _anim.SetFloat("yVelocity", _rb.velocity.y);
    }

    void CheckCollision()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckDistance, _whatIsGround);
        _ceillingDetected = Physics2D.Raycast(transform.position, Vector2.up, _ceillingCheckDistance, _whatIsGround);
        _wallDetected = Physics2D.BoxCast(_wallCheck.position, _wallCheckSize, 0, Vector2.zero, 0, _whatIsGround);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - _groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + _ceillingCheckDistance));
        Gizmos.DrawWireCube(_wallCheck.position, _wallCheckSize);
    }
    #endregion
}
