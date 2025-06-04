using DG.Tweening;
using Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookPage : InventoryItemView, ITextDisplayer
{
    public string Text;
    public string Title;

    [SerializeField] TextRunner _runner;
    [SerializeField] InteractionSettings _interactionSettings;
    [SerializeField] bool _haveDisplay = true;

    static LineView _textDisplay;
    static Window _textDisplayWindow;
    static Button _showTextButton;
    static Button _interuptButton;
    static TextMeshProUGUI _header;

    private void Start()
    {
        Init();

    }
    void Init()
    {
        if (_runner == null) GetComponent<TextRunner>();
        if (_textDisplay == null)
        {
            _textDisplayWindow = FindFirstObjectByType<Canvas>().transform.FindDeepChild("NoteUI").GetComponent<Window>();
            _textDisplay = _textDisplayWindow.GetComponentInChildren<LineView>(true);
            _showTextButton = _textDisplayWindow.transform.FindDeepChild("Btn_ShowText").GetComponent<Button>();
            _interuptButton = _textDisplayWindow.transform.FindDeepChild("Btn_Interupt").GetComponent<Button>();
            _header = _textDisplayWindow.transform.FindDeepChild("Header").GetComponentInChildren<TextMeshProUGUI>(true);
        }
        _runner.LineView = _textDisplay;

    }

    public void Activate()
    {
        _interuptButton.gameObject.SetActive(true);

        _runner.EnableTextArea();
    }

    public void Deactivate()
    {
        _interuptButton.gameObject.SetActive(false);

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
        Init();

        _textDisplayWindow.gameObject.SetActive(true);
        _showTextButton.gameObject.SetActive(_haveDisplay);
        if (!_haveDisplay) return;

        _showTextButton.onClick.AddListener(_interactionSettings.ToggleNoteShowText);
        _showTextButton.onClick.AddListener(SetActiveStateFromSettings);
        _interuptButton.onClick.AddListener(_runner.InteruptDialogue);

        if (!_interactionSettings.NoteShowText) return;

        Activate();
        _header.text = Title;
        SetActiveStateFromSettings();
    }

    public override void Deselect()
    {
        Init();
        _showTextButton.gameObject.SetActive(!_haveDisplay);

        _showTextButton.onClick.RemoveListener(_interactionSettings.ToggleNoteShowText);
        _showTextButton.onClick.RemoveListener(SetActiveStateFromSettings);
        _interuptButton.onClick.RemoveListener(_runner.InteruptDialogue);
        _textDisplayWindow.gameObject.SetActive(false);

        Deactivate();
    }

    

    public void SetActiveStateFromSettings()
    {
        if (_interactionSettings.NoteShowText)
        {
            Activate();
            _runner.DisplayText(Text);
        }
        else Deactivate();
    }
}
