using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler
{
    public InventorySlot slot;

    void Awake()
    {
        slot = GetComponent<InventorySlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        slot.DisplayInformation();
    }
}
