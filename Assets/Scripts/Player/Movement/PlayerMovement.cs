using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, InputSystem_Actions.IPlayerMovementActions
{
    #region Setup

    // Allow for editing variables in the inspector
    [Header("Base Movement Settings")]
    public float MAX_SPEED;
    public float MAX_DOWNWARD_SPEED;
    public float ACCELERATION;
    public float DECELERATION;

    [Header("Jump Settings")]
    public float DEFAULT_JUMP_FORCE;
    public int DEFAULT_COYOTE;

    [Header("Climbing Settings")]
    public float climbSpeed;
    public LayerMask ladderLayer;

    [Header("Colliders")]
    [SerializeField] private GroundCheck groundCheck;

    // Set up hidden values 
    private float coyoteTime = 0;
    private Rigidbody2D rb;
    private float horizontalInput;
    private float verticalInput;
    private bool isClimbing;
    private bool isTouchingLadder;
    private float defaultGravity;

    void OnEnable()
    {
        // Connect player input and rigidbody to script
        PlayerInputManager.Instance.playerControls.PlayerMovement.Enable();
        PlayerInputManager.Instance.playerControls.PlayerMovement.SetCallbacks(this);
    }

    void OnDisable()
    {
        // Disconnect input to prevent null references
        PlayerInputManager.Instance.playerControls.PlayerMovement.Disable();
        PlayerInputManager.Instance.playerControls.PlayerMovement.RemoveCallbacks(this);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    #endregion
    #region Movement

    void Update()
    {
        // Check grounded state
        coyoteTime = groundCheck.IsGrounded() ? DEFAULT_COYOTE : coyoteTime;

        if (isTouchingLadder && Mathf.Abs(verticalInput) > 0.1f) isClimbing = true;

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, verticalInput * climbSpeed);
            return;
        }

        // Move the player
        if (horizontalInput == 0)
        {
            // If no player input - Decelerate
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, DECELERATION * Time.deltaTime);
        }
        else
        {
            // If player input - Move
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, MAX_SPEED * horizontalInput, ACCELERATION * Time.deltaTime);
        }

        if (rb.linearVelocityY < -1 * MAX_DOWNWARD_SPEED)
        {
            //If above speed cap - Reduce speed
            rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, -1 * MAX_DOWNWARD_SPEED, DECELERATION * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // Reduce coyote time
        coyoteTime = coyoteTime < 0 ? coyoteTime : coyoteTime - 1;
    }


    #endregion
    #region Climb Detection


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & ladderLayer) != 0)
        {
            isTouchingLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & ladderLayer) != 0)
        {
            isTouchingLadder = false;
            isClimbing = false;
            rb.gravityScale = defaultGravity;
        }
    }


    #endregion
    #region Input

    public void OnHorizontalMove(InputAction.CallbackContext context)
    {
        // Read the players horizontal input
        horizontalInput = context.ReadValue<float>();
    }

    public void OnVerticalMove(InputAction.CallbackContext context)
    {
        // Read the players horizontal input
        verticalInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // If the player is currently or was recently
        if (coyoteTime >= 0 || isClimbing)
        {
            rb.linearVelocityY = DEFAULT_JUMP_FORCE;
            isClimbing = false;
            rb.gravityScale = defaultGravity;
        }
        coyoteTime = -1;
    }
    #endregion
}