using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryCell : MonoBehaviour
{
    public static FactoryCell Instance;

    [SerializeField] private Transform _content;
    [SerializeField] private Cell _cell;

    private void Awake() => Instance = this;
    public Cell CreateCell()
    {
        var cell = Instantiate(_cell, _content);
        cell.Init();

        return cell;
    }

}
