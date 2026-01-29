using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/*
Code==Done
jigglephysics==extra
mr fat computer guy =pleased
*/

public class PlayerInteraction : MonoBehaviour, InputSystem_Actions.IPlayerInteractionActions
{
    #region Setup
    public InventoryManager inventoryManager;
    public ItemUse itemUse;
    public PlayerContainer playerContainer;
    public CollisionCheck collisionCheck;

    // Data
    private float regenProgress = 0f;
    private float regenSpeed = 3f;

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


    private void Update()
    {
        if (PlayerContainer.instance.health == PlayerContainer.instance.MAX_HEALTH) return;

        regenProgress += regenSpeed * Time.deltaTime / 60f;

        if (regenProgress >= 1f)
        {
            regenProgress -= 1;
            PlayerContainer.instance.health += 1;
            HealthbarManager.instance.UpdateHealthBar();
        }
    }

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

        HealthbarManager.instance.UpdateHealthBar();
        regenProgress = 0f;
    }

    private void Death(PlayerContext context)
    {
        // Mark the player as dead
        PlayerContainer.instance.playerIsAlive = false;
        // Clear the inventory
        InventoryManager.instance.inventory = new List<ItemData>();
        // Delete save data
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
        if (!context.performed) return;
        if (GameManager.instance.GetGameState() != GameManager.GameState.Playing)
        {
            GameManager.instance.ChangeGameState(GameManager.GameState.Playing);
            return;
        }

        if (collisionCheck.IsTouchingChest())
        {
            ChestData chest = collisionCheck.GetItemsInChest();
            chest.Open();
            inventoryManager.OpenWithChestLoot(chest);
        }
        else if (collisionCheck.IsTouchingWorkbench())
        {
            GameManager.instance.ChangeGameState(GameManager.GameState.WorkbenchMenu);
        }
    }

    public void OnPlaceLadder(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (itemUse.UseLadder()) HubManager.instance.PlacedLadderCost();
    }
    #endregion
}