using UnityEngine;

public class LadderCheck : MonoBehaviour
{
    private bool isTouchingLadder = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log($"Collided with {collision.tag}");
        if (collision.CompareTag("Ladder"))
        {
            isTouchingLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Debug.Log($"Exited from {collision.tag}");
        if (collision.CompareTag("Ladder"))
        {
            isTouchingLadder = false;
        }
    }

    public bool IsTouchingLadder()
    {
        return isTouchingLadder;
    }
}
