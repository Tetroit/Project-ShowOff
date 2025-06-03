using Dialogue;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour, IHoldable, ITextDisplayer
{
    [SerializeField] string _title = "Old Note";
    [SerializeField, TextArea] string _text;

    [SerializeField] float _returnTime;
    [SerializeField] TextRunner _runner;
    [SerializeField] InteractionSettings _interactionSettings;
    [field: SerializeField] public BookPage PagePrefab { get; private set; }

    Vector3 _initialPosition;
    Quaternion _initialRotation;

    public string Text => _text;
    public string Title => _title;

    public Transform Self => transform;

    static LineView _textDisplay;
    static Window _textDisplayWindow;
    static Button _showTextButton;
    static TextMeshProUGUI _header;

    void Awake()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        try
        {
            if (_textDisplay == null)
            {
                _textDisplayWindow = FindFirstObjectByType<Canvas>().transform.FindDeepChild("NoteUI").GetComponent<Window>();
                _textDisplay = _textDisplayWindow.GetComponentInChildren<LineView>(true);
                _showTextButton = _textDisplayWindow.transform.FindDeepChild("Btn_ShowText").GetComponent<Button>();
                _header = _textDisplayWindow.transform.FindDeepChild("Header").GetComponentInChildren<TextMeshProUGUI>(true);

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

        
        _showTextButton.onClick.RemoveListener(_interactionSettings.ToggleNoteShowText);
        _showTextButton.onClick.RemoveListener(SetActiveStateFromSettings);

        WindowManager.Instance.CloseCurrentWindow();
    }

    public IEnumerator Interact()
    {
        yield return null;
        if (!WindowManager.Instance.TrySwitchWindow(_textDisplayWindow)) yield break;

        _showTextButton.onClick.AddListener(_interactionSettings.ToggleNoteShowText);
        _showTextButton.onClick.AddListener(SetActiveStateFromSettings);

        _header.text = Title;

        _runner.DisplayText(_text);
        SetActiveStateFromSettings();
        AudioManager.instance.PlayOneShot(FMODEvents.instance.paperHandling, transform.position);
    }

    public void Activate()
    {
        if (!_interactionSettings.NoteShowText) return;

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

    void SetActiveStateFromSettings()
    {
        if(_interactionSettings.NoteShowText) Activate();
        else Deactivate();
    }

}
