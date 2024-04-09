using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedBuff : PlayerBuffs
{
    [SerializeField] private float _speedMultiplier;
    [SerializeField] private Transform _speedBuffIcon; 
    [SerializeField] private Transform _speedBuffIconPos;

    public float SpeedMultiplier { get { return _speedMultiplier; } }

    public override void Start()
    {
        _speedBuffIcon.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if (_isAllowToUpdate)
        {
            if (Time.time - _entryTime >= _buffDuration)
            {
                _isAllowToUpdate = false;
                _speedBuffIcon.gameObject.SetActive(false);
                SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.NormalBuffDownSfx, 1.0f);
            }
            _speedBuffIcon.transform.position = _speedBuffIconPos.position;
        }
    }

    public override void ApplyBuff()
    {
        base.ApplyBuff();
        _speedBuffIcon.gameObject.SetActive(true);
        _speedBuffIcon.transform.position = _speedBuffIconPos.position;
        SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.NormalBuffUpSfx, 1.0f);
    }
}