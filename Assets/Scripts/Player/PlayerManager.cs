using System;
using UnityEngine;

public static class PlayerManager
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
    public static Action OnPlayerHit;
    public static Action OnPlayerUseItem;
    public static Action OnPlayerTakeDamage;
    public static Action OnPlayerDeath;

    #endregion
    #region Event Calling

    public static void TriggerEvent(Events curEvent)
    {
        switch (curEvent)
        {
            case Events.PlayerHit:
                OnPlayerHit.Invoke();
                break;
            case Events.PlayerUseItem:
                OnPlayerUseItem.Invoke();
                break;
            case Events.PlayerTakeDamage:
                OnPlayerTakeDamage.Invoke();
                break;
            case Events.PlayerDeath:
                OnPlayerDeath.Invoke();
                break;
            default:
                Debug.LogWarning("Attempted to call player event but no event was found!");
                break;
        }
    }
    #endregion
}