using DG.Tweening;
using Dialogue;
using System.Collections;
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
        Debug.Log("Current selected "+ gameObject);
    }

    public override void Deselect()
    {
        Debug.Log("Current deselected " + gameObject);


    }

    public void TurnLeft()
    {
        transform.DOLocalRotateQuaternion(Quaternion.AngleAxis(-180, Vector3.up), .3f);
    }

    public void TurnRight()
    {
        transform.DOLocalRotateQuaternion(Quaternion.AngleAxis(0, Vector3.up), .3f);

    }
}
