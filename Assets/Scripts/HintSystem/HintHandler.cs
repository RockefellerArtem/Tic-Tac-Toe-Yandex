using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintHandler : MonoBehaviour
{
    [SerializeField] private List<Sprite> _hintSprites;

    [SerializeField] private Image _currentImage;

    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _closeButton;

    private int _currentIndex = 0;


    private void Start()
    {
        InitializationButton();
        
        _currentImage.sprite = _hintSprites[_currentIndex];
    }

    private void InitializationButton()
    {
        _nextButton.onClick.AddListener(NextImage);
        _backButton.onClick.AddListener(BackImage);
        _closeButton.onClick.AddListener(CloseWindow);
    }

    private void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    private void BackImage()
    {
        _currentIndex--;
        _currentImage.sprite = _hintSprites[_currentIndex];
    }

    private void NextImage()
    {
        _currentIndex++;
        _currentImage.sprite = _hintSprites[_currentIndex];
    }
}
