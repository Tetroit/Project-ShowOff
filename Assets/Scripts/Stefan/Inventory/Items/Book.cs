using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEditor.U2D.ScriptablePacker;

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

        //_interactionManager.HoldManager.Interacted.AddListener(AddNote);
        _swipeRight.onClick.AddListener(() =>
        {
            if (_notesContainer.ItemCount <= 1) return;
            BookPage prev = _notesContainer.GetCurrentItem() as BookPage;
            int prevIndex = _notesContainer.CurentItemIndex;
            _notesContainer.ChangeItemPosition(SelectDirection.Right);
            var next = _notesContainer.GetCurrentItem();
            //couldn't turn page
            if (prev == next) return;

            if (prevIndex - 1 >= 0)
            {
                var overlapingItem = _notesContainer.GetItemAt(prevIndex - 1);

                _turningTween?.Complete();

                _turningTween = TurnLeft(prev, _leftAngle);
                _turningTween.onComplete = () =>
                {
                    overlapingItem.gameObject.SetActive(false);
                    _turningTween = null;
                };

                next.gameObject.SetActive(true);
            }
            else
            {
                next.gameObject.SetActive(true);
                TurnLeft(prev, _leftAngle);
            }
        });
        _swipeLeft.onClick.AddListener(() =>
        {
            if (_notesContainer.ItemCount <= 1) return;

            var prev = _notesContainer.GetCurrentItem();
            _notesContainer.ChangeItemPosition(SelectDirection.Left);
            var next = _notesContainer.GetCurrentItem() as BookPage;
            int nextIndex = _notesContainer.CurentItemIndex;

            //couldn't turn page
            if (prev == next) return;

            _turningTween?.Complete();

            _turningTween = TurnRight(next, _rightAngle);

            _turningTween.onComplete = () =>
            {
                prev.gameObject.SetActive(false);
                _turningTween = null;
            };
            if(nextIndex - 1 >= 0)  
                _notesContainer.GetItemAt(nextIndex - 1).gameObject.SetActive(true);

        });

        _notesContainer.ForEach(item => item.transform.localRotation = _rightAngle);
    }

    public Tween TurnLeft(BookPage page, Quaternion leftAngle)
    {
        return page.transform.DOLocalRotateQuaternion(leftAngle, .3f);
    }

    public Tween TurnRight(BookPage page, Quaternion rightAngle)
    {
        return page.transform.DOLocalRotateQuaternion(rightAngle, .3f);

    }

    public void AddNote(GameObject noteObj, IHoldable component)
    {
        if(_notesContainer == null) _notesContainer = GetComponent<InventoryView>();
        if (_notesContainer.Any(x => x.name == noteObj.name)) return;
        Note note = component as Note; 

        _notesContainer.AddItem(note.PagePrefab);
        var page = _notesContainer.GetAddedItem() as BookPage;
        page.transform.parent = _pagesContainer;
        page.transform.SetLocalPositionAndRotation(
            Vector3.zero,
            _rightAngle
        );
        page.name = noteObj.name;
        page.transform.localScale = Vector3.one;
        page.Text = note.Text;
        page.Title = note.Title;
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

        //_notesContainer.ChangeItemPosition(0);
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
