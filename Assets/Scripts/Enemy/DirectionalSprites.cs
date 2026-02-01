using Pathfinding;
using UnityEngine;

public class DirectionalSprites : MonoBehaviour
{
    public AIPath path;
    private bool facingLeft = false;

    void Update()
    {
        facingLeft = path.desiredVelocity.x > 0 ? false : true;
        if (facingLeft) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);
    }
}
