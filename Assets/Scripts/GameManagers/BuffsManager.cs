﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsManager : BaseSingleton<BuffsManager>
{
    [SerializeField] PlayerSpeedBuff _speedBuff;
    [SerializeField] PlayerJumpBuff _jumpBuff;
    [SerializeField] PlayerInvisibleBuff _invisibleBuff;
    [SerializeField] PlayerShieldBuff _shieldBuff;
    [SerializeField] PlayerAbsorbBuff _absorbBuff;

    private Dictionary<GameEnums.EBuffs, PlayerBuffs> _dictBuffs = new();

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InitBuffDictionary();
        foreach (var buff in _dictBuffs.Values)
            buff.Start();
    }

    private void Update()
    {
        foreach(var buff  in _dictBuffs.Values)
            buff.Update();
    }

    private void InitBuffDictionary()
    {
        _dictBuffs.Add(GameEnums.EBuffs.Speed, _speedBuff);
        _dictBuffs.Add(GameEnums.EBuffs.Jump, _jumpBuff);
        _dictBuffs.Add(GameEnums.EBuffs.Invisible, _invisibleBuff);
        _dictBuffs.Add(GameEnums.EBuffs.Shield, _shieldBuff);
        _dictBuffs.Add(GameEnums.EBuffs.Absorb, _absorbBuff);
    }

    public PlayerBuffs GetTypeOfBuff(GameEnums.EBuffs buffType)
    {
        return _dictBuffs[buffType];
    }
}
