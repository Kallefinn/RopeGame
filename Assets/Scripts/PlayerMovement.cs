using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Player Movement is achieved by Applying Forces to a rigidbody, updates on "position" should be avoided


public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    public Data Data;
  
    [SerializeField]
    Transform mouseHitbox = null;

    [SerializeField]
    GameObject rope = null;
    GameObject RopeObject;
    RopeBehaviour ropeControll;


    public Rigidbody2D body { get; private set; }

    public bool IsFacingRight { get; private set; }
    public bool isJumping { get; private set; }
    public bool isWallJumping { get; private set; }
    public bool isSliding { get; private set; }


    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }


    private bool _isJumpCut;
    private bool _isJumpFalling;


    private float _wallJumpStartTime;
    private int _lastWallJumpDir;


    private Vector2 _moveInput;
    public float LastPressedJumpTime { get; private set; }


    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);

    [Header("Layer and Masks")]
    [SerializeField] private LayerMask _groundLayer;

    // Start is called before the first frame update

    void Awake()
    {

        body = this.GetComponent<Rigidbody2D>();
        createNewRope();

    }

    void Start()
    {
        setGravityScale(Data.gravityScale);
        IsFacingRight = true;
    }


    void Update()
    {


        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;

        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
        {
            CheckDirectionToFace(_moveInput.x > 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            onJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            onJumpUpInput();
        }

        if (!isJumping)
        {

            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                LastOnGroundTime = Data.coyoteTime;
            }

            if(Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize,0, _groundLayer) && IsFacingRight
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize,0,_groundLayer) && !IsFacingRight && !isWallJumping))
            {
                LastOnWallRightTime = Data.coyoteTime;
            }

            if (Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight && !isWallJumping))
            {
                LastOnWallLeftTime = Data.coyoteTime;
            }

            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);

        }


        if(isJumping && body.velocity.y < 0)
        {
            isJumping = false;

            if (!isWallJumping)
            {
                _isJumpFalling = true;
            }
        }

        if(isWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            isWallJumping = false;
        }

        if(LastOnGroundTime > 0 && !isJumping && !isWallJumping)
        {
            _isJumpCut = false;

            if (!isJumping)
            {
                _isJumpFalling = false;
            }
        }


        if(canJump() && LastPressedJumpTime > 0)
        {
            isJumping = true;
            isWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }
        else if(canWallJump() && LastPressedJumpTime > 0)
        {
            isWallJumping = true;
            isJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (LastOnWallRightTime > 0 ? -1 : 1);

            WallJump(_lastWallJumpDir);

        }


        if(canSlide() && LastOnWallLeftTime > 0 && _moveInput.x < 0 || (LastOnWallRightTime > 0 && _moveInput.x > 0))
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        if (isSliding)
        {
            setGravityScale(0);
        }
        else if (body.velocity.y < 0 && _moveInput.y < 0)
        {
            setGravityScale(Data.gravityScale * Data.fastFallGravityMult);

            body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, -Data.maxFastFallSpeed));
        }
        else if (_isJumpCut)
        {
            setGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
        }
        else if(body.velocity.y < 0)
        {
            setGravityScale(Data.gravityScale * Data.fallGravityMult);

            body.velocity = new Vector2(body.velocity.x, Mathf.Max(body.velocity.y, -Data.maxFallSpeed));
        }
        else
        {
            setGravityScale(Data.gravityScale);
        }

        ropeUpdates();
        updateMouse();

    }

    private void FixedUpdate()
    {
        if (isWallJumping)
        {
            Run(Data.wallJumpRunLerp);
        }
        else
        {
            Run(1);
        }

        if (isSliding)
        {
            Slide();
        }
    }


    void createNewRope()
    {
        RopeObject = Instantiate(rope);
        ropeControll = RopeObject.GetComponent<RopeBehaviour>();
        ropeControll.setHook(this.transform);
        ropeControll.setHand(mouseHitbox);
    }

    void changeHookTo(Transform newHook)
    {
        ropeControll.setHook(newHook);
    }

    void updateMouse()
    {
        mouseHitbox.position = this.transform.position + new Vector3(this.transform.localScale.x + 0.5f, 0, 0);
    }
 

    public float timer = 0.2f;
    public float createcounter = 0, deletecounter = 0;

    void ropeUpdates()
    {
        createcounter += Time.deltaTime;
        deletecounter += Time.deltaTime;

        if (Input.GetKey("l") && createcounter >= timer)
        {
            createcounter = 0;
            ropeControll.addSegment();
        }

        if (Input.GetKeyDown("k"))
        {
            ropeControll.disconnectHook();
            createNewRope();
        }

        if (Input.GetKey("j") && deletecounter >= timer)
        {
            deletecounter = 0;
            ropeControll.removeLastSegment();
        }

        if (Input.GetKeyDown("p"))
        {
            changeHookTo(this.transform);
        }

        if(deletecounter > 100) 
        {
            deletecounter = 0;
        }
        if(createcounter > 100)
        {
            createcounter = 0;
        }


    }


    public void onJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void onJumpUpInput()
    {
        if(canJumpCut() && canWallJumpCut())
        {
            _isJumpCut = true;
        }
    }

    public void setGravityScale(float scale)
    {
        body.gravityScale = scale;
    }


    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;


        targetSpeed = Mathf.Lerp(body.velocity.x, targetSpeed, lerpAmount);

        float accelRate;

        if (LastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        }

        if(isJumping || isWallJumping || _isJumpFalling && Mathf.Abs(body.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }

        //just to not stop player if they are moving faster than maxspeed
        if(Data.doConserveMomentum && Mathf.Abs(body.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(body.velocity.x) == Mathf.Sign(targetSpeed) &&
            Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        float speedDif = targetSpeed - body.velocity.x;

        float movement = speedDif * accelRate;


        body.AddForce(movement * Vector2.right, ForceMode2D.Force);

    }



    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }


    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;


        float force = Data.jumpForce;
        if(body.velocity.y < 0)
        {
            force -= body.velocity.y;

        }

        body.AddForce(force * Vector2.up, ForceMode2D.Impulse);
    }



    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallLeftTime = 0;
        LastOnWallRightTime = 0;

        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force *= dir;

        if(Mathf.Sign(body.velocity.x) != Mathf.Sign(force.x))
        {
            force.x -= body.velocity.x;
        }

        if(body.velocity.y < 0)
        {
            force.y -= body.velocity.y;
        }

        body.AddForce(force, ForceMode2D.Impulse);

    }



    private void Slide()
    {
        float speedDif = Data.slideSpeed - body.velocity.y;

        float movement = speedDif * Data.slideAccel;

        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        body.AddForce(movement * Vector2.up);
    }






    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
        {
            Turn();
        }
    }


    private bool canJump()
    {
        return LastOnGroundTime > 0 && !isJumping;
    }


    private bool canWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!isWallJumping || 
            (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || LastOnWallLeftTime > 0 && _lastWallJumpDir == -1);
    }


    private bool canJumpCut()
    {
        return isJumping && body.velocity.y > 0;
    }

    private bool canWallJumpCut()
    {
        return isWallJumping && body.velocity.y > 0;
    }

    public bool canSlide()
    {
        if(LastOnWallTime > 0 && !isJumping && !isWallJumping && LastOnGroundTime <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }



}
