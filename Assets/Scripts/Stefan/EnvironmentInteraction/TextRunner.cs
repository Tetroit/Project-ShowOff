using Dialogue;
using FMODUnity;
using UnityEngine;

public class TextRunner : MonoBehaviour
{
    public LineView LineView;
    [SerializeField] DialogueLine[] _lines;
    [SerializeField] bool _runOnStart;
    [SerializeField] bool _disableAfterFinish;
    [SerializeField] bool _useDialogueTime;

    public bool IsTextAreaActive { get;private set; }

    [SerializeField] EventReference voiceLines;

    int currentDialogueIndex;
    DialogueLine _currentDialogue;

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
        //Addition for voicelines
        if(AudioManager.instance != null && !voiceLines.IsNull)
        {
            AudioManager.instance.PlayOneShot(voiceLines, Vector3.zero);
        }

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
        _currentDialogue = new DialogueLine("", txt);
        LineView.RunLine(_currentDialogue, ()=> _currentDialogue = null);
    }

    void RecursiveAdvance(int i)
    {
        currentDialogueIndex = i;
        var line = _lines[i];
        _currentDialogue = line;
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
        if (_currentDialogue == null) return;

        if (currentDialogueIndex >= _lines.Length - 1)
            LineView.InterruptLine(_currentDialogue, null);
        else
            LineView.InterruptLine(_currentDialogue, () => RecursiveAdvance(++currentDialogueIndex));
    }
}
