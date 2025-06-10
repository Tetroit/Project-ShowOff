using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance;

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience;

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference footSteps;
    [field: SerializeField] public EventReference paperHandling;
    [field: SerializeField] public EventReference keyPickup;
    [field: SerializeField] public EventReference woodBreak;

    [field: Header("Dialogue")]
    [field: SerializeField] public EventReference voicemail;

    [field: Header("Music")]
    [field: SerializeField] public EventReference followMusic;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
