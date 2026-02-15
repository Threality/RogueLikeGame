using Pathfinding;
using UnityEngine;

public class DirectionalSprites : MonoBehaviour
{
    public AIPath path;
    private bool facingLeft = false;

    void Update()
    {
        // Update facing left bool
        facingLeft = path.desiredVelocity.x > 0 ? false : true;

        // Point sprite in the right direction
        if (facingLeft) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);
    }
}
