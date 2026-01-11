using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour, InputSystem_Actions.IInventoryActions
{
    #region Setup

    public static InventoryManager instance;

    // Store inventory data
    [Header("Data")]
    public List<ItemData> inventory = new  List<ItemData>();
    private InventorySlot[] slots;
    public int space = 20;
    public bool inventoryDoesPause;

    // References for the UI
    [Header("References")]
    public GameObject inventoryUI;
    public GameObject slotPrefab;
    public Transform inventoryParent;
    // Default item for debugging
    public ItemData defaultItem;
    public InformationDisplayer informationDisplayer;

    void OnEnable()
    {
        // Connect player input
        PlayerInputManager.Instance.playerControls.Inventory.Enable();
        PlayerInputManager.Instance.playerControls.Inventory.SetCallbacks(this);
    }

    void OnDisable()
    {
        // Disconnect input to prevent null references
        PlayerInputManager.Instance.playerControls.Inventory.Disable();
        PlayerInputManager.Instance.playerControls.Inventory.RemoveCallbacks(this);
    }

    void Awake()
    {
        // Set up the instance of the inventory
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    void Start()
    {
        for (int i = 0; i < space; i++)
        {
            // Instantiate object
            GameObject obj = Instantiate(slotPrefab, inventoryParent);

            // Set key information
            InventorySlot slot = obj.GetComponent<InventorySlot>();
            slot.slotNum = i;
            slot.informationDisplayer = informationDisplayer;
        }

        slots = inventoryParent.GetComponentsInChildren<InventorySlot>();

        // Hide it when starting the game
        UpdateUI();
        inventoryUI.SetActive(false);

        // Enable input
        PlayerInputManager.Instance.playerControls.Inventory.Enable();
        PlayerInputManager.Instance.playerControls.Inventory.SetCallbacks(this);
    }


    #endregion
    #region Item Handling


    public bool Add(ItemData item)
    {
        // Check if there is room in the inventory
        if (inventory.Count >= space)
        {
            Debug.Log("Inventory is full");
            return false;
        }

        // Add the item to the inventory
        inventory.Add(item);
        UpdateUI();
        return true;
    }

    public void Remove(ItemData item)
    {
        // Remove the item from the inventory
        inventory.Remove(item);
        UpdateUI();
    }

    #endregion
    #region UI

    void UpdateUI()
    {
        // Update all slots
        for (int i = 0; i < space; i++)
        {
            if (i < inventory.Count)
            {
                slots[i].Add(inventory[i]);
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    #endregion
    #region Input


    public void OnOpenInventory(InputAction.CallbackContext context)
    {
        informationDisplayer.ClearSlots();

        // Open or close inventory
        inventoryUI.SetActive(!inventoryUI.activeSelf);

        if (inventoryUI.activeSelf && inventoryDoesPause) Time.timeScale = 0;
        else Time.timeScale = 1f;
    }
    #endregion
}