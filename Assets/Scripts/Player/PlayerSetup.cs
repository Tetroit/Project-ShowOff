using Dialogue;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[ExecuteInEditMode]
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
            //var stateManager = FindFirstObjectByType<GameStateManager>();

            var interactionTextRunner = interactionManager.GetComponent<TextRunner>();
            var pickupManager = interactionManager.GetComponent<PickupManager>();

            //var holdManager = interactionManager.GetComponent<ConnorHold>();
            var itemInventory = canvas.transform.FindDeepChild("Inventory").GetComponent<InventoryView>();
            var arms = interactionManager.GetComponentsInChildren<InventoryItemView>(true);
            var book = canvas.GetComponentInChildren<Book>(true);
            
            
            interactionTextRunner.LineView = canvas.transform.FindDeepChild("InteractionTextArea").GetComponent<LineView>();
            EditorUtility.SetDirty(interactionTextRunner);

            pickupManager.KeyInventory = canvas.transform.FindDeepChild("KeyInventory").GetComponent<InventoryView>();
            EditorUtility.SetDirty(pickupManager);
            itemInventory.Clear();
            foreach (var arm in arms)
                itemInventory.AddItem(arm);
            itemInventory.AddItem(book);
            itemInventory.User = interactionManager.gameObject;
            EditorUtility.SetDirty(itemInventory);

            pickupManager.KeyInventory.User = interactionManager.gameObject;
            EditorUtility.SetDirty(pickupManager.KeyInventory);

            //UnityEventUtility.CleanMissingUnityEvents(holdManager);
            //UnityEventUtility.AddPersistentListenerIfMissing(holdManager.Interacted, holdManager, (x,y) => stateManager.SwitchToUI());
            //UnityEventUtility.AddPersistentListenerIfMissing(holdManager.Dismissed, holdManager, (x, y) => stateManager.SwitchToPlay());
        }
    }
#endif


}
