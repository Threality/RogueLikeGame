using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class EnemyInteraction : MonoBehaviour
{
    // References
    public EnemyData data;
    public EnemyManager enemyManager;

    // Stats
    public int health;

    public void Initialise(EnemyData enemyData, EnemyManager manager)
    {
        data = enemyData;
        enemyManager = manager;
        health = enemyData.MAX_HEALTH;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) enemyManager.RemoveEnemy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PlayerContainer.instance.immunity < 0)
        {
            PlayerContext context = new PlayerContext();
            context.attacker = gameObject;
            context.damage = data.damage;

            PlayerEventManager.TriggerEvent(PlayerEventManager.Events.PlayerHit, context);
        }
    }
}