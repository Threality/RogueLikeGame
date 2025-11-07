using UnityEngine;

public class PlayerContainer : MonoBehaviour
{
    public static PlayerContainer instance;
    public int MAX_HEALTH = 10;
    public int health;
    public int DEFAULT_IMMUNITY = 5;
    public int immunity;
    public int[,] inventory = new int[9, 5];
    public bool playerIsAlive;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Set default values
        health = MAX_HEALTH;
        immunity = DEFAULT_IMMUNITY;
        playerIsAlive = true;
    }
    
    private void FixedUpdate()
    {
        // Constantly reduce immunity frames
        immunity = immunity < 0 ? immunity : immunity - 1;
    }
}