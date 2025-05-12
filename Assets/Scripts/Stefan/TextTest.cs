using Dialogue;
using UnityEngine;

public class TextTest : MonoBehaviour
{
    [SerializeField] LineView lineView;
    [SerializeField] DialogueLine[] lines;
    int currentDialogueIndex;

    void Start()
    {
        RecursiveAdvance(0);
    }

    void RecursiveAdvance(int i)
    {
        currentDialogueIndex = i;
        var line = lines[i];
        i++;
        if (i > lines.Length - 1)
            lineView.RunLine(line);
        else
        {
            lineView.RunLine(line, () => {
                Debug.Log("Line finished");
                RecursiveAdvance(i);
            } );
        }    
            
            
    }

    public void InteruptDialogue()
    {
        var currentDialogue = lines[currentDialogueIndex];
        if (currentDialogueIndex >= lines.Length - 1)
            lineView.InterruptLine(currentDialogue, null);
        else
            lineView.InterruptLine(currentDialogue, () => RecursiveAdvance(++currentDialogueIndex));

    }
}
