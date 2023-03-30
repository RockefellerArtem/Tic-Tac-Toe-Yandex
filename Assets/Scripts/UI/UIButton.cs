using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool _isPointerDown = false;
    private bool _isPointerEnter = false;

    private Action _callbackClick;
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        _isPointerDown = true;
    }

    public void Subscribe(Action callback)
    {
        _callbackClick += callback;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerEnter = true;
    }

    internal void UnSubscribe(Action callback)
    {
        _callbackClick -= callback;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerEnter = false;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        _isPointerDown = false;
        Click();
    }

    private void Click()
    {
        if (_isPointerEnter == true)
        {
            _callbackClick?.Invoke();
        }
    }
}
