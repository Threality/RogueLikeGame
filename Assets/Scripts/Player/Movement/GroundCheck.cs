using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public int groundCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundCount += 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            groundCount -= 1;
        }
    }

    public bool IsGrounded()
    {
        return (groundCount > 0);
    }
}
