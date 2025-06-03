using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InventoryView))]
public class Book : InventoryItemView
{
    [SerializeField] GameStateManager _gameStateManager;
    [SerializeField] GameObject[] UI_Icons;
    [SerializeField] InteractionManager _interactionManager;
    [SerializeField] BookPage _notePrefab;
    [SerializeField] BookPage _emptyPagePrefab;
    [SerializeField] Button _swipeRight;
    [SerializeField] Button _swipeLeft;

    InventoryView _notesContainer;

    void Awake()
    {
        _notesContainer = GetComponent<InventoryView>();
    }

    void Start()
    {
        _interactionManager.HoldManager.Interacted.AddListener(AddNote);
        _swipeRight.onClick.AddListener(()=> _notesContainer.ChangeItemPosition(SelectDirection.Right));
        _swipeLeft.onClick.AddListener(()=> _notesContainer.ChangeItemPosition(SelectDirection.Left));
    }

    void AddNote(GameObject noteObj, IHoldable component)
    {
        if (_notesContainer.Any(x => x.name == noteObj.name)) return;

        _notesContainer.AddItem(_notePrefab);
        var page = _notesContainer.GetAddedItem();

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
