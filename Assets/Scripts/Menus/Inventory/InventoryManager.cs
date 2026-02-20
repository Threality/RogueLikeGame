using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;

public class InventoryManager : MonoBehaviour, InputSystem_Actions.IInventoryActions
{
    #region Setup

    // Create an instance of the inventory
    public static InventoryManager instance;

    // Store inventory data
    [Header("Data")]
    public List<ItemData> inventory = new  List<ItemData>();
    private InventorySlot[] inventorySlots;
    private InventorySlot[] chestSlots;
    public int space = 20;
    public bool inventoryDoesPause;
    public ItemData equippedItem;
    public ChestData chestData;
    public bool chestMenu;

    // References for the UI
    [Header("References")]
    public GameObject inventoryUI;
    public GameObject chestUI;
    public GameObject slotPrefab;
    public Transform inventoryParent;
    public Transform chestParent;
    // Default item for debugging
    public ItemData defaultItem;
    public InformationDisplayer informationDisplayer;
    public Color defaultColor;
    public Color equippedColor;


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
        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            InitialiseSlots();
        }
    }


    #endregion
    #region Item Handling


    public void InitialiseSlots()
    {
        for (int i = 0; i < space; i++)
        {
            // Instantiate object
            GameObject obj = Instantiate(slotPrefab, inventoryParent);

            // Set key information
            InventorySlot slot = obj.GetComponent<InventorySlot>();
            slot.slotNum = i;
            slot.inChest = false;
            slot.informationDisplayer = informationDisplayer;
            slot.inventoryManager = this;
        }

        for (int i = 0; i < space; i++)
        {
            // Instantiate object
            GameObject obj = Instantiate(slotPrefab, chestParent);

            // Set key information
            InventorySlot slot = obj.GetComponent<InventorySlot>();
            slot.slotNum = i;
            slot.inChest = true;
            slot.informationDisplayer = informationDisplayer;
            slot.inventoryManager = this;
        }

        inventorySlots = inventoryParent.GetComponentsInChildren<InventorySlot>();
        chestSlots = chestParent.GetComponentsInChildren<InventorySlot>();

        // Hide it when starting the game
        UpdateUI();
        inventoryUI.SetActive(false);
        chestUI.SetActive(false);

        // Enable input
        PlayerInputManager.Instance.playerControls.Inventory.Enable();
        PlayerInputManager.Instance.playerControls.Inventory.SetCallbacks(this);
    }

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

    public void EquipItem(int itemNum)
    {
        // Store a reference to the data
        equippedItem = inventorySlots[itemNum].data;

        // Update visuals for the player
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.slotNum == itemNum) slot.SetColor(equippedColor);
            else slot.SetColor(defaultColor);
        }
    }

    
    #endregion
    #region Interactables

    
    public void OpenWithChestLoot(ChestData data)
    {
        GameManager.instance.ChangeGameState(GameManager.GameState.ChestMenu);
        chestMenu = true;
        chestData = data;
        informationDisplayer.ClearSlots();

        UpdateUI();
    }

    public void MoveItem(ItemData data, bool inChest)
    {
        // Move direction based on where item is stored
        if (!inChest)
        {
            if (chestData.HasSpace())
            {
                chestData.AddToInventory(data);
                inventory.Remove(data);
            }
        }
        else
        {
            if (instance.space > instance.inventory.Count)
            {
                inventory.Add(data);
                chestData.RemoveFromInventory(data);
            }
        }

        UpdateUI();        
    }


    #endregion
    #region UI


    private void UpdateUI()
    {
        if (inventorySlots == null || inventorySlots.Length == 0)
        {
            InitialiseSlots();
        }

        // Update all inventorySlots
        for (int i = 0; i < space; i++)
        {
            if (i < inventory.Count) inventorySlots[i].Add(inventory[i]);
            else inventorySlots[i].Clear();
        }
        
        if (!chestData) return;
        // Update all chestSlots
        for (int i = 0; i < space; i++)
        {
            if (i < chestData.inventory.Count) chestSlots[i].Add(chestData.inventory[i]);
            else chestSlots[i].Clear();
        }
    }


    #endregion
    #region Input


    public void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        chestMenu = false;

        informationDisplayer.ClearSlots();

        // Open or close inventory
        if (GameManager.instance.GetGameState() == GameManager.GameState.Playing) GameManager.instance.ChangeGameState(GameManager.GameState.InventoryMenu);
        else if (GameManager.instance.GetGameState() == GameManager.GameState.InventoryMenu) GameManager.instance.ChangeGameState(GameManager.GameState.Playing);
    }
    #endregion
}