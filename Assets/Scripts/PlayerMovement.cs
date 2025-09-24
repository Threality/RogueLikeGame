using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Allow for editing variables in the inspector
    [Header("Base Movement Settings")]
    public float MAX_SPEED = 10f;
    public float ACCELERATION = 3f;

    [Header("Jump Settings")]
    public float DEFAULT_JUMP_FORCE = 3f;
    public int DEFAULT_COYOTE = 5;

    // Set up hidden values 
    private float coyoteTime = 0;
    private int groundCount = 0;
    private Rigidbody2D rb;

    void Update()
    {
        
    }
}
