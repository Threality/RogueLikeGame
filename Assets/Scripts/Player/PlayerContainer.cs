using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContainer : MonoBehaviour
{
    public static PlayerContainer instance;
    public int MAX_HEALTH;
    public int health;
    public int[,] items = new int[9,5];

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    
}