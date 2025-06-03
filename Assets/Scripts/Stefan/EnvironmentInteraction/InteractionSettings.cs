using UnityEngine;

[CreateAssetMenu(fileName = "InteractionSettings", menuName = "Stefan/InteractionSettings")]
public class InteractionSettings : ScriptableObject
{
    public bool NoteShowText;

    public void ToggleNoteShowText()
    {
        NoteShowText = !NoteShowText;
    }
}
