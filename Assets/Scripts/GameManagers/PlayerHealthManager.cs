using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct HP
{
    public int _state;
    public Sprite _HPSprite;
    public Dictionary<int, Sprite> _dictHP;
}

public class PlayerHealthManager : BaseSingleton<PlayerHealthManager>
{
    [Header("HP Icon")]
    [SerializeField] private Image[] _uiHP = new Image[GameConstants.PLAYER_MAX_HP];
    [SerializeField] private Sprite _normalHPSprite;
    [SerializeField] private Sprite _lostHPSprite;
    [SerializeField] private Sprite _tempHPSprite;

    [Header("SO")]
    [SerializeField] private PlayerStats _playerSO;

    private HP[] _HPs = new HP[GameConstants.PLAYER_MAX_HP];
    private int _maxHP;
    private int _currentHP;
    private int _tempHP;
    private bool _hasIncreaseHP;

    #region BLINK EFFECT FOR TEMP_HP WHEN RUNNING OUT
    private bool _hasGotTempHP;
    private float _tempHPEntryTime;
    private bool _hasTickRunOut;
    private float _tempHPEachRunOutEntryTime;
    private float _tempHPRunOutEntryTime;
    private bool _blinkLost = true;
    #endregion

    public HP[] HPs { get { return _HPs; } set { HPs = value; } }

    public int CurrentHP { get { return _currentHP; } }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InitHP();
        InitHPArray();
        InitHPDictionary();
        InitUIHP();
    }

    private void InitHP()
    {
        _maxHP = _playerSO.MaxHP;
        _currentHP = _maxHP;
        _tempHP = 0;
    }

    private void InitHPDictionary()
    {
        for (int i = 0; i < _maxHP; i++)
        {
            _HPs[i]._dictHP = new Dictionary<int, Sprite>()
            {
                {GameConstants.HP_STATE_NORMAL, _normalHPSprite },
                {GameConstants.HP_STATE_LOST, _lostHPSprite },
                {GameConstants.HP_STATE_TEMP, _tempHPSprite }
            };        
        }
    }

    private void InitUIHP()
    {
        for (int i = 0; i < GameConstants.PLAYER_MAX_HP; i++)
        {
            if (i < _maxHP)
                _uiHP[i].enabled = true;
            else
                _uiHP[i].enabled = false;
        }
    }

    private void InitHPArray()
    {
        for (int i = 0; i < _maxHP; i++)
        {
            _HPs[i]._state = GameConstants.HP_STATE_NORMAL;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            UpdateHPToUI();
        }
    }

    public void ChangeHPState(int state)
    {
        HandleIfIncreaseHP(state);
        HandleIfDecreaseHP(state);
        HandleIfIncreaseTempHP(state);
    }

    private void HandleIfIncreaseHP(int state)
    {
        if (state == GameConstants.HP_STATE_NORMAL)
        {
            if (_currentHP < _maxHP)
            {
                _HPs[_currentHP]._state = state;
                _currentHP++;
            }
        }
    }

    private void HandleIfDecreaseHP(int state)
    {
        if (state == GameConstants.HP_STATE_LOST)
        {
            if (_tempHP != 0)
            {
                if (_tempHP + _currentHP > _maxHP)
                {
                    _uiHP[_tempHP + _currentHP - 1].enabled = false;
                    _tempHP--;
                }
                else
                {
                    _HPs[_tempHP + _currentHP - 1]._state = state;
                    _tempHP--;
                }
            }
            else
            {
                if (_currentHP > 0)
                {
                    _currentHP--;
                }
                
                _HPs[_currentHP]._state = state;
            }
        }
    }

    private void HandleIfIncreaseTempHP(int state)
    {
        if (state == GameConstants.HP_STATE_TEMP)
        {
            if (_tempHP == 0)
            {
                _hasGotTempHP = true;
                _tempHPEntryTime = Time.time;
            }

            _HPs[_currentHP + _tempHP]._state = state;
            _tempHP++;

            if (_tempHP + _currentHP >= _maxHP)
                _uiHP[_tempHP + _currentHP - 1].enabled = true;
        }
    }

    private void UpdateHPToUI()
    {
        for (int i = 0; i < _maxHP; i++)
        {
            _uiHP[i].sprite = _HPs[i]._dictHP[_HPs[i]._state];
        }

        for (int i = 0; i < _tempHP; i++)
        {
            if (_HPs[i + _currentHP]._dictHP == null)
            {
                _HPs[i + _currentHP]._dictHP = new Dictionary<int, Sprite>()
                {
                        { GameConstants.HP_STATE_NORMAL, _normalHPSprite },
                        { GameConstants.HP_STATE_LOST, _lostHPSprite },
                        { GameConstants.HP_STATE_TEMP, _tempHPSprite }
                };
            }
            _uiHP[i + _currentHP].sprite = _HPs[i + _currentHP]._dictHP[_HPs[i + _currentHP]._state];
        }
    }

    private void StartTickRunOut()
    {
        if (!_hasTickRunOut)
        {
            _hasTickRunOut = true;
            _tempHPRunOutEntryTime = Time.time;
            _tempHPEachRunOutEntryTime = Time.time;
        }
    }

    private void HandleExpireTempHP()
    {
        for (int i = _currentHP; i < _currentHP + _tempHP; i++)
        {
            if (i > _maxHP - 1)
                _uiHP[i].enabled = false;
            else
                _HPs[i]._state = GameConstants.HP_STATE_LOST;
        }
    }

    private void ResetDataRelatedToTempHP()
    {
        _tempHP = 0;
        _hasGotTempHP = false;
        _hasTickRunOut = false;
    }

    public void RestartHP()
    {
        Start();
    }

    public void IncreaseHP()
    {
        if (_hasIncreaseHP) return;

        _hasIncreaseHP = true;
        _playerSO.MaxHP = GameConstants.PLAYER_MAX_HP_LEVEL_2;
        Start();
        Debug.Log("Incr");
    }

    public void DecreaseHP()
    {
        _hasIncreaseHP = false;
        _playerSO.MaxHP = GameConstants.PLAYER_MAX_HP_LEVEL_1;
    }
}