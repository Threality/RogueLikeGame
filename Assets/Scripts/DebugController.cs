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
