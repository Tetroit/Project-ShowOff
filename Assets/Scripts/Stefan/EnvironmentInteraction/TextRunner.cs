using Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;

public class TextRunner : MonoBehaviour
{
    [SerializeField] LineView _lineView;
    [SerializeField] DialogueLine[] _lines;
    [SerializeField] bool _runOnStart;
    [SerializeField] bool _disableAfterFinish;

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
        _lineView.gameObject.SetActive(true);
    }
    public void DisableTextArea()
    {
        _lineView.gameObject.SetActive(false);
    }
    public void DisplayText()
    {
        if (_lines.Length == 0)
        {
            Debug.LogWarning("No text lines to display");
            return;
        }
        EnableTextArea();
        RecursiveAdvance(0);
    }
    
    public void DisplayText(string txt)
    {
        if(string.IsNullOrEmpty(txt)) return;
        _lineView.RunLine(new DialogueLine("", txt));

    }

    void RecursiveAdvance(int i)
    {
        currentDialogueIndex = i;
        var line = _lines[i];
        i++;
        if (i > _lines.Length - 1)
            _lineView.RunLine(line, () => { if (_disableAfterFinish) DisableTextArea(); });
        else
        {
            _lineView.RunLine(line, () => {
                RecursiveAdvance(i);
            });
        }


    }

    public void InteruptDialogue()
    {
        var currentDialogue = _lines[currentDialogueIndex];
        if (currentDialogueIndex >= _lines.Length - 1)
            _lineView.InterruptLine(currentDialogue, null);
        else
            _lineView.InterruptLine(currentDialogue, () => RecursiveAdvance(++currentDialogueIndex));

    }
}
