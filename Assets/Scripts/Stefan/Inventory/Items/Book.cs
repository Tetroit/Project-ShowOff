using DG.Tweening;
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
    [SerializeField] Quaternion _rightAngle;
    [SerializeField] Quaternion _leftAngle;

    InventoryView _notesContainer;

    Tween _turningTween;

    void Start()
    {
        if (_notesContainer == null) _notesContainer = GetComponent<InventoryView>();

        _swipeRight.onClick.AddListener(GoToRightPage);
        _swipeLeft.onClick.AddListener(GoToLeftPage);

        _notesContainer.ForEach(item => item.transform.localRotation = _rightAngle);
    }

    void GoToLeftPage()
    {
        if (_notesContainer.ItemCount <= 1) return;

        InventoryItemView deselectedPage = _notesContainer.GetCurrentItem();
        _notesContainer.ChangeItemPosition(SelectDirection.Left);
        BookPage selectedPage = _notesContainer.GetCurrentItem() as BookPage;
        int selectedPageIndex = _notesContainer.CurentItemIndex;

        //couldn't turn page
        if (deselectedPage == selectedPage) return;

        //turning on the page 
        _turningTween?.Complete();
        _turningTween = TurnRight(selectedPage);
        _turningTween.onComplete = () =>
        {
            deselectedPage.gameObject.SetActive(false);
            _turningTween = null;
        };
        if (selectedPageIndex - 1 >= 0)
            _notesContainer.GetItemAt(selectedPageIndex - 1).gameObject.SetActive(true);
    }

    void GoToRightPage()
    {
        if (_notesContainer.ItemCount <= 1) return;

        BookPage deselectedPage = _notesContainer.GetCurrentItem() as BookPage;
        int deselectedPageIndex = _notesContainer.CurentItemIndex;
        _notesContainer.ChangeItemPosition(SelectDirection.Right);
        InventoryItemView selectedPage = _notesContainer.GetCurrentItem();

        //couldn't turn page
        if (deselectedPage == selectedPage) return;

        if (deselectedPageIndex - 1 >= 0)
        {
            InventoryItemView overlapingItem = _notesContainer.GetItemAt(deselectedPageIndex - 1);

            _turningTween?.Complete();
            _turningTween = TurnLeft(deselectedPage);
            _turningTween.onComplete = () =>
            {
                overlapingItem.gameObject.SetActive(false);
                _turningTween = null;
            };

            selectedPage.gameObject.SetActive(true);
            return;
        }
        selectedPage.gameObject.SetActive(true);
        TurnLeft(deselectedPage);
    }

    public Tween TurnLeft(BookPage page)
    {
        return page.transform.DOLocalRotateQuaternion(_leftAngle, .3f);
    }

    public Tween TurnRight(BookPage page)
    {
        return page.transform.DOLocalRotateQuaternion(_rightAngle, .3f);

    }

    public void AddNote(GameObject noteObj, IHoldable component)
    {
        if(_notesContainer == null) _notesContainer = GetComponent<InventoryView>();
        if (_notesContainer.Any(x => x.name == noteObj.name)) return;
        Note note = component as Note; 

        _notesContainer.AddItem(note.PagePrefab);
        var page = _notesContainer.GetAddedItem() as BookPage;

        //initializing page
        page.transform.parent = _pagesContainer;
        page.transform.SetLocalPositionAndRotation(
            Vector3.zero,
            _rightAngle
        );
        page.name = noteObj.name;
        page.transform.localScale = Vector3.one;
        page.Text = note.Text;
        page.Title = note.Title;
        page.gameObject.SetActive(false);
    }

    public override void Select()
    {
        base.Select();
        if( _gameStateManager != null)
        _gameStateManager.SwitchState<S_UI>();

        foreach (var icon in UI_Icons) icon.SetActive(true);

        if( _interactionManager != null)
        _interactionManager.enabled = false;

        _swipeLeft.gameObject.SetActive(true);
        _swipeRight.gameObject.SetActive(true);
    }

    public override void Deselect()
    {
        base.Deselect();
        if( _gameStateManager != null)
        _gameStateManager.SwitchState<S_Play>();

        foreach (var icon in UI_Icons) icon.SetActive(false);

        if (_interactionManager != null)
            _interactionManager.enabled = true;

        _swipeLeft.gameObject.SetActive(false);
        _swipeRight.gameObject.SetActive(false);

    }


}
