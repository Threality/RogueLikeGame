using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;

public class EnemyManager : MonoBehaviour
{
    #region Setup

    [Header("Spawn Settings")]

    // Enemy variables
    public int ENEMY_SPAWN_RATE;
    public int MAX_ENEMIES;

    // Player variables
    public int MAX_PLAYER_VISION_X;
    public int MAX_PLAYER_VISION_Y;

    // List of all enemy prefabs
    public EnemyData[] ENEMY_POOL;

    public enum AIType
    {
        GroundMelee,
        FlyingMelee
    }
    public enum MovementMode
    {
        Chase,
        Search,
        Roam,
    }

    // References
    [Header("References")]
    public GameManager gameManager;
    public GameObject enemiesContainer;
    public GameObject player;
    public Transform targetContainer;

    // Tracking enemy related variables
    [Header("Current Values")]
    public List<GameObject> enemyInstances;


    #endregion
    #region Spawning


    void FixedUpdate()
    {
        // Get the difficulty and a random number
        float difficulty = GameManager.instance.GetDifficulty();
        int rand = Random.Range(0,  ENEMY_SPAWN_RATE);

        // Check whether an enemy should be spawned
        if (difficulty >= rand)
        {
            // Debug.Log("Attempting to spawn enemy");
            AttemptToSpawnEnemy();
        }
    }

    private void AttemptToSpawnEnemy()
    {
        // Attempt to find a location to spawn the enemy
        Vector2 spawnLocation = GetRandomSpawnLocation();
        if (spawnLocation == new Vector2()) return;

        if (enemyInstances.Count >= MAX_ENEMIES)
        {
            if (!AttemptToRemoveEnemy()) return;
        }

        // Ensures the number of enemies is below maximum 
        // even if removing an enemy was successful
        if (enemyInstances.Count >= MAX_ENEMIES) return;

        // Attempt to spawn enemy
        SpawnEnemy(spawnLocation);
    }

    private void SpawnEnemy(Vector2 position)
    {
        // Filter out enemies that cant spawn in this biome
        GameManager.Biomes currentBiome =  gameManager.GetCurrentBiome();
        List<EnemyData> validEnemies = ENEMY_POOL.Where(x => x.spawnBiomes.Contains(currentBiome)).ToList();

        // Return early if no enemies can be found
        if(validEnemies.Count == 0) return;

        // Find the enemy to spawn using the filtered weights and biomes
        EnemyData enemyToSpawn = GetEnemyToSpawn(validEnemies, gameManager.GetDifficulty());

        // Spawn the enemy in
        GameObject enemyObj = Instantiate(enemyToSpawn.prefab, position, Quaternion.identity, enemiesContainer.transform);

        // Initialise the data
        EnemyController controller = enemyObj.GetComponent<EnemyController>();
        if(controller != null) controller.Initialise(enemyToSpawn, this);

        // Store information
        enemyInstances.Add(enemyObj);

        // Create context for the event
        EnemyContext context = new EnemyContext();
        context.enemySpawned = enemyObj;
        context.spawnLocation = position;

        // Start the event
        EnemyEventManager.TriggerEvent(EnemyEventManager.Events.EnemySpawned, context);
    }

    private EnemyData GetEnemyToSpawn(List<EnemyData> enemies, float difficulty)
    {
        // Create a dictionary of all valid enemies
        Dictionary<EnemyData, float> weights = new Dictionary<EnemyData, float>();
        float totalWeight = 0f;

        // Loop through all the enemies and add their weights to the dictionary
        foreach (var enemy in enemies)
        {
            float curWeight = enemy.spawnWeight;

            switch (enemy.rarity)
            {
                // Adjust weighting based on rarity and difficulty
                // Check out animation curve later
                case GameManager.Rarity.Common:
                    if (difficulty > 2.5f) curWeight /= 2f;
                    break;
                case GameManager.Rarity.Uncommon:
                    if (difficulty <= 2.5f && difficulty >= 1.5f) curWeight *= 1.5f;
                    else if (difficulty > 2.5f) curWeight *= 2/3;
                    break;
                case GameManager.Rarity.Rare:
                    if (difficulty < 2f) curWeight /= 2f;
                    break;
                case GameManager.Rarity.Epic:
                    if (difficulty < 2.5f) curWeight /= 3f;
                    if (difficulty >= 2.5f) curWeight *= 2f;
                    break;
                case GameManager.Rarity.Legendary:
                    if (difficulty < 2f) curWeight = 0f;
                    else curWeight *=  difficulty / 3;
                    break;
                case GameManager.Rarity.Mythical:
                    if (difficulty < 3f) curWeight = 0f;
                    else curWeight *= difficulty / 6;
                    break;
            }

            // Add it to the dictionary
            weights.Add(enemy, curWeight);
            totalWeight += curWeight;
        }

        // Generate a random value
        float rand = Random.Range(0, totalWeight);
        float runningTotal = 0f;

        // Tally up until the random value is reached
        foreach (var weight in weights)
        {
            runningTotal += weight.Value;
            if (rand <= runningTotal)
            {  
                // Select the data for the enemy
                return weight.Key;
            }
        }

        // Return a default value
        Debug.LogWarning("Failed to find weighted enemy");
        return enemies[0];
    }

    private Vector2 GetRandomSpawnLocation()
    {
        // Check a maximum of 30 times to prevent loops
        for (int i = 0; i < 30; i++)
        {
            // Generate random direction and distance
            Vector2 direction = Random.insideUnitCircle.normalized;
            float distance = Random.Range(MAX_PLAYER_VISION_X, MAX_PLAYER_VISION_X * 1.5f);

            // Create position vector using random values
            Vector2 position = (Vector2) player.transform.position + direction * distance;

            // Check the position is near a node on the navigation path
            NNInfo nearestNode = AstarPath.active.GetNearest((Vector3) position, NNConstraint.Default);

            // If it was near one return the coords of the nearest node
            if (nearestNode.node != null) return (Vector2) nearestNode.position;
        }

        // Return a default value if no location is found
        return new Vector2(0, 0);
    }


    #endregion
    #region Removing

    public bool AttemptToRemoveEnemy()
    {
        // Get a list of all enemies that can be deleted
        List<GameObject> validInstances = GetEnemiesOffScreen();

        // Return early if no enemies qualify
        if (validInstances.Count ==  0) return false;

        // Debug.Log("Removing enemy");
        // Sort them by rarity, common first
        validInstances.Sort((a, b) =>
            a.GetComponent<EnemyController>().data.rarity
            .CompareTo(b.GetComponent<EnemyController>().data.rarity));


        // Remove the enemy from the list then delete it
        RemoveEnemy(validInstances[0]);
        
        return true;
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemyInstances.Remove(enemy);
        enemy.GetComponent<EnemyController>().RemoveTarget();
        Destroy(enemy);
    }

    public List<GameObject> GetEnemiesOffScreen()
    {
        // Calculate the distance to be considered off screen
        float distance = Mathf.Sqrt(MAX_PLAYER_VISION_X * MAX_PLAYER_VISION_X + MAX_PLAYER_VISION_Y * MAX_PLAYER_VISION_Y);

        // Get a list of all enemies off screen
        return enemyInstances
            .Where(e => Vector3.Distance(e.transform.position, player.transform.position) > distance)
            .ToList();
    }

    #endregion
}
