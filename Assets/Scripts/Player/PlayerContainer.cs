using UnityEngine;

public class PlayerContainer : MonoBehaviour
{
    public static PlayerContainer instance;

    // Constant values
    public int MAX_HEALTH = 10;
    public int DEFAULT_IMMUNITY = 20;

    // Player stats
    public int health;
    public int immunity;
    public bool playerIsAlive;
    public int useCooldown;
    public GameObject player;

    private void Awake()
    {
        // Check it is the only instance
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Set default values
        health = MAX_HEALTH;
        immunity = DEFAULT_IMMUNITY;
        playerIsAlive = true;
        useCooldown = -1;
    }

    private void FixedUpdate()
    {
        // Constantly reduce immunity frames
        immunity = immunity < 0 ? immunity : immunity - 1;
        useCooldown = useCooldown < 0 ? useCooldown : useCooldown - 1;
    }
}