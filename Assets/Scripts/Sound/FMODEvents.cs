using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance;

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience;

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference footSteps;

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
