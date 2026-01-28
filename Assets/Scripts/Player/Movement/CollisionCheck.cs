using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    private bool isTouchingLadder = false;
    private bool isTouchingWorkbench = false;
    private List<GameObject> chestList = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log($"Collided with {collision.tag}");
        if (collision.CompareTag("Ladder"))
        {
            isTouchingLadder = true;
        }
        else if (collision.CompareTag("Chest"))
        {
            chestList.Add(collision.gameObject);
        }
        else if (collision.CompareTag("Workbench"))
        {
            isTouchingWorkbench = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Debug.Log($"Exited from {collision.tag}");
        if (collision.CompareTag("Ladder"))
        {
            isTouchingLadder = false;
        }
        else if (collision.CompareTag("Chest"))
        {
            chestList.Remove(collision.gameObject);
        }
        else if (collision.CompareTag("Workbench"))
        {
            isTouchingWorkbench = false;
        }
    }

    public bool IsTouchingLadder()
    {
        return isTouchingLadder;
    }

    public bool IsTouchingChest()
    {
        return chestList.Count >= 1;
    }

    public bool IsTouchingWorkbench()
    {
        return isTouchingWorkbench;
    }

    public ChestData GetItemsInChest()
    {
        return chestList[0].GetComponent<ChestData>();
    }
}
