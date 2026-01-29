using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.UI;
using UnityEngine;

public class WorkbenchManager : MonoBehaviour
{
    public static WorkbenchManager instance;
    
    [Header("Data")]
    public int space = 5;

    [Header("References")]
    public GameObject slotPrefab;
    public Transform workbenchParent;
    public GameObject workbenchUI;

    public List<Sprite> materialSprites = new List<Sprite>();

    private void Awake()
    {
        // Set up the instance of the inventory
        if (instance != null) Destroy(gameObject);
        else instance = this;
    }

    public void OpenWorkbench()
    {
        foreach (RecipeData recipe in HubManager.instance.GetAvailableRecipes())
        {
            // Instantiate object
            GameObject obj = Instantiate(slotPrefab, workbenchParent);

            // Set key information
            RecipeSlot slot = obj.GetComponent<RecipeSlot>();
            slot.Add(recipe);
        }
    }

    public void CloseWorkbench()
    {
        GameObject[] toDestroy = new GameObject[workbenchParent.childCount];
        for (int i = 0; i < workbenchParent.childCount; i++) toDestroy[i] = workbenchParent.GetChild(i).gameObject;
        for (int i = 0; i < workbenchParent.childCount; i++) Destroy(toDestroy[i]);
    }
}