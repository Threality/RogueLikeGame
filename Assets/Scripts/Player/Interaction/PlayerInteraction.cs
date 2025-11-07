using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction: MonoBehaviour, InputSystem_Actions.IPlayerInteractionActions
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

    private void OnHit()
    {
        // If the player does not have I frames remaining - take damage
        if (PlayerContainer.instance.immunity == -1)
        {
            PlayerManager.TriggerEvent(PlayerManager.Events.PlayerTakeDamage/*, damage*/);
        }
    }

    private void TakeDamage()
    {
        PlayerContainer.instance.health -= 1;
        // If the players health is less than or equal to 0 - kill the player
        if (PlayerContainer.instance.health <= 0 && PlayerContainer.instance.playerIsAlive)
        {
            PlayerContainer.instance.health = 0;
            PlayerManager.TriggerEvent(PlayerManager.Events.PlayerDeath);
        }
    }

    private void Death()
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