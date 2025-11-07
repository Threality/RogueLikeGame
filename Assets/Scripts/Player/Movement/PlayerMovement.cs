using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, InputSystem_Actions.IPlayerMovementActions
{
    #region Setup

    // Allow for editing variables in the inspector
    [Header("Base Movement Settings")]
    public float MAX_SPEED;
    public float ACCELERATION;
    public float DECELERATION;

    [Header("Jump Settings")]
    public float DEFAULT_JUMP_FORCE;
    public int DEFAULT_COYOTE;

    [Header("Colliders")]
    [SerializeField] private GroundCheck groundCheck;

    // Set up hidden values 
    private float coyoteTime = 0;
    private Rigidbody2D rb;
    private float movementInput;

    void OnEnable()
    {
        // Connect player input and rigidbody to script
        rb = GetComponent<Rigidbody2D>();
        PlayerInputManager.Instance.playerControls.PlayerMovement.Enable();
        PlayerInputManager.Instance.playerControls.PlayerMovement.SetCallbacks(this);
    }

    void OnDisable()
    {
        // Disconnect input to prevent null references
        PlayerInputManager.Instance.playerControls.PlayerMovement.Disable();
        PlayerInputManager.Instance.playerControls.PlayerMovement.RemoveCallbacks(this);
    }

    #endregion
    #region Movement

    void Update()
    {
        // Check grounded state
        coyoteTime = groundCheck.IsGrounded() ? DEFAULT_COYOTE : coyoteTime;

        // Move the player
        // ----Move the player----
        if (movementInput == 0)
        {
            // If no player input - Decelerate
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, DECELERATION * Time.deltaTime);
        }
        else
        {
            // If player input - Move
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, movementInput * MAX_SPEED, ACCELERATION * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // Reduce coyote time
        coyoteTime = coyoteTime < 0 ? coyoteTime : coyoteTime - 1;
    }

    #endregion
    #region Input

    // Read the players horizontal input
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // If the player is currently or was recently
        if (coyoteTime >= 0)
        {
            rb.linearVelocityY = DEFAULT_JUMP_FORCE;
        }
        coyoteTime = -1;
    }

    public void OnCollisionEnter2D()
    {

    }
    #endregion
}
