using UnityEngine;

/**
 * A nice and empty note component.
 * That means this component hardly takes up any data/performance in your final build.
 */ 
[AddComponentMenu("Tools/Notes/Comment")]
public class Comment : MonoBehaviour  {
    public string noteText = "";
    public int noteType = 0;
}


