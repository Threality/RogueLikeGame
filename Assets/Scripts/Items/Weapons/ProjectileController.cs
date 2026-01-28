using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    // References
    public ItemData data;
    public Rigidbody2D rb;

    public void Initialise(ItemData itemData, Vector2 direction)
    {
        // Load item data
        data = itemData;

        // Set speed
        rb.linearVelocity = direction.normalized * data.P_MAX_SPEED;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Dont detect the player
        if (collision.CompareTag("Player") || collision.CompareTag("Chest")) return;
        
        // Damage an enemy
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyInteraction>().TakeDamage(data.damage);
        }

        // Remove the projectile if it hits anything but the player
        Destroy(gameObject);
    }
}