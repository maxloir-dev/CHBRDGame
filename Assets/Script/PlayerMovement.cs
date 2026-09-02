using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float climbSpeed;

    [Header("Jump")]
    public float minJumpHeight = 1.4f;
    public float maxJumpHeight = 2.7f;

    // Temps pendant lequel le joueur peut augmenter
    // progressivement la hauteur du saut
    public float maxJumpTime = 0.25f;

    // Coupe la montée lorsque le bouton est relâché
    // Plus la valeur est basse, plus le saut est coupé
    public float jumpCutMultiplier = 0.5f;

    // Gravité supplémentaire pendant la descente
    public float fallMultiplier = 1.3f;

    [Header("Double Jump")]
public bool canDoubleJump = false;
public float doubleJumpHeight = 2.16f;

public float doubleJumpHorizontalBoost = 8f;
public float doubleJumpBoostTime = 0.12f;
private float doubleJumpBoostTimer = 0f;
private float doubleJumpDirection = 0f;

private bool hasDoubleJumped = false;



    private bool isGrounded;
    private bool isJumping;
    public bool isClimbing;

    private float jumpTimeCounter;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask collisionLayers;

    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private Vector3 velocity = Vector3.zero;
    private float horizontaleMovement;
    private float verticalMovement;


    void Update()
    {
        // ==========================================
        // INPUT
        // ==========================================

        horizontaleMovement = Input.GetAxis("Horizontal") * moveSpeed;
        verticalMovement = Input.GetAxis("Vertical") * climbSpeed;


        // ==========================================
        // DEBUT DU SAUT
        // ==========================================

        if (Input.GetButtonDown("Jump"))
{
    // ==========================================
    // PREMIER SAUT
    // ==========================================

    if (isGrounded)
    {
        isJumping = true;
        hasDoubleJumped = false;
        jumpTimeCounter = maxJumpTime;

        float jumpVelocity = Mathf.Sqrt(
            2f * Mathf.Abs(Physics2D.gravity.y * rb.gravityScale) * minJumpHeight
        );

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpVelocity
        );
    }

    // ==========================================
    // DOUBLE SAUT
    // ==========================================

    // DOUBLE SAUT
else if (canDoubleJump && !hasDoubleJumped)
{
    hasDoubleJumped = true;

    // Direction du double saut
    doubleJumpDirection = Input.GetAxisRaw("Horizontal");

    // Si aucune direction n'est appuyée,
    // on utilise la direction du personnage
    if (doubleJumpDirection == 0)
    {
        doubleJumpDirection = spriteRenderer.flipX ? -1f : 1f;
    }

    // Active la période de boost
    doubleJumpBoostTimer = doubleJumpBoostTime;

    // Calcul de la vitesse verticale
    float doubleJumpVelocity = Mathf.Sqrt(
        2f *
        Mathf.Abs(Physics2D.gravity.y * rb.gravityScale) *
        doubleJumpHeight
    );

    // Vraie propulsion horizontale
    rb.linearVelocity = new Vector2(
        doubleJumpDirection * doubleJumpHorizontalBoost,
        doubleJumpVelocity
    );

    // Pas de maintien du bouton pour ce saut
    isJumping = false;
}
}


        // ==========================================
        // MAINTIEN DU SAUT
        // ==========================================

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                // Progression entre la hauteur minimale
                // et la hauteur maximale
                float jumpProgress =
                    1f - (jumpTimeCounter / maxJumpTime);

                float currentJumpHeight = Mathf.Lerp(
                    minJumpHeight,
                    maxJumpHeight,
                    jumpProgress
                );

                // Calcul de la vitesse nécessaire
                // pour atteindre cette hauteur
                float jumpVelocity = Mathf.Sqrt(
                    2f *
                    Mathf.Abs(Physics2D.gravity.y * rb.gravityScale) *
                    currentJumpHeight
                );

                // On ne redonne jamais une vitesse supérieure
                // à celle nécessaire pour la hauteur actuelle
                if (rb.linearVelocity.y < jumpVelocity)
                {
                    rb.linearVelocity = new Vector2(
                        rb.linearVelocity.x,
                        jumpVelocity
                    );
                }

                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }


        // ==========================================
        // RELACHEMENT DE LA TOUCHE
        // ==========================================

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;

            // Si le personnage monte encore,
            // on réduit sa vitesse verticale.
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    rb.linearVelocity.y * jumpCutMultiplier
                );
            }
        }


        // ==========================================
        // DESCENTE PLUS RAPIDE
        // ==========================================

 if (rb.linearVelocity.y < 0)
{
    rb.linearVelocity += Vector2.up
        * Physics2D.gravity.y
        * (fallMultiplier - 1f)
        * Time.deltaTime;
}


        // ==========================================
        // ANIMATIONS
        // ==========================================

        Flip(rb.linearVelocity.x);

        float characterVelocity = Mathf.Abs(rb.linearVelocity.x);

        animator.SetFloat("Speed", characterVelocity);
        animator.SetBool("isClimbing", isClimbing);
    }


    void FixedUpdate()
    {
        // ==========================================
        // DETECTION DU SOL
        // ==========================================

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            collisionLayers
        );
        if (isGrounded)
{
    hasDoubleJumped = false;
}

        MovePlayer(
            horizontaleMovement,
            verticalMovement
        );
    }


    void MovePlayer(
    float _horizontalMovement,
    float _verticalMovement
)
{
    if (!isClimbing)
    {
        // BOOST DU DOUBLE SAUT
        if (doubleJumpBoostTimer > 0)
        {
            doubleJumpBoostTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(
                doubleJumpDirection * doubleJumpHorizontalBoost,
                rb.linearVelocity.y
            );

            return;
        }

        // CONTROLE NORMAL
        Vector3 targetVelocity = new Vector2(
            _horizontalMovement,
            rb.linearVelocity.y
        );

        rb.linearVelocity = Vector3.SmoothDamp(
            rb.linearVelocity,
            targetVelocity,
            ref velocity,
            0.03f
        );
    }
    else
    {
        Vector3 targetVelocity = new Vector2(
            0,
            _verticalMovement
        );

        rb.linearVelocity = Vector3.SmoothDamp(
            rb.linearVelocity,
            targetVelocity,
            ref velocity,
            0.05f
        );
    }
}


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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}
