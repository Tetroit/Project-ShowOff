using Dialogue;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn;

public class TextRunner : MonoBehaviour
{
    [SerializeField] LineView _lineView;
    [SerializeField] DialogueLine[] _lines;
    [SerializeField] bool _runOnStart;
    int currentDialogueIndex;
    InputSystem_Actions inputActions;
    void Start()
    {
        //inputActions = new InputSystem_Actions();
        //inputActions.Player.P.performed += StartText;
        if(_runOnStart)
        {
            EnableTextArea();
            DisplayText();
        }
    }

    private void StartText(InputAction.CallbackContext context)
    {
        EnableTextArea();
        DisplayText();
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
        if (_lines.Length == 0) return;

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
            _lineView.RunLine(line, () => DisableTextArea());
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
