using UnityEngine;
using System;

#region Context

public class PlayerContext
{
    // All items must be nullable
    public int? damage;
    public GameObject attacker;
}


#endregion


public static class PlayerEventManager
{
    #region Setup
    public enum Events
    {
        PlayerHit,
        PlayerUseItem,
        PlayerTakeDamage,
        PlayerDeath,
    }

    // Setup all events
    public static event Action<PlayerContext> OnPlayerHit;
    public static event Action<PlayerContext> OnPlayerUseItem;
    public static event Action<PlayerContext> OnPlayerTakeDamage;
    public static event Action<PlayerContext> OnPlayerDeath;


    #endregion
    #region Event Calling


    public static void TriggerEvent(Events curEvent, PlayerContext context)
    {
        Debug.Log("Triggering player event");
        switch (curEvent)
        {
            case Events.PlayerHit:
                OnPlayerHit?.Invoke(context);
                break;
            case Events.PlayerUseItem:
                OnPlayerUseItem?.Invoke(context);
                break;
            case Events.PlayerTakeDamage:
                OnPlayerTakeDamage?.Invoke(context);
                break;
            case Events.PlayerDeath:
                OnPlayerDeath?.Invoke(context);
                break;
            default:
                Debug.LogWarning("Attempted to call player event but no event was found!");
                break;
        }
    }
    #endregion
}