using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _game;
    [SerializeField] private GameObject _result;

    [SerializeField] private TMP_Text _resultDrawText;
    [SerializeField] private TMP_Text _resultWinText;

    [Header("ControllerResult")]
    [SerializeField] private TMP_Text _textResult;
    [SerializeField] private Image _iconResult;
    [SerializeField] private Image _iconWin;

    [Header("ControllerPanelCurrentPlayer")]
    [SerializeField] private Image _currentImage;
    [SerializeField] protected TMP_Text _currentText;

    [Header("ScorePanel")]
    [SerializeField] private TMP_Text _scoreZero;
    [SerializeField] private TMP_Text _scoreCross;

    [Header("Sprites")]
    [SerializeField] private Sprite _cross;
    [SerializeField] private Sprite _zero;

    [SerializeField] private Text _roundText;

    public static GameManager Instance;
    private const int _countCells = 9;

    private int _countRound = 1;

    private int _scoreX = 0;
    private int _scoreO = 0;

    [SerializeField] private ButtonMode[] _modeButtons;

    private TypeMode _currentMode;

    private TypeItem _currentType;

    private TypeItem _currentPlayer;

    private TypeItem[] _typeItems = new TypeItem[2]
    {
        TypeItem.Zero,
        TypeItem.Cross
    };

    private List<Cell> _cells = new List<Cell>();

    private int[,] _variants = new int[8, 3]
    {
        {0,1,2 },
        {3,4,5 },
        {6,7,8 },
        {0,3,6 },
        {1,4,7 },
        {2,5,8 },
        {0,4,8 },
        {2,4,6 },
    };

    private void Start()
    {
        foreach (var modeButton in _modeButtons)
        {
            modeButton.SubcribeButtonMode(Select);
        }

        _currentMode = TypeMode.Empty;
    }

    private void Update()
    {
        ControllerPanelCurrentPlayer();
    }

    private void ControllerPanelCurrentPlayer()
    {
        if (_currentMode == TypeMode.OneXFriend)
        {
            _currentImage.gameObject.SetActive(true);
            if (_currentType == TypeItem.Cross)
            {
                _currentImage.sprite = _cross;
            }
            else
            {
                _currentImage.sprite = _zero;
            }
        }
        if (_currentMode == TypeMode.OneXBot)
        {
            _currentImage.gameObject.SetActive(false);
            _currentText.gameObject.SetActive(true);
            
            if (_currentPlayer == TypeItem.Player)
            {
                _currentText.text = "Вы";
                _currentType = TypeItem.Cross;
                _currentImage.sprite = _cross;
            }
            if(_currentPlayer == TypeItem.Bot)
            {
                _currentText.text = "Бот";
                _currentType = TypeItem.Zero;
                _currentImage.sprite = _zero;
            }
        }
    }

    public void Next()
    {
        _result.SetActive(false);

        foreach (var cell in _cells)
        {
            cell.UnSubscribe();
            Destroy(cell.gameObject);
        }
        _cells = new List<Cell>();
        StartGame();
    }

    public void BotMode()
    {
        _menu.SetActive(false);
        _game.SetActive(true);

        for (int i = 0; i < _countCells; i++)
        {
            var cell = FactoryCell.Instance.CreateCell();
            cell.Subscribe(HandlerCell);
            _cells.Add(cell);
        }

        SetFirstPlayer();
    }

    public void FriendMode()
    {
        _currentText.gameObject.SetActive(false);
        _menu.SetActive(false);
        _game.SetActive(true);

        for (int i = 0; i < _countCells; i++)
        {
            var cell = FactoryCell.Instance.CreateCell();
            cell.Subscribe(HandlerCell);
            _cells.Add(cell);
        }
        SetFirstPlayer();
    }

    public void Menu()
    {
        _game.SetActive(false);
        _result.SetActive(false);
        _menu.SetActive(true);

        foreach (var cell in _cells)
        {
            Destroy(cell.gameObject);
        }
        _cells = new List<Cell>();

        _scoreCross.text = $"{_scoreX = 0}";
        _scoreZero.text = $"{_scoreO = 0}";
        _roundText.text = $"Раунд {_countRound = 1}";

        _currentMode = TypeMode.Empty;
        foreach (var modeButton in _modeButtons)
        {
            modeButton.SetOutline(false);
        }
    }

    public void Select(ButtonMode sender)
    {
        foreach (var modeButton in _modeButtons)
        {
            modeButton.SetOutline(modeButton == sender);
        }

        if (sender == _modeButtons[0])
        {
            _currentMode = TypeMode.OneXFriend;
        }
        if(sender == _modeButtons[1])
        {
            _currentMode = TypeMode.OneXBot;
        }
    }

    private void SetFirstPlayer()
    {
        if (_currentMode == TypeMode.OneXBot)
        {
            if (_currentPlayer == TypeItem.Player)
            {
                _currentPlayer = TypeItem.Bot;
                _currentType = TypeItem.Zero;
            }
            else
            {
                _currentPlayer = TypeItem.Player;
                _currentType = TypeItem.Cross;
            }
            BotStep();
        }

        var currentPlayer = _currentType;
    }

    private void HandlerCell(Cell cell)
    {
        if (cell.Type != TypeItem.Empty)
        {
            return;
        }
        cell.SetSprite(_currentType);

        CheckVariantWinToCells();

        NextPlayer();
    }

    private IEnumerator DelayStepBot()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));

        var cells = new List<Cell>();
        foreach (var cellTemp in _cells)
        {
            if (cellTemp.Type == TypeItem.Empty)
            {
                cells.Add(cellTemp);
            }
        }
        if(_currentPlayer == TypeItem.Bot) cells[Random.Range(0, cells.Count - 1)].ClickCell();
    }

    private void NextPlayer()
    {
        foreach (var typeItem in _typeItems)
        {
            if(typeItem != _currentType)
            {
                _currentType = typeItem;
                break;
            }
        }

        if (_currentMode == TypeMode.OneXBot)
        {
            if (_currentPlayer == TypeItem.Player)
            {
                _currentPlayer = TypeItem.Bot;
            }
            else
            {
                _currentPlayer = TypeItem.Player;
            }
            BotStep();

        }
    }

    private Coroutine _botStepCoroutine = null;
    private void BotStep()
    {
        if (_currentPlayer == TypeItem.Bot)
        {
            SetActiveCell(false);
            _botStepCoroutine = StartCoroutine(DelayStepBot());
        }
        else
        {
            SetActiveCell(true);
        }
    }

    private void SetActiveCell(bool isActive)
    {
        foreach (var cell in _cells)
        {
            cell.InteractableCell(isActive);
        }
    }

    private void CheckVariantWinToCells()
    {
        for (int i = 0; i < 8; i++)
        {
            bool isCheck = false;
            for (int j = 0; j < 3; j++)
            {
                Cell cell = _cells[_variants[i, j]];
                if (cell.Type != _currentType)
                {
                    isCheck = true;
                    break;
                }
            }
            if (isCheck) continue;

            switch (_currentType)
            {
                case TypeItem.Cross:
                    if (_botStepCoroutine != null)
                    {
                        StopCoroutine(_botStepCoroutine);
                    }
                    _result.SetActive(true);
                    _textResult.text = "Победил";
                    _iconResult.sprite = _cross;
                    _scoreX++;
                    _scoreCross.text = $"{_scoreX}";
                    _iconResult.gameObject.SetActive(true);
                    _iconWin.gameObject.SetActive(true);
                    _resultWinText.gameObject.SetActive(true);
                    _resultDrawText.gameObject.SetActive(false);
                    break;

                case TypeItem.Zero:
                    if (_botStepCoroutine != null)
                    {
                        StopCoroutine(_botStepCoroutine);
                    }
                    _result.SetActive(true);
                    _textResult.text = "Победил";
                    _iconResult.sprite = _zero;
                    _scoreO++;
                    _scoreZero.text = $"{_scoreO}";
                    _iconResult.gameObject.SetActive(true);
                    _iconWin.gameObject.SetActive(true);
                    _resultWinText.gameObject.SetActive(true);
                    _resultDrawText.gameObject.SetActive(false);
                    break;
            }
            ReStart();
            return;
        }

        if (_cells.All((t)=> t.Type != TypeItem.Empty))
        {
            if (_botStepCoroutine != null)
            {
                StopCoroutine(_botStepCoroutine);
            }
            _result.SetActive(true);
            _iconWin.gameObject.SetActive(false);
            _resultDrawText.gameObject.SetActive(true);
            _iconResult.gameObject.SetActive(false);
            _resultWinText.gameObject.SetActive(false);
            ReStart();
        }

    }

    private void ReStart()
    {
        _roundText.text = $"Раунд {++_countRound}";
        for (int i = 0; i < _cells.Count; i++)
        {
            _cells[i].SetSprite(TypeItem.Empty);
        }

        SetFirstPlayer();
    }

    public void StartGame()
    {
        if (_currentMode == TypeMode.Empty)
        {
            Debug.Log("Выберите мод!");
        }

        if (_currentMode == TypeMode.OneXBot)
        {
            BotMode(); 
        }

        if(_currentMode == TypeMode.OneXFriend)
        {
            FriendMode();
        }

        
    }
}
