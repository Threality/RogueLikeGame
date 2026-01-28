using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;

public class PlayerInteraction : MonoBehaviour, InputSystem_Actions.IPlayerInteractionActions
{
    #region Setup
    public InventoryManager inventoryManager;
    public ItemUse itemUse;
    public PlayerContainer playerContainer;
    public CollisionCheck collisionCheck;

    void OnEnable()
    {
        // Connect player input and events to script
        PlayerInputManager.Instance.playerControls.PlayerInteraction.Enable();
        PlayerInputManager.Instance.playerControls.PlayerInteraction.SetCallbacks(this);
        PlayerEventManager.OnPlayerTakeDamage += TakeDamage;
        PlayerEventManager.OnPlayerHit += OnHit;
        PlayerEventManager.OnPlayerDeath += Death;
    }

    void OnDisable()
    {
        // Disconnect input and events to prevent null references
        PlayerInputManager.Instance.playerControls.PlayerInteraction.Disable();
        PlayerInputManager.Instance.playerControls.PlayerInteraction.RemoveCallbacks(this);
        PlayerEventManager.OnPlayerTakeDamage -= TakeDamage;
        PlayerEventManager.OnPlayerHit -= OnHit;
        PlayerEventManager.OnPlayerDeath -= Death;
    }

    #endregion
    #region Player Interaction

    private void OnHit(PlayerContext context)
    {
        // If the player does not have i-frames remaining - take damage
        if (PlayerContainer.instance.immunity < 0)
        {
            Debug.Log("Player was hit");
            PlayerEventManager.TriggerEvent(PlayerEventManager.Events.PlayerTakeDamage, context);
        }
    }

    private void TakeDamage(PlayerContext context)
    {
        // Damage player and prevent damage for a certain amount of time.
        PlayerContainer.instance.health -= context.damage ?? 1;
        PlayerContainer.instance.immunity = PlayerContainer.instance.DEFAULT_IMMUNITY;

        // If the players health is less than or equal to 0 - kill the player
        if (PlayerContainer.instance.health <= 0 && PlayerContainer.instance.playerIsAlive)
        {
            PlayerContainer.instance.health = 0;
            PlayerEventManager.TriggerEvent(PlayerEventManager.Events.PlayerDeath, context);
        }
    }

    private void Death(PlayerContext context)
    {
        // Mark the player as dead
        PlayerContainer.instance.playerIsAlive = false;
        // Clear the inventory
        InventoryManager.instance.inventory = new List<ItemData>();
        // Save the game
        // Generate a new world
    }

    #endregion
    #region Inputs

    public void OnUseItem(InputAction.CallbackContext context)
    {
        // Only use this function on the initial press
        if (!context.performed) return;

        // Use the item if the player should be able to
        if (playerContainer.useCooldown < 0 && inventoryManager.equippedItem != null)
        {
            itemUse.UseItem(inventoryManager.equippedItem);
            playerContainer.useCooldown = inventoryManager.equippedItem.useTime;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (collisionCheck.IsTouchingChest() && !inventoryManager.IsInMenu())
        {
            inventoryManager.OpenWithChestLoot(collisionCheck.GetItemsInChest());
        }
    }

    public void OnPlaceLadder(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        itemUse.UseLadder();
    }
    #endregion
}