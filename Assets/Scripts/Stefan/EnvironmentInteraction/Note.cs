using DG.Tweening;
using Dialogue;
using System.Collections;
using UnityEngine;

public class Note : MonoBehaviour, IHoldable, ITextDisplayer
{
    [SerializeField, TextArea] string _text;
    [SerializeField] float _returnTime;
    [SerializeField] TextRunner _runner;
    Vector3 _initialPosition;
    Quaternion _initialRotation;

    public Transform Self => transform;

    static LineView _textDisplay;
    
    void Awake()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

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
    public Vector3 GetInitialPosition()
    {
        return _initialPosition;
    }

    public Quaternion GetInitialRotation()
    {
        return _initialRotation;
    }

    public IEnumerator Deselect()
    {
        yield return null;
        Deactivate();
    }

    public IEnumerator Interact()
    {
        yield return null;
        _runner.DisplayText(_text);
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
}
