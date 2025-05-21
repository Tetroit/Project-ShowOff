using Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;

public class TextRunner : MonoBehaviour
{
    public LineView LineView;
    [SerializeField] DialogueLine[] _lines;
    [SerializeField] bool _runOnStart;
    [SerializeField] bool _disableAfterFinish;
    [SerializeField] bool _useDialogueTime;

    public bool IsTextAreaActive { get;private set; }

    int currentDialogueIndex;
    void Start()
    {
        if(_runOnStart)
        {
            DisplayText();
        }
    }

    public void EnableTextArea()
    {
        IsTextAreaActive = true;
        LineView.gameObject.SetActive(IsTextAreaActive);
    }
    public void DisableTextArea()
    {
        IsTextAreaActive = false;
        LineView.gameObject.SetActive(IsTextAreaActive);
    }
    public void DisplayText()
    {
        if (_lines.Length == 0)
        {
            Debug.LogWarning("No text lines to display");
            return;
        }
        LineView.matchAudioTime = _useDialogueTime;

        EnableTextArea();
        RecursiveAdvance(0);
    }
    
    public void DisplayText(string txt)
    {
        if(string.IsNullOrEmpty(txt)) return;
        LineView.matchAudioTime = _useDialogueTime;

        EnableTextArea();
        LineView.RunLine(new DialogueLine("", txt));

    }

    void RecursiveAdvance(int i)
    {
        currentDialogueIndex = i;
        var line = _lines[i];
        i++;
        if (i > _lines.Length - 1)
            LineView.RunLine(line, () => { if (_disableAfterFinish) DisableTextArea(); });
        else
        {
            LineView.RunLine(line, () => {
                RecursiveAdvance(i);
            });
        }


    }

    public void InteruptDialogue()
    {
        var currentDialogue = _lines[currentDialogueIndex];
        if (currentDialogueIndex >= _lines.Length - 1)
            LineView.InterruptLine(currentDialogue, null);
        else
            LineView.InterruptLine(currentDialogue, () => RecursiveAdvance(++currentDialogueIndex));

    }
}
