using Dialogue;
using UnityEngine;

public class BookPage : InventoryItemView, ITextDisplayer
{
    public string Text;

    [SerializeField] TextRunner _runner;
    static LineView _textDisplay;

    void Awake()
    {
        try
        {
            if (_textDisplay == null)
            {
                _textDisplay = FindFirstObjectByType<Canvas>().transform.FindDeepChild("NoteContentTextArea").GetComponent<LineView>();
            }

            if (_runner == null) GetComponent<TextRunner>();
            _runner.LineView = _textDisplay;

        }
        catch { }

    }

    public void Activate()
    {
        _runner.EnableTextArea();
    }

    public void Deactivate()
    {
        _runner.InteruptDialogue();
        _runner.DisableTextArea();
    }

    public void Toggle()
    {
        if (_runner.IsTextAreaActive)
            Deactivate();
        else
            Activate();
    }

    public override void Select()
    {
        //start rotation
        //rotate left or right based on toggle

    }

    public override void Deselect()
    {
        Deactivate();
    }
}
