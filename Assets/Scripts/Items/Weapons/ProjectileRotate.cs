using UnityEngine;

public class ProjectileRotate : MonoBehaviour
{
    // References
    public Rigidbody2D rb;
    
    private void LateUpdate()
    {
        // Rotate projectile sprite
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
