using UnityEngine;
using UnityEngine.UI;

public class HealthbarManager : MonoBehaviour
{
    public static HealthbarManager instance;
    public Image[] icons;

    private void Awake()
    {
        // Create a singleton so other places can access
        if (instance != null) Destroy(gameObject);
        else instance = this;
        
        // Get all images from the healthbar
        icons = gameObject.GetComponentsInChildren<Image>();
    }

    public void UpdateHealthBar()
    {
        // Get the health needing to be displayed
        int curHealth = PlayerContainer.instance.health;

        // Show only the correct amount
        for (int i = 0; i < 10; i++)
        {
            if (i >= curHealth) icons[i].gameObject.SetActive(false);
            else icons[i].gameObject.SetActive(true);
        }
    }
}
