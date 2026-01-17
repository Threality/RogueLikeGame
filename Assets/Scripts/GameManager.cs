using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Create instance
    public static GameManager instance;

    // Get important references
    public GameObject player;
    public GameObject hub;

    public enum Biomes
    {
        Forest,
        Grassland,
        IceCave,
        MagmaCave
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythical
    }
    
    public enum UseType
    {
        None,
        Sword,
        Gun,
    }

    private void Awake()
    {
        // Check it is the only instance
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public float GetDifficulty()
    {
        // Call distance function with player and hub
        float dist = GetDistanceToObject(player, hub);

        // Prevent undefined values and exit early with a default value
        if (dist <= 10)
        {
            return 1f;
        }
                
        // Return log representing difficulty
        float diff = Mathf.Log10(dist);
        return diff;
    }

    public Biomes GetCurrentBiome()
    {
        // Link to terrain script to generate accurate result
        // For now return a default value
        return Biomes.Forest;
    }
    
    public float GetDistanceToObject(GameObject obj1, GameObject obj2)
    {
        // Calculate the change in x and y
        float x = obj1.transform.position.x - obj2.transform.position.x;
        float y = obj1.transform.position.y - obj2.transform.position.y;

        // Use pythagoras to calculate distance
        return Mathf.Sqrt(x * x + y * y);
    }

    public Vector2 GetMouseDirection()
    {
        Vector2 mouseScreenLocation = Mouse.current.position.ReadValue();
        Vector3 mouseWorldLocation = Camera.main.ScreenToWorldPoint(mouseScreenLocation);

        Vector2 direction = mouseWorldLocation - player.transform.position;
        return direction.normalized;
    }
}
