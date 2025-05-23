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
    static Window _textDisplayWindow;
    
    void Awake()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        try
        {
            if (_textDisplay == null)
            {
                _textDisplay = FindFirstObjectByType<Canvas>().transform.FindDeepChild("NoteContentTextArea").GetComponent<LineView>();
                _textDisplayWindow = _textDisplay.GetComponent<Window>();
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
        AudioManager.instance.PlayOneShot(FMODEvents.instance.paperHandling, transform.position);

        //remove from window manager current selected
        WindowManager.Instance.CloseCurrentWindow();
    }

    public IEnumerator Interact()
    {
        yield return null;
        if (!WindowManager.Instance.TrySwitchWindow(_textDisplayWindow)) yield break;


        _runner.DisplayText(_text);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.paperHandling, transform.position);
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
