using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PostProcessingManager : MonoBehaviour
{
    public Volume mainVolume;

    public List<Volume> volumes = new();
}
