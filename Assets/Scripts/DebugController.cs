using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(PlayerInteraction))]
public class InteractionDebugger : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerInteraction interaction = (PlayerInteraction)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Hit Player"))
        {
            Debug.Log("Attempting to hit player");
            PlayerEventManager.TriggerEvent(PlayerEventManager.Events.PlayerHit, new PlayerContext());
        }
    }
}

[CustomEditor (typeof(InventoryManager))]
public class InventoryDebugger : Editor
{
    public override void OnInspectorGUI()
    {
        InventoryManager inventory = (InventoryManager)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Add item"))
        {
            InventoryManager.instance.Add(InventoryManager.instance.defaultItem);
        }
    }
}