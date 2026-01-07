using UnityEngine;
using System;

#region Context

public class EnemyContext
{
    // All items must be nullable
    public GameObject enemySpawned;
    public Vector2 spawnLocation;
}


#endregion


public static class EnemyEventManager
{
    #region Setup
    public enum Events
    {
        EnemySpawned,
    }

    // Setup all events
    public static event Action<EnemyContext> OnEnemySpawned;


    #endregion
    #region Event Calling


    public static void TriggerEvent(Events curEvent, EnemyContext context)
    {
        // Debug.Log("Triggering enemy event");
        switch (curEvent)
        {
            case Events.EnemySpawned:
                OnEnemySpawned?.Invoke(context);
                break;
            default:
                Debug.LogWarning("Attempted to call enemy event but no event was found!");
                break;
        }
    }
    #endregion
}