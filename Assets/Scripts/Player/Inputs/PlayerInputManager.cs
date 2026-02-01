using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;
    public InputSystem_Actions playerControls { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        playerControls = new InputSystem_Actions();
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }
}