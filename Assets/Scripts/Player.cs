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

    [Header("Powerup Info")]
    [SerializeField] bool _doubleJump;
    [SerializeField] bool _slide;
    [SerializeField] bool _airdash;

    [Header("Dash Info")]
    [SerializeField] float _dashSpeed;
    [SerializeField] float _dashTime;
    [SerializeField] float _dashCoolDown;
    float _distanceBetweenImages;
    float _dashCoolDownCounter;
    float _dashTimeCounter;
    float _lastImageXpos;

    [Header("Collision Info")]
    [SerializeField] float _groundCheckDistance;
    [SerializeField] float _ceillingCheckDistance;
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] Transform _wallCheck;
    [SerializeField] Vector2 _wallCheckSize;
    [HideInInspector] public bool ledgeDetected;

    [Header("Ledge Info")]
    [SerializeField] Vector2 _offSet1; // offset for position BEFORE climb
    [SerializeField] Vector2 _offSet2; // offset for position AFTER climb

    Vector2 _climbBegunPosition;
    Vector2 _climbOverPosition;

    bool _isGrounded;
    bool _isSliding;
    bool _isAirdashing;
    bool _runStarted;
    bool _canGrabLedge = true;
    bool _canClimb;
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

        CheckForLedge();

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

    void CheckForLedge()
    {
        if (ledgeDetected == true && _canGrabLedge)
        {
            _canGrabLedge = false;

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            _climbBegunPosition = ledgePosition + _offSet1;
            _climbOverPosition = ledgePosition + _offSet2;

            _canClimb = true;
        }

        if (_canClimb == true)
            transform.position = _climbBegunPosition;
    }

    void LedgeClimbOver()
    {
        _canClimb = false;
        transform.position = _climbOverPosition;
        Invoke("AllowLedgeGrab", 0.1f);
    }

    void AllowLedgeGrab() => _canGrabLedge = true;

    void CheckForDash()
    {
        if (_dashTimeCounter < 0 && _ceillingDetected == false)
        {
            _isSliding = false;
            _isAirdashing = false;
        }

        if (_wallDetected == true)
        {
            _isAirdashing = false;
            _isSliding = false;
        }
    }

    void Movement()
    {
        if (_isAirdashing == false)
        {
            _rb.constraints = RigidbodyConstraints2D.None;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        //After Image FX
        if (_isAirdashing || _isSliding)
        {
            AfterImagePool.Instance.GetFromPool();
            _lastImageXpos = transform.position.x;

            if (Mathf.Abs(transform.position.x - _lastImageXpos) > _distanceBetweenImages)
            {
                AfterImagePool.Instance.GetFromPool();
                _lastImageXpos = transform.position.x;
            }
        }

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
        else if (_canDoubleJump == true && _doubleJump == true)
        {
            //Double Jump | Requires Powerup to use!
            _canDoubleJump = false;
            _rb.velocity = new Vector2(_rb.velocity.x, _doubleJumpForce);
        }
    }

    void DashButton()
    {
        //Slide | Requires Powerup to use!
        if (_rb.velocity.x != 0 && _dashCoolDownCounter < 0 && _isGrounded == true && _slide == true)
        {
            _isSliding = true;
            _dashTimeCounter = _dashTime;
            _dashCoolDownCounter = _dashCoolDown;
        }

        //Airdash | Requires Powerup to use!
        if (_rb.velocity.x != 0 && _dashCoolDownCounter < 0 && _isGrounded == false && _canAirdash == true && _airdash == true)
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
        _anim.SetBool("canClimb", _canClimb);
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
