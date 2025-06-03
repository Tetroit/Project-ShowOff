using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InventoryView))]
public class Book : InventoryItemView
{
    [SerializeField] GameStateManager _gameStateManager;
    [SerializeField] GameObject[] UI_Icons;
    [SerializeField] InteractionManager _interactionManager;
    [SerializeField] BookPage _emptyPagePrefab;
    [SerializeField] Button _swipeRight;
    [SerializeField] Button _swipeLeft;
    [SerializeField] Transform _pagesContainer;

    InventoryView _notesContainer;

    void Awake()
    {
        _notesContainer = GetComponent<InventoryView>();
    }

    void Start()
    {
        _interactionManager.HoldManager.Interacted.AddListener(AddNote);
        _swipeRight.onClick.AddListener(() =>
        {
            if (_notesContainer.ItemCount <= 1) return;
            var prev = _notesContainer.GetCurrentItem() as BookPage;
            _notesContainer.ChangeItemPosition(SelectDirection.Right);
            var next = _notesContainer.GetCurrentItem();
            //couldn't turn page
            if (prev == next) return;
            prev.TurnLeft();
        });
        _swipeLeft.onClick.AddListener(() =>
        {
            if (_notesContainer.ItemCount <= 1) return;

            var prev = _notesContainer.GetCurrentItem();
            _notesContainer.ChangeItemPosition(SelectDirection.Left);
            var next = _notesContainer.GetCurrentItem() as BookPage;
            //couldn't turn page
            if (prev == next) return;
            next.TurnRight();
        });
    }

    void AddNote(GameObject noteObj, IHoldable component)
    {
        if (_notesContainer.Any(x => x.name == noteObj.name)) return;
        Note note = component as Note; 

        _notesContainer.AddItem(note.PagePrefab);
        var page = _notesContainer.GetAddedItem();
        page.transform.parent = _pagesContainer;
        page.transform.SetLocalPositionAndRotation(
            Vector3.zero,
            Quaternion.identity
        );
        page.transform.localScale = Vector3.one;
    }

    public override void Select()
    {
        base.Select();
        if( _gameStateManager != null)
        _gameStateManager.SwitchState(GameState.UI);

        foreach (var icon in UI_Icons) icon.SetActive(true);

        if( _interactionManager != null)
        _interactionManager.enabled = false;

        _swipeLeft.gameObject.SetActive(true);
        _swipeRight.gameObject.SetActive(true);

        _notesContainer.ChangeItemPosition(0);
    }

    public override void Deselect()
    {
        base.Deselect();
        if( _gameStateManager != null)
        _gameStateManager.SwitchState(GameState.Play);

        foreach (var icon in UI_Icons) icon.SetActive(false);

        if (_interactionManager != null)
            _interactionManager.enabled = true;

        _swipeLeft.gameObject.SetActive(false);
        _swipeRight.gameObject.SetActive(false);

    }


}
