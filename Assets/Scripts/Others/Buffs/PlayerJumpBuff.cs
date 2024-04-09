using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpBuff : PlayerBuffs
{
    [SerializeField] private float _jumpMultiplier;
    [SerializeField] private Transform _jumpBuffIcon;
    [SerializeField] private Transform _jumpBuffIconPos;

    public float JumpMutiplier { get { return _jumpMultiplier; } }

    public override void Start()
    {
        _jumpBuffIcon.gameObject.SetActive(false);
    }

    public override void Update()
    {
        if (_isAllowToUpdate)
        {
            if (Time.time - _entryTime >= _buffDuration)
            {
                _isAllowToUpdate = false;
                _jumpBuffIcon.gameObject.SetActive(false);
                SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.NormalBuffDownSfx, 1.0f);
            }

            _jumpBuffIcon.transform.position = _jumpBuffIconPos.position;
        }
    }

    public override void ApplyBuff()
    {
        base.ApplyBuff();
        _jumpBuffIcon.gameObject.SetActive(true);
        _jumpBuffIcon.transform.position = _jumpBuffIconPos.position;
        SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.NormalBuffUpSfx, 1.0f);
    }
}