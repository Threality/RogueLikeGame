using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour, InputSystem_Actions.IPlayerInteractionActions
{
    #region Setup
    void OnEnable()
    {
        // Connect player input and events to script
        PlayerInputManager.Instance.playerControls.PlayerInteraction.Enable();
        PlayerInputManager.Instance.playerControls.PlayerInteraction.SetCallbacks(this);
        PlayerManager.OnPlayerTakeDamage += TakeDamage;
        PlayerManager.OnPlayerHit += OnHit;
        PlayerManager.OnPlayerDeath += Death;
    }

    void OnDisable()
    {
        // Disconnect input and events to prevent null references
        PlayerInputManager.Instance.playerControls.PlayerInteraction.Disable();
        PlayerInputManager.Instance.playerControls.PlayerInteraction.RemoveCallbacks(this);
        PlayerManager.OnPlayerTakeDamage -= TakeDamage;
        PlayerManager.OnPlayerHit -= OnHit;
        PlayerManager.OnPlayerDeath -= Death;
    }

    #endregion
    #region Player Interaction

    private void OnHit(Context context)
    {
        // If the player does not have i-frames remaining - take damage
        if (PlayerContainer.instance.immunity < 0)
        {
            Debug.Log("Player was hit");
            PlayerManager.TriggerEvent(PlayerManager.Events.PlayerTakeDamage, context);
        }
    }

    private void TakeDamage(Context context)
    {
        // Damage player and prevent damage for a certain amount of time.
        PlayerContainer.instance.health -= context.damage ?? 1;
        PlayerContainer.instance.immunity  = PlayerContainer.instance.DEFAULT_IMMUNITY;
        
        // If the players health is less than or equal to 0 - kill the player
        if (PlayerContainer.instance.health <= 0 && PlayerContainer.instance.playerIsAlive)
        {
            PlayerContainer.instance.health = 0;
            PlayerManager.TriggerEvent(PlayerManager.Events.PlayerDeath, context);
        }
    }

    private void Death(Context context)
    {
        // Mark the player as dead
        PlayerContainer.instance.playerIsAlive = false;
        // Clear the inventory
        PlayerContainer.instance.inventory = new int[9, 5];
        // Save the game
        // Generate a new world
    }
    
    #endregion
    #region Inputs

    public void OnUseItem(InputAction.CallbackContext context)
    {
        // Use the item
    }
    #endregion
}