using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WorkbenchManager : MonoBehaviour
{
    public static WorkbenchManager instance;

    [Header("References")]
    public WorldGenerator worldGenerator;
    public GameObject slotPrefab;
    public Transform workbenchParent;
    public GameObject workbenchUI;
    public TextMeshProUGUI aggressionCount;
    public GameObject player;

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
        aggressionCount.text = "Aggression:" + "\n" + Mathf.Round(GameManager.instance.GetDifficulty(player) - GameManager.instance.GetGalacticDepth()) + " + " + GameManager.instance.GetGalacticDepth();
    }

    public void CloseWorkbench()
    {
        GameObject[] toDestroy = new GameObject[workbenchParent.childCount];
        for (int i = 0; i < workbenchParent.childCount; i++) toDestroy[i] = workbenchParent.GetChild(i).gameObject;
        for (int i = 0; i < workbenchParent.childCount; i++) Destroy(toDestroy[i]);
    }

    public void AttemptLaunch()
    {
        // Check the player has the required resources
        if (HubManager.instance.blackSteel < HubManager.instance.GetQuota())
        {
            MenuManager.instance.AlertUser("You do not have enough steel!");
            return;
        }

        // Launch
        GameManager.instance.ChangeGameState(GameManager.GameState.LaunchMenu);
        Time.timeScale = 0f;
    }

    public void LaunchToNewPlanet()
    {
        GameManager.instance.ChangeGameState(GameManager.GameState.LoadingMenu);
        StopAllCoroutines();
        StartCoroutine(WaitToGenWorld());
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator WaitToGenWorld()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        worldGenerator.NewWorld(false);
    }
}