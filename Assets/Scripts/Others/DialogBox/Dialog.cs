using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{

    [SerializeField] private GameObject _window; 
    [SerializeField] private GameObject _indicator;
    [SerializeField] private TMP_Text _dialogText;
    [SerializeField] private TMP_Text _indicatorText; 

    [SerializeField] private List<string> _dialog; 
    [SerializeField] private List<string> _dialog2; 

    [SerializeField] private List<string> _indicatorString;
    [SerializeField] private float _writingSpeed; 
    [SerializeField] private bool _dontNeedStartIndicatorTextAgain; 

    private int _rowIndex; 
    private int _charIndex; 
    private bool _started; 
    private bool _isWaiting; 
    private bool _cantGetNextIndicatorText;
    private bool _isFinishedFirstConversation;

    private bool _startConversationPassive;

    public bool Started { get { return _started; } }

    public bool IsWaiting { get { return _isWaiting; } }

    public bool CantGetNextIndicatorText { set { _cantGetNextIndicatorText = value; } }

    public bool StartConversationPassive { set { _startConversationPassive = value; } }

    public bool IsFinishedFirstConversation { get => _isFinishedFirstConversation; }

    private void Awake()
    {
        ToggleWindow(false);
        ToggleIndicator(false);
    }

    void Start()
    {
        if (_indicatorText)
            _indicatorText.text = _indicatorString[0];
    }

    void Update()
    {
        if (_indicatorText)
            _indicatorText.transform.eulerAngles = Vector3.zero;

        if (!_started) 
            return;

        _dialogText.transform.eulerAngles = Vector3.zero;

        if (_isWaiting && Input.GetKeyDown(KeyCode.Space) && !_startConversationPassive
            || _isWaiting && Input.GetKeyDown(KeyCode.T) && _startConversationPassive)
        {
            if (_startConversationPassive)
                _startConversationPassive = false;

            _isWaiting = false; 

            if(!_isFinishedFirstConversation)
            {
                _rowIndex++; 

                if (_rowIndex < _dialog.Count)
                    GetDialog(_rowIndex);
                else
                {
                    UpdateIndicatorText();
                    EndDialog();
                }
            }
            else
            {
                _rowIndex++;
                if (_rowIndex < _dialog2.Count)
                    GetDialog(_rowIndex);
                else
                {
                    UpdateIndicatorText();
                    EndDialog();
                }
            }
        }
    }

    private void UpdateIndicatorText()
    {
        if (!_dontNeedStartIndicatorTextAgain)
            _indicatorText.text = _indicatorString[0];
        else
            _indicatorText.text = _indicatorString[1];
    }

    public void ToggleWindow(bool show)
    {
        _window.SetActive(show);
    }

    public void ToggleIndicator(bool show)
    {
        if (_indicator)
            _indicator.SetActive(show);
    }

    public void StartDialog(int index)
    {
        if(_started) 
            return;

        _started = true;
        ToggleWindow(true);
        ToggleIndicator(true);
        GetDialog(index);
    }

    public void EndDialog()
    {
        _started = false;
        StopAllCoroutines();
        ToggleWindow(false);

        if (!_isFinishedFirstConversation)
            _isFinishedFirstConversation = true;
        Debug.Log("End: " + _isFinishedFirstConversation);
    }

    private void GetDialog(int i)
    {
        _rowIndex = i; 
        _charIndex = 0; 
        _dialogText.text = string.Empty; 
        StartCoroutine(Writing());
    }

    IEnumerator Writing()
    {
        yield return new WaitForSeconds(_writingSpeed);

        string currentDialog;
        if (!_isFinishedFirstConversation)
            currentDialog = _dialog[_rowIndex];
        else 
            currentDialog = _dialog2[_rowIndex];

        _dialogText.text += currentDialog[_charIndex];

        ++_charIndex;

        if (_charIndex < currentDialog.Length)
            StartCoroutine(Writing());
        else
        {
            if (_indicatorText)
            {
                _indicatorText.text = string.Empty;
                if (_rowIndex >= 0 && _indicatorString.Count > 1 && !_cantGetNextIndicatorText)
                    _indicatorText.text = _indicatorString[1];
                else
                    _indicatorText.text = _indicatorString[0];
            }
            ToggleIndicator(true);
            _isWaiting = true;
        }
    }
}
