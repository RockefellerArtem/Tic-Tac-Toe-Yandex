using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] private Image _icon;

    [Header("Sprites")]
    [SerializeField] private Sprite _cross;
    [SerializeField] private Sprite _zero;

    public TypeItem Type { get; private set; }

    private UIButton _button = default;

    public Action<Cell> CallbackEvent { get; set; }

    public void ClickCell()
    {
        CallbackEvent?.Invoke(this);
    }

    public void InteractableCell(bool isActive)
    {
        _button.enabled = isActive;
    }

    public void SetSprite(TypeItem typeItem)
    {
        _icon.gameObject.SetActive(true);

        switch (typeItem)
        {
            case TypeItem.Cross:
                _icon.sprite = _cross;
                break;

            case TypeItem.Zero:
                _icon.sprite = _zero;
                break;
        }

        Type = typeItem;
    }

    public void Init()
    {
        Type = TypeItem.Empty;
        _button = GetComponent<UIButton>();
        _button.Subscribe(ClickCell);
    }

    public void Subscribe(Action<Cell> callback)
    {
        CallbackEvent = callback;
    }
    public void UnSubscribe()
    {
        CallbackEvent = null;
    }
}