using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using Pathfinding.Ionic.Zip;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("References")]
    public GameObject inventoryUI;
    public GameObject chestUI;
    public GameObject workbenchUI;
    public GameObject loadingUI;
    public GameObject inGameUI;
    public GameObject deathUI;
    public List<TextMeshProUGUI> hudMats = new List<TextMeshProUGUI>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    
    public void DisableAll()
    {
        inventoryUI.SetActive(false);
        chestUI.SetActive(false);
        WorkbenchManager.instance.CloseWorkbench();
        workbenchUI.SetActive(false);
        loadingUI.SetActive(false);
        inGameUI.SetActive(false);
        deathUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void EnterPlaying()
    {
        inGameUI.SetActive(true);
    }

    public void EnterChest()
    {
        chestUI.SetActive(true);
        inventoryUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void EnterInventory()
    {
        inventoryUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void EnterWorkbench()
    {
        WorkbenchManager.instance.OpenWorkbench();
        workbenchUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void EnterLoading()
    {
        loadingUI.SetActive(true);
    }

    public void EnterDeath()
    {
        deathUI.SetActive(true);
    }

    public void UpdateUI()
    {
        List<int> counts = HubManager.instance.GetHubMaterials();
        for (int i = 0; i < hudMats.Count; i++)
        {
            hudMats[i].text = counts[i].ToString();
        }
    }
}