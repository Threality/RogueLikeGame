using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

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
    public GameObject LaunchUI;
    public List<TextMeshProUGUI> hudMats = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> quotaMats = new List<TextMeshProUGUI>();
    public TextMeshProUGUI quotaCount;

    [Header("Alerts")]
    public GameObject alertUI;
    public TextMeshProUGUI alertText;

    [Header("Upgrades")]
    public TextMeshProUGUI craftingCost;
    public TextMeshProUGUI drillCost;

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
        LaunchUI.SetActive(false);
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
        Time.timeScale = 0f;
    }

    public void EnterLaunch()
    {
        LaunchUI.SetActive(true);
    }

    public void UpdateUI()
    {
        List<int> counts = HubManager.instance.GetHubMaterials();
        for (int i = 0; i < hudMats.Count; i++)
        {
            hudMats[i].text = counts[i].ToString();
        }

        for (int i = 0; i < quotaMats.Count; i++)
        {
            quotaMats[i].text = counts[i].ToString();
        }

        quotaCount.text = counts[0] + " / "+ HubManager.instance.GetQuota() + " Steel";
        craftingCost.text = HubManager.instance.GetCraftingUpgradeCost().ToString();
        drillCost.text = HubManager.instance.GetMiningUpgradeCost().ToString();
    }

    public void AlertUser(string text)
    {
        alertText.text = text;
        StopAllCoroutines();
        StartCoroutine(ShowAlert());
    }

    private IEnumerator ShowAlert()
    {
        alertUI.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        alertUI.SetActive(false);
    }
}