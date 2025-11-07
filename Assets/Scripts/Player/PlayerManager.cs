using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public enum Events
    {
        PlayerHit
    }
    public Action OnPlayerHit;

    public void TriggerEvent(Events curEvent)
    {
        switch (curEvent)
        {
            case 0: // Player is damaged
                OnPlayerHit.Invoke();
                break;
            default:
                Debug.LogWarning("Attempted to call player event but no event was found!");
                break;
        }
    }
}