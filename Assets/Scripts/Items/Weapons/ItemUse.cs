using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ItemUse : MonoBehaviour
{
    #region Setup

    [Header("References")]
    public InventoryManager inventoryManager;
    public LayerMask enemyLayer;
    public Transform projectileParent;

    [Header("Ladders")]
    public Tilemap ladderMap;
    public Tilemap groundMap;
    public Tilemap backgroundMap;
    public TileBase ladderTile;

    // Debugging variables
    private Vector2 lastCenter;
    private Vector2 lastSize;
    private float lastAngle;


    #endregion
    #region Use


    public void UseItem(ItemData data)
    {
        switch (data.useType)
        {
            case GameManager.UseType.None:
                break;
            case GameManager.UseType.Sword:
                UseSword(data);
                break;
            case GameManager.UseType.Gun:
                UseGun(data);
                break;
            default:
                Debug.LogWarning("Attempted to use item but did not provide data");
                break;
        }
    }

    public bool UseLadder()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        float distance = Vector2.Distance(transform.position, mousePos);
        if (distance > 10f) return false;

        Vector3Int cellPos = ladderMap.WorldToCell(mousePos);

        if (ladderMap.HasTile(cellPos)) return false;
        if (groundMap.HasTile(cellPos)) return false;
        if (backgroundMap.HasTile(cellPos)
         || ladderMap.HasTile(cellPos + Vector3Int.down)
         || groundMap.HasTile(cellPos + Vector3Int.down))
        {
            ladderMap.SetTile(cellPos, ladderTile);
            return true;
        }
        return false;
    }


    #endregion
    #region Attacks


    private void UseSword(ItemData data)
    {
        // Calculate box cast parameters
        Vector2 origin = transform.position;
        float offset = data.hitboxWidth / 2;
        Vector2 direction = GameManager.instance.GetMouseDirection() * offset;
        Vector2 hitCenter = origin + direction;
        Vector2 hitSize = new Vector2(data.hitboxWidth, data.hitboxHeight);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Cast box cast
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitCenter, hitSize, angle, enemyLayer);

        // Debuggin
        lastCenter = hitCenter;
        lastSize = hitSize;
        lastAngle = angle;

        // Damage each hit enemy
        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<Collider2D>().TryGetComponent(out EnemyInteraction enemyInteraction))
            {
                enemyInteraction.TakeDamage(data.damage);
            }
        }
    }

    private void UseGun(ItemData data)
    {
        Vector2 direction = GameManager.instance.GetMouseDirection();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(data.projectile, transform.position, Quaternion.Euler(0f, 0f, angle), projectileParent);
        projectile.GetComponent<ProjectileController>().Initialise(data, direction);
    }


    #endregion
    #region Visualisation

    private void OnDrawGizmos()
    {
        if (lastSize == Vector2.zero) return;

        Gizmos.color = Color.red;

        // Rotate the Gizmo to match the angle of the box
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(lastCenter, Quaternion.Euler(0, 0, lastAngle), Vector3.one);
        Gizmos.matrix = rotationMatrix;

        // Draw the box
        Gizmos.DrawWireCube(Vector3.zero, lastSize);

        // Reset matrix so other gizmos aren't rotated
        Gizmos.matrix = Matrix4x4.identity;
    }

    #endregion
}