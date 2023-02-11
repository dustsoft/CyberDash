using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region VARIABLES

    [Header("Movement Info")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _speedMultiplier;
    float _defaultSpeed;
    [Space]
    [SerializeField] float _milestoneIncreaser;
    float _defaultMilestoneIncrease;
    float _speedMilestone;

    [Header("Jump Info")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _doubleJumpForce;
    
    [Header("Powerup Info")]
    [SerializeField] public bool _doubleJump;
    [SerializeField] bool _slide;
    [SerializeField] bool _airdash;
    [SerializeField] bool _doubleAirdash;

    [Header("Dash Info")]
    [SerializeField] float _dashSpeed;
    [SerializeField] float _dashTime;
    [SerializeField] float _dashCoolDown;

    float _distanceBetweenImages;
    float _dashCoolDownCounter;
    float _dashTimeCounter;
    float _lastImageXpos;

    [Header("Knockback Info")]
    [SerializeField] Vector2 _knockbackDirection;

    [Header("Collision Info")]
    [SerializeField] LayerMask _whatIsGround;
    [SerializeField] Transform _groundCheck;
    [SerializeField] float _groundCheckDistance;
    [SerializeField] float _groundCheckRadius;
    [Space]
    [SerializeField] float _ceillingCheckDistance; // Need for fall anim ground check
    [SerializeField] Transform _wallCheck;
    [SerializeField] Vector2 _wallCheckSize;
    [HideInInspector] public bool ledgeDetected;

    [Header("Ledge Info")]
    [SerializeField] Vector2 _offSet1; // offset for position BEFORE climb
    [SerializeField] Vector2 _offSet2; // offset for position AFTER climb

    Vector2 _climbBegunPosition;
    Vector2 _climbOverPosition;

    bool _isGrounded;
    bool _isGroundedCircleCheck;
    bool _isKnocked;
    bool _isSliding;
    bool _isAirdashing;
    bool _isDead;
    bool _runStarted;
    bool _canGrabLedge = true;
    bool _canClimb;
    bool _canBeKnocked = true;
    bool _canDoubleJump;
    bool _canAirdash;
    bool _canDoubleAirdash;
    bool _wallDetected;
    bool _ceillingDetected;

    Rigidbody2D _rb;
    Animator _anim;
    SpriteRenderer _spriteRenderer;
    #endregion

    #region METHODS/FUNCTIONS
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _speedMilestone = _milestoneIncreaser;
        _defaultSpeed = _moveSpeed;
        _defaultMilestoneIncrease = _milestoneIncreaser;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) //TEMP CODE FOR TESTING, WILL CALL FROM PROPER METHOD
            Knockback();

        if (Input.GetKeyDown(KeyCode.O) && _isDead == false) //TEMP CODE FOR TESTING, WILL CALL FROM PROPER METHOD
            StartCoroutine(Die());

        CheckCollision();

        AnimatorController();

        _dashTimeCounter = _dashTimeCounter - Time.deltaTime;
        _dashCoolDownCounter = _dashCoolDownCounter - Time.deltaTime;

        if (_isDead == true)
            return;

        if (_isKnocked == true)
        {
            if (_isAirdashing || _isSliding)
            {
                _rb.constraints = RigidbodyConstraints2D.None;
                _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

                _isSliding = false;
                _isAirdashing = false;
            }

            return;
        }

        if (_runStarted == true)
            Movement();

        // Dbl-Jump & Airdash Reset
        if (_isGrounded == true)
        {
            _canDoubleJump = true;
            _canDoubleAirdash = false; // Can only dbl-airdash aftering airdashing
            _canAirdash = true;
        }

        // Double Airdash Logic Check
        if (_doubleAirdash == true)
            _airdash = true;

        SpeedController();

        CheckForLedge();

        CheckForDashCancel();

        CheckInput();
    }

    public void Damange() //This will need to be changed later
    {
        Knockback();
        SpeedReset();
    }

    void Knockback()
    {
        if (_canBeKnocked == false)
            return;

        StartCoroutine(Invincibility());
        _isKnocked = true;
        _rb.velocity = _knockbackDirection;
    }

    void CancelKnockback() => _isKnocked = false;

    void CheckForLedge()
    {
        if (ledgeDetected == true && _canGrabLedge)
        {
            _canGrabLedge = false;
            _rb.gravityScale = 0;

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
        _rb.gravityScale = 6; // 7 is the default I've been using for awhile, though now I'm using 6.
        transform.position = _climbOverPosition;
        Invoke("AllowLedgeGrab", 0.1f);
    }

    void AllowLedgeGrab() => _canGrabLedge = true;

    void CheckForDashCancel()
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

    void SpeedReset()
    {
        _moveSpeed = _defaultSpeed;
        _milestoneIncreaser = _defaultMilestoneIncrease;
    }

    void SpeedController()
    {
        if (_moveSpeed == _maxSpeed)
            return;

        if (transform.position.x > _speedMilestone)
        {
            _speedMilestone = _speedMilestone + _milestoneIncreaser;

            _moveSpeed = _moveSpeed * _speedMultiplier;
            _milestoneIncreaser = _milestoneIncreaser * _speedMultiplier;

            if (_moveSpeed > _maxSpeed)
                _moveSpeed = _maxSpeed;
        }
    }

    void Movement()
    {
        //Reset Player Transform Constraints After Airdash
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
        {
            SpeedReset();
            return;
        }

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

        if (_canClimb == true)
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
        if (_doubleAirdash == false)
        {
            if (_rb.velocity.x != 0 && _dashCoolDownCounter < 0 && _isGrounded == false && _canAirdash && _airdash)
            {
                _isAirdashing = true;
                _canAirdash = false;
                _dashTimeCounter = _dashTime;
                _dashCoolDownCounter = _dashCoolDown;
            }
        }

        //Double Airdash | Requires Powerup to use!
        if (_doubleAirdash == true)
        {
            if (_rb.velocity.x != 0 && _dashCoolDownCounter < 0 && _isGrounded == false && _canAirdash && _airdash && _canDoubleAirdash == false)
            {
                _isAirdashing = true;
                _canDoubleAirdash = true;
                _dashTimeCounter = _dashTime;
                _dashCoolDownCounter = _dashCoolDown;
            }

            if (_rb.velocity.x != 0 && _dashCoolDownCounter < 0 && _isGrounded == false && _canAirdash && _canDoubleAirdash)
            {
                _isAirdashing = true;
                _canDoubleAirdash = false;
                _canAirdash = false;
                _dashTimeCounter = _dashTime;
                _dashCoolDownCounter = _dashCoolDown;
            }
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
        _anim.SetBool("isKnocked", _isKnocked);

        _anim.SetFloat("xVelocity", _rb.velocity.x);
        _anim.SetFloat("yVelocity", _rb.velocity.y);

        if (_rb.velocity.y < -24)
            _anim.SetBool("canRoll", true);
    }

    void RollAnimFinished() => _anim.SetBool("canRoll", false);
    
    void CheckCollision()
    {
        // Ground Check Circle
        _isGroundedCircleCheck = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _whatIsGround);

        // Ground Check Line
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckDistance, _whatIsGround);


        if (_isGroundedCircleCheck == true && _isGrounded == false)
        {
            if (_wallDetected == false)
                _isGrounded = true;
        }

        _ceillingDetected = Physics2D.Raycast(transform.position, Vector2.up, _ceillingCheckDistance, _whatIsGround);
        _wallDetected = Physics2D.BoxCast(_wallCheck.position, _wallCheckSize, 0, Vector2.zero, 0, _whatIsGround);
    }

    void OnDrawGizmos()
    {
        // Draw Circle
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);

        // Draw Line
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - _groundCheckDistance));

        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + _ceillingCheckDistance));
        Gizmos.DrawWireCube(_wallCheck.position, _wallCheckSize);
    }
    #endregion

    #region COROUTINES
    IEnumerator Invincibility()
    {
        Color originalColor = _spriteRenderer.color;
        Color darkenColor = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0.25f);
        Color redFlash = new Color(255f, 0f, 0f, _spriteRenderer.color.a);

        _canBeKnocked = false;

        _spriteRenderer.color = redFlash;
        yield return new WaitForSeconds(0.2f);
        _spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = darkenColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = darkenColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = darkenColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = originalColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = darkenColor;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = originalColor;

        _canBeKnocked = true;
    }

    IEnumerator Die()
    {
        _isDead = true;
        _canBeKnocked = false;
        _rb.velocity = _knockbackDirection;
        _anim.SetBool("isDead", true);

        yield return new WaitForSeconds(3f);
        _rb.velocity = new Vector2(0, 0);

        yield return new WaitForSeconds(1f);
        GameManager.instance.RestartLevel();
    }
    #endregion
}
