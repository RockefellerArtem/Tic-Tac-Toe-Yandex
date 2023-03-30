using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMode : UIButton
{
    [SerializeField] private GameObject _outlineObject;

    public TypeMode TypeMode { get; private set; }

    private Action<ButtonMode> _callback;

    private void Start()
    {
        Subscribe(() => 
        { 
            _callback?.Invoke(this);
        });
    }

    public void SetOutline(bool isActive)
    {
        _outlineObject.SetActive(isActive);
    }

    public void SubcribeButtonMode(Action<ButtonMode> callback)
    {
        _callback += callback;
    }
}
