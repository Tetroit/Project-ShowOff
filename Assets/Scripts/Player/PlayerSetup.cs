using Dialogue;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [SerializeField] bool _startSetup;
#if UNITY_EDITOR
    void OnValidate()
    {
        if (_startSetup)
        {
            _startSetup = false;
            //var player = FindFirstObjectByType<PlayerFSM>();
            var interactionManager = FindFirstObjectByType<InteractionManager>();
            var canvas = FindFirstObjectByType<Canvas>();
            var stateManager = FindFirstObjectByType<GameStateManager>();

            var interactionTextRunner = interactionManager.GetComponent<TextRunner>();
            var pickupManager = interactionManager.GetComponent<PickupManager>();

            var holdManager = interactionManager.GetComponent<ConnorHold>();

            
            UnityEventUtility.AddPersistentListenerIfMissing(holdManager.Interacted, holdManager, stateManager.SwitchToUI);
            UnityEventUtility.AddPersistentListenerIfMissing(holdManager.Dismissed, holdManager, stateManager.SwitchToPlay);
            interactionTextRunner.LineView = canvas.transform.Find("InteractionTextArea").GetComponent<LineView>();
            pickupManager.KeyInventory = canvas.transform.Find("KeyInventory").GetComponent<InventoryView>();

        }
    }
#endif


}
