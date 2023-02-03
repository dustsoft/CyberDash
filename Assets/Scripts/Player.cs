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

    [Header("Dash Info")]
    [SerializeField] float _dashSpeed;
    [SerializeField] float _dashTime;
    [SerializeField] float _dashCoolDown;
    float _dashCoolDownCounter;
    float _dashTimeCounter;

    [Header("Collision Info")]
    [SerializeField] float _groundCheckDistance;
    [SerializeField] float _ceillingCheckDistance;
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] Transform _wallCheck;
    [SerializeField] Vector2 _wallCheckSize;
    [HideInInspector] public bool ledgeDetected;

    bool _isGrounded;
    bool _isSliding;
    bool _isAirdashing;
    bool _runStarted;
    bool _canDoubleJump;
    bool _canAirdash;
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

        _dashTimeCounter = _dashTimeCounter - Time.deltaTime;
        _dashCoolDownCounter = _dashCoolDownCounter - Time.deltaTime;

        if (_runStarted == true)
            Movement();

        if (_isGrounded == true)
        {
            _canDoubleJump = true;
            _canAirdash = true;
        }

        CheckForDash();

        CheckInput();
    }

    void CheckForDash()
    {
        if (_dashTimeCounter < 0 && _ceillingDetected == false)
        {
            _isSliding = false;
            _isAirdashing = false;
        }
     
    }

    void Movement()
    {
        if (_wallDetected == true)
            return;

        //Running
        if(_isSliding == false && _isGrounded == true && _isAirdashing == false)
            _rb.velocity = new Vector2(_moveSpeed, _rb.velocity.y);

        //Sliding
        if (_isSliding == true)
            _rb.velocity = new Vector2(_dashSpeed, _rb.velocity.y);

        //Airdashing
        if (_isAirdashing == true && _isGrounded == false && _wallDetected == false)
        {
            _rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            _rb.velocity = new Vector2(_dashSpeed, _rb.velocity.y);
        }
        if (_isAirdashing == false)
        {
            _rb.constraints = RigidbodyConstraints2D.None;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
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

    void DashButton()
    {
        //Slide
        if (_rb.velocity.x != 0 && _dashCoolDownCounter < 0 && _isGrounded == true)
        {
            _isSliding = true;
            _dashTimeCounter = _dashTime;
            _dashCoolDownCounter = _dashCoolDown;
        }

        //Airdash
        if (_rb.velocity.x != 0 && _dashCoolDownCounter < 0 && _isGrounded == false && _canAirdash == true)
        {
            _isAirdashing = true;
            _canAirdash = false;
            _dashTimeCounter = _dashTime;
            _dashCoolDownCounter = _dashCoolDown;
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

        //Slide/AirDash Input
        if (Input.GetKeyDown(KeyCode.LeftShift))
            DashButton();
    }

    void AnimatorController()
    {
        _anim.SetBool("canDoubleJump", _canDoubleJump);
        _anim.SetBool("isGrounded", _isGrounded);
        _anim.SetBool("isSliding", _isSliding);
        _anim.SetBool("isAirdashing", _isAirdashing);

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
