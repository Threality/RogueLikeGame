using UnityEngine;
using UnityEngine.UI;

public class HealthbarManager : MonoBehaviour
{
    public static HealthbarManager instance;
    public Image[] icons;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else instance = this;
        
        icons = gameObject.GetComponentsInChildren<Image>();
    }

    public void UpdateHealthBar()
    {
        int curHealth = PlayerContainer.instance.health;

        for (int i = 0; i < 10; i++)
        {
            if (i >= curHealth) icons[i].gameObject.SetActive(false);
            else icons[i].gameObject.SetActive(true);
        }
    }
}
