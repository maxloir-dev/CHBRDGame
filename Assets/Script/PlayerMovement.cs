using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // =============================================================
    // MOVEMENT
    // =============================================================

    [Header("Movement")]
    public float moveSpeed;


    // =============================================================
    // JUMP
    // =============================================================

    [Header("Jump")]
    public float minJumpHeight = 1.4f;
    public float maxJumpHeight = 2.7f;
    public float maxJumpTime = 0.25f;
    public float jumpCutMultiplier = 0.5f;
    public float fallMultiplier = 1.3f;


    // =============================================================
    // DOUBLE JUMP
    // =============================================================

    [Header("Double Jump")]
    public bool canDoubleJump = false;
    public float doubleJumpHeight = 2.16f;
    public float doubleJumpHorizontalBoost = 8f;
    public float doubleJumpBoostTime = 0.12f;

    private float doubleJumpBoostTimer = 0f;
    private float doubleJumpDirection = 0f;
    private bool hasDoubleJumped = false;


    // =============================================================
    // GROUND CHECK
    // =============================================================

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask collisionLayers;

    private bool isGrounded;


    // =============================================================
    // WALL CHECK
    // =============================================================

    [Header("Wall Check")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckRadius = 0.15f;

    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool isTouchingWall;


    // =============================================================
    // WALL CLIMB
    // =============================================================

    [Header("Wall Climb")]
    public bool canWallClimb = true;
    public float wallClimbSpeed = 4f;


    // =============================================================
    // WALL HANG
    // =============================================================

    [Header("Wall Hang")]
    public float wallHangTime = 2f;

    private float wallHangTimer;


    // =============================================================
    // WALL JUMP
    // =============================================================

    [Header("Wall Jump")]
    public float wallJumpHorizontalForce = 10f;
    public float wallJumpVerticalForce = 14f;
    public float wallJumpBoostTime = 0.15f;
    public float wallJumpAirControlTime = 0.3f;
public float wallJumpAirControl = 1f;
    private bool jumpInputConsumed = false;
    private bool canNormalJump = true;
    private bool wallJumpActive = false;

    private float wallJumpBoostTimer;
    private float wallJumpDirection;
    private float wallJumpAirControlTimer;


    // =============================================================
    // PLAYER STATES
    // =============================================================

    private enum PlayerState
    {
        Normal,
        WallCling,
        WallJump
    }

    private PlayerState currentState = PlayerState.Normal;


    // =============================================================
    // REFERENCES
    // =============================================================

    [Header("References")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;


    // =============================================================
    // INTERNAL VARIABLES
    // =============================================================

    private Vector3 velocity = Vector3.zero;

    private float horizontaleMovement;
    private float verticalMovement;

    private float jumpTimeCounter;

    private float originalGravityScale;


    // =============================================================
    // AWAKE
    // =============================================================

    void Awake()
    {
        originalGravityScale = rb.gravityScale;
    }


    // =============================================================
    // UPDATE
    // =============================================================

    void Update()
    {
        // =========================================================
        // INPUT
        // =========================================================

        horizontaleMovement =
            Input.GetAxis("Horizontal") * moveSpeed;

        verticalMovement =
            Input.GetAxis("Vertical");


        // =========================================================
        // JUMP INPUT
        // =========================================================
 if (Input.GetButtonUp("Jump"))
    {
        jumpInputConsumed = false;
        wallJumpActive = false;
    }
        if (Input.GetButtonDown("Jump"))
        {
            HandleJump();
        }


        // =========================================================
        // VARIABLE JUMP
        // =========================================================

        HandleVariableJump();


        // =========================================================
        // JUMP RELEASE
        // =========================================================

        HandleJumpRelease();


        // =========================================================
        // EXTRA GRAVITY
        // =========================================================

        if (currentState != PlayerState.WallCling &&
            rb.linearVelocity.y < 0)
        {
            rb.linearVelocity +=
                Vector2.up *
                Physics2D.gravity.y *
                (fallMultiplier - 1f) *
                Time.deltaTime;
        }


        // =========================================================
        // ANIMATIONS
        // =========================================================

        Flip(rb.linearVelocity.x);

        float characterVelocity =
            Mathf.Abs(rb.linearVelocity.x);

        animator.SetFloat(
            "Speed",
            characterVelocity
        );

        animator.SetBool(
            "isClimbing",
            currentState == PlayerState.WallCling
        );
    }


    // =============================================================
    // HANDLE JUMP
    // =============================================================

    void HandleJump()
{
    Debug.Log(
        "JUMP INPUT | State: " + currentState +
        " | Grounded: " + isGrounded +
        " | TouchingWall: " + isTouchingWall
    );
    // =========================================================
    // BLOQUER LA RÉUTILISATION DU MÊME APPUI
    // =========================================================

    if (jumpInputConsumed)
    {
        return;
    }


    // =========================================================
    // WALL JUMP
    // =========================================================

    if (currentState == PlayerState.WallCling &&
        isTouchingWall)
    {
        StartWallJump();
        return;
    }


    // =========================================================
    // NORMAL JUMP
    // =========================================================

    if (currentState == PlayerState.Normal &&
    isGrounded &&
    canNormalJump)
    {
        StartNormalJump();
        return;
    }


    // =========================================================
    // DOUBLE JUMP
    // =========================================================

    if (currentState == PlayerState.Normal &&
        !isGrounded &&
        canDoubleJump &&
        !hasDoubleJumped)
    {
        StartDoubleJump();
    }
}


    // =============================================================
    // NORMAL JUMP
    // =============================================================

    void StartNormalJump()
    {
        Debug.Log("NORMAL JUMP !");
        currentState = PlayerState.Normal;

        hasDoubleJumped = false;

        jumpTimeCounter = maxJumpTime;

        float jumpVelocity = Mathf.Sqrt(
            2f *
            Mathf.Abs(
                Physics2D.gravity.y *
                rb.gravityScale
            ) *
            minJumpHeight
        );

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpVelocity
        );
    }


    // =============================================================
    // DOUBLE JUMP
    // =============================================================

    void StartDoubleJump()
    {
        Debug.Log("DOUBLE JUMP !");
        hasDoubleJumped = true;

        doubleJumpDirection =
            Input.GetAxisRaw("Horizontal");

        if (doubleJumpDirection == 0)
        {
            doubleJumpDirection =
                spriteRenderer.flipX ? -1f : 1f;
        }

        doubleJumpBoostTimer =
            doubleJumpBoostTime;

        float doubleJumpVelocity = Mathf.Sqrt(
            2f *
            Mathf.Abs(
                Physics2D.gravity.y *
                rb.gravityScale
            ) *
            doubleJumpHeight
        );

        rb.linearVelocity = new Vector2(
            doubleJumpDirection *
            doubleJumpHorizontalBoost,
            doubleJumpVelocity
        );
    }


    // =============================================================
    // WALL JUMP
    // =============================================================

    void StartWallJump()
    {
        jumpInputConsumed = true;
        wallJumpActive = true;
        canNormalJump = false;
        currentState = PlayerState.WallJump;

        wallHangTimer = 0f;
        jumpTimeCounter = 0f;

        rb.gravityScale = originalGravityScale;

        // Détermine la direction opposée au mur
        if (isTouchingWallLeft)
        {
            wallJumpDirection = 1f;
        }
        else
        {
            wallJumpDirection = -1f;
        }

        wallJumpBoostTimer =
            wallJumpBoostTime;
            wallJumpAirControlTimer = wallJumpAirControlTime;

        rb.linearVelocity = new Vector2(
            wallJumpDirection *
            wallJumpHorizontalForce,
            wallJumpVerticalForce
        );
    }


    // =============================================================
    // VARIABLE JUMP
    // =============================================================

    void HandleVariableJump()
    {
        // Le saut variable concerne uniquement
        // le premier saut normal.
     if (wallJumpActive)
{
    return;
}

        if (currentState != PlayerState.Normal)
            return;

        if (!Input.GetButton("Jump"))
            return;

        if (jumpTimeCounter <= 0)
            return;

        // Si le joueur est au sol et vient de sauter,
        // le compteur est actif.

        if (rb.linearVelocity.y <= 0)
            return;

        float jumpProgress =
            1f -
            (jumpTimeCounter / maxJumpTime);

        float currentJumpHeight =
            Mathf.Lerp(
                minJumpHeight,
                maxJumpHeight,
                jumpProgress
            );

        float jumpVelocity = Mathf.Sqrt(
            2f *
            Mathf.Abs(
                Physics2D.gravity.y *
                rb.gravityScale
            ) *
            currentJumpHeight
        );
        Debug.Log(
    "VARIABLE JUMP | State: " + currentState +
    " | Y: " + rb.linearVelocity.y +
    " | Counter: " + jumpTimeCounter
);

        if (rb.linearVelocity.y < jumpVelocity)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpVelocity
            );
        }

        jumpTimeCounter -= Time.deltaTime;
    }


    // =============================================================
    // JUMP RELEASE
    // =============================================================

    void HandleJumpRelease()
    {
        if (jumpInputConsumed)
    {
        return;
    }
        if (!Input.GetButtonUp("Jump"))
            return;

        jumpTimeCounter = 0f;

        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y *
                jumpCutMultiplier
            );
        }
    }


    // =============================================================
    // FIXED UPDATE
    // =============================================================

    void FixedUpdate()
    {
        // =========================================================
        // GROUND CHECK
        // =========================================================

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            collisionLayers
        );
        


        // =========================================================
        // WALL CHECK
        // =========================================================

        isTouchingWallLeft =
            Physics2D.OverlapCircle(
                wallCheckLeft.position,
                wallCheckRadius,
                collisionLayers
            );

        isTouchingWallRight =
            Physics2D.OverlapCircle(
                wallCheckRight.position,
                wallCheckRadius,
                collisionLayers
            );

        isTouchingWall =
            isTouchingWallLeft ||
            isTouchingWallRight;


        // =========================================================
        // GROUND RESET
        // =========================================================

        if (isGrounded && currentState != PlayerState.WallJump)
{
    hasDoubleJumped = false;
    canNormalJump = true;

    if (currentState != PlayerState.Normal)
    {
        currentState = PlayerState.Normal;

        wallHangTimer = 0f;
        wallJumpBoostTimer = 0f;

        rb.gravityScale =
            originalGravityScale;
    }
}


        // =========================================================
        // WALL STATE
        // =========================================================

        HandleWallState();


        // =========================================================
        // MOVEMENT
        // =========================================================
if (wallJumpAirControlTimer > 0f)
{
    wallJumpAirControlTimer -= Time.fixedDeltaTime;
}
        HandleMovement();
    }


    // =============================================================
    // WALL STATE
    // =============================================================

    void HandleWallState()
    {
        // =========================================================
        // WALL JUMP
        // =========================================================

        if (currentState == PlayerState.WallJump)
        {
            wallJumpBoostTimer -=
                Time.fixedDeltaTime;

            // Pendant le boost, aucune accroche possible.
            if (wallJumpBoostTimer > 0)
            {
                return;
            }

            // Le wall jump est terminé.
            currentState = PlayerState.Normal;

            return;
        }


        // =========================================================
        // PAS DE WALL CLIMB
        // =========================================================

        if (!canWallClimb)
            return;

        if (isGrounded)
        {
          
        }

        if (!isTouchingWall)
        {
            // Si on n'est plus contre un mur,
            // on est forcément en mouvement normal.
            if (currentState == PlayerState.WallCling)
            {
                currentState = PlayerState.Normal;

                rb.gravityScale =
                    originalGravityScale;
            }

            return;
        }


        // =========================================================
        // WALL CLING
        // =========================================================

        float verticalInput =
            Input.GetAxisRaw("Vertical");

        float horizontalInput =
            Input.GetAxisRaw("Horizontal");


        // =========================================================
        // DÉCROCHAGE HORIZONTAL
        // =========================================================

        bool movingAwayFromLeftWall =
            isTouchingWallLeft &&
            horizontalInput > 0;

        bool movingAwayFromRightWall =
            isTouchingWallRight &&
            horizontalInput < 0;

        if (movingAwayFromLeftWall ||
            movingAwayFromRightWall)
        {
            LeaveWall();
            return;
        }


        // =========================================================
        // ACCROCHAGE AUTOMATIQUE
        // =========================================================

        if (currentState != PlayerState.WallCling)
        {
            currentState = PlayerState.WallCling;

            wallHangTimer =
                wallHangTime;
        }


        // =========================================================
        // GRAVITÉ OFF
        // =========================================================

        rb.gravityScale = 0f;


        // =========================================================
        // MOUVEMENT VERTICAL
        // =========================================================

        if (verticalInput != 0)
        {
            // Le joueur bouge :
            // le compteur revient à 2 secondes.

            wallHangTimer =
                wallHangTime;
        }
        else
        {
            // Aucun mouvement :
            // le joueur reste accroché mais immobile.

            wallHangTimer -=
                Time.fixedDeltaTime;
        }


        // =========================================================
        // TIMEOUT
        // =========================================================

        if (wallHangTimer <= 0f)
        {
            LeaveWall();
        }
    }


    // =============================================================
    // LEAVE WALL
    // =============================================================

    void LeaveWall()
    {
        currentState = PlayerState.Normal;

        wallHangTimer = 0f;

        rb.gravityScale =
            originalGravityScale;
    }


    // =============================================================
    // MOVEMENT
    // =============================================================

    void HandleMovement()
    {
        // =========================================================
        // WALL CLING / CLIMB
        // =========================================================

        if (currentState == PlayerState.WallCling)
        {
            float verticalInput =
                Input.GetAxisRaw("Vertical");


            // -----------------------------------------------------
            // MONTER
            // -----------------------------------------------------

            if (verticalInput > 0)
            {
                rb.linearVelocity = new Vector2(
                    0f,
                    wallClimbSpeed
                );

                return;
            }


            // -----------------------------------------------------
            // DESCENDRE
            // -----------------------------------------------------

            if (verticalInput < 0)
            {
                rb.linearVelocity = new Vector2(
                    0f,
                    -wallClimbSpeed
                );

                return;
            }


            // -----------------------------------------------------
            // IMMOBILE
            // -----------------------------------------------------

            rb.linearVelocity =
                Vector2.zero;

            return;
        }


        // =========================================================
        // WALL JUMP BOOST
        // =========================================================

 if (currentState == PlayerState.WallJump)
{
    rb.linearVelocity = new Vector2(
        wallJumpDirection * wallJumpHorizontalForce,
        rb.linearVelocity.y
    );

    return;
}


        // =========================================================
        // DOUBLE JUMP BOOST
        // =========================================================

        if (doubleJumpBoostTimer > 0)
        {
            doubleJumpBoostTimer -=
                Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(
                doubleJumpDirection *
                doubleJumpHorizontalBoost,
                rb.linearVelocity.y
            );

            return;
        }


        // =========================================================
        // NORMAL MOVEMENT
        // =========================================================

        float currentAirControl = 1f;

if (wallJumpAirControlTimer > 0f)
{
    currentAirControl = wallJumpAirControl;
}

Vector3 targetVelocity =
    new Vector2(
        horizontaleMovement * currentAirControl,
        rb.linearVelocity.y
    );

        rb.linearVelocity =
            Vector3.SmoothDamp(
                rb.linearVelocity,
                targetVelocity,
                ref velocity,
                0.03f
            );
    }


    // =============================================================
    // FLIP
    // =============================================================

    void Flip(float _velocityX)
    {
        if (_velocityX > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (_velocityX < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }


    // =============================================================
    // GIZMOS
    // =============================================================

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        if (wallCheckLeft != null)
        {
            Gizmos.DrawWireSphere(
                wallCheckLeft.position,
                wallCheckRadius
            );
        }

        if (wallCheckRight != null)
        {
            Gizmos.DrawWireSphere(
                wallCheckRight.position,
                wallCheckRadius
            );
        }
    }
}