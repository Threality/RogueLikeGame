using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{
    #region Setup

    // Initialisation data
    public EnemyData data;
    private AIPath aIPath;
    private AIDestinationSetter aIDestinationSetter;
    private Transform playerTarget;
    private GameObject curTarget;

    // Movement variables
    [Header("Movement")]
    public int UPDATE_RATE;
    [Range(0f, 1f)] public float FORGET_CHANCE;
    public int roamDestChangeFrequency;
    private int tickNum;
    private int roamTick;
    private bool recentlySeenPlayer;
    private Transform lastSeenLocation;
    public EnemyManager.MovementMode curMode;

    // References
    [Header("References")]
    public LayerMask obstacleMask;
    public LayerMask playerMask;
    public EnemyManager enemyManager;

    private void Awake()
    {
        // Get references
        aIPath = GetComponent<AIPath>();
        aIDestinationSetter = GetComponent<AIDestinationSetter>();
    }


    #endregion
    #region Initialisation


    public void Initialise(EnemyData enemyData, EnemyManager manager)
    {
        // Save data to enemy
        data = Instantiate(enemyData);
        enemyManager = manager;

        // Store values with random variation
        data.moveSpeed *= Random.Range(0.9f, 1.1f);
        data.acceleration *= Random.Range(0.8f, 1.2f);
        GameObject obj = Instantiate(data.GraphicsPrefab, transform);
        obj.GetComponent<DirectionalSprites>().path = aIPath;

        // Find the player and set it as the target
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Put into roaming mode
        curMode = EnemyManager.MovementMode.Roam;
        aIPath.maxSpeed = data.moveSpeed * 0.33f;
        aIPath.maxAcceleration = data.acceleration * 0.33f;

        // Set sizing
        transform.localScale = new Vector3(data.width, data.height, transform.localScale.z);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector3(data.width * 0.8f, data.height * 0.8f, transform.localScale.z);
        gameObject.GetComponent<BoxCollider2D>().size = new Vector3(data.width * 1.1f, data.height * 1.1f, transform.localScale.z);

        // Set up other instance stats
        gameObject.GetComponent<EnemyInteraction>().Initialise(data, enemyManager);


        // Generate a transform to enable setting custom targets later
        GameObject temp = new GameObject("Enemy Path Target");
        temp.transform.SetParent(enemyManager.targetContainer);
        curTarget = temp;
    }


    #endregion
    #region Movement


    void FixedUpdate()
    {    
        tickNum++;

        // Only update the enemy a few times per second
        if (tickNum % UPDATE_RATE != 0) return;

        // Check what state the enemy should be in
        if (CanSeePlayer())
        {
            // Enemy can see the player so mark the location
            recentlySeenPlayer = true;
            lastSeenLocation = playerTarget.transform;
            curMode = EnemyManager.MovementMode.Chase;
        }
        else if (recentlySeenPlayer)
        {
            // If the player cannot currently see the player but has seen them recently
            // Go into search mode
            curMode = EnemyManager.MovementMode.Search;

            // Randomly forget the players location and move to roaming
            if (Random.value <= FORGET_CHANCE)
            {
                curMode = EnemyManager.MovementMode.Roam;
                recentlySeenPlayer = false;
            }

        }
        else
        {
            // If the player has not been seen recently roam
            curMode = EnemyManager.MovementMode.Roam;
        }

        // Apply the movement mode
        HandleMovement();
    }

    private void HandleMovement()
    {
        switch (curMode)
        {
            // When chasing the enemy moves at full speed towards player
            case EnemyManager.MovementMode.Chase:
                aIDestinationSetter.target = playerTarget.transform;
                aIPath.maxSpeed = data.moveSpeed;
                aIPath.maxAcceleration = data.acceleration;
                break;
            // When searching the enemy moves slightly slower towards the players last known location
            case EnemyManager.MovementMode.Search:
                aIDestinationSetter.target = lastSeenLocation;
                aIPath.maxSpeed = data.moveSpeed;
                aIPath.maxAcceleration = data.acceleration;
                break;
            // When roaming the enemy moves slowly and randomly
            case EnemyManager.MovementMode.Roam:
                // Uses a roam tick value to make adjusting the target less frequent
                if (roamTick % roamDestChangeFrequency == 0)
                {
                    // Generate a random direction
                    float randX = Random.Range(5f, 10f);
                    float randY = Random.Range(3f, 7f);
                    randX *= Random.value < 0.5f ? 1 : -1;
                    randY *= Random.value < 0.5f ? 1 : -1;

                    // Set the target to these randomly selected nearby coords
                    curTarget.transform.position = new Vector2(transform.position.x + randX, transform.position.y + randY);
                    aIDestinationSetter.target = curTarget.transform;
                    aIPath.maxSpeed = data.moveSpeed * 0.33f;
                    aIPath.maxAcceleration = data.acceleration * 0.33f;
                }
                roamTick++;
                break;
        }
    }

    private bool CanSeePlayer()
    {
        // Calculate directions and positions
        Vector2 curLocation = transform.position;
        Vector2 direction = playerTarget.position - transform.position;
        float distance = direction.magnitude;

        // When enemy is standing still give them vision of everything
        if (aIPath.velocity.sqrMagnitude > 0.05f)
        {
            // Check player is visible
            float dot = Vector2.Dot(direction.normalized, aIPath.velocity.normalized);
            if (dot <= 0) return false;
        }

        // Cast a raycast
        RaycastHit2D hit = Physics2D.Raycast(curLocation, direction.normalized, distance, obstacleMask | playerMask);
        Debug.DrawRay(curLocation, direction, Color.green);

        // If it hits nothing return false
        if (hit.collider == null) return false;

        // If it hits the player return true else false
        return hit.collider.CompareTag("Player");
    }


    #endregion
    #region Remove

    public void RemoveTarget()
    {
        Destroy(curTarget);
    }

    #endregion
}
