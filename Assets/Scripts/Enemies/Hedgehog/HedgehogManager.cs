using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogManager : NMEnemiesManager
{
    [Header("Time")]
    [SerializeField] private float _spikeInDelay;

    [Header("Collider2D")]
    [SerializeField] private Vector2 _sizeIncrease;

    private HedgehogSpikeIdleState _hedgehogSpikeIdle = new();
    private BoxCollider2D _boxCollider2D;
    private Vector2 _prevCollider2DSize;

    public float SpikeInDelay { get { return _spikeInDelay; } }

    public Vector2 getSizeIncrease { get { return _sizeIncrease; } }

    public BoxCollider2D getBoxCollider2D { get { return _boxCollider2D; } }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void GetReferenceComponents()
    {
        base.GetReferenceComponents();
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void ChangeToSpikeIdle()
    {
        if (BuffsManager.Instance.GetTypeOfBuff(GameEnums.EBuffs.Invisible).IsAllowToUpdate)
        {
            ChangeState(_nmEnemiesIdleState);
            return;
        }

        _prevCollider2DSize = _boxCollider2D.size;
        ChangeState(_hedgehogSpikeIdle);
    }

    private void ChangeToSpikeIn()
    {
        _anim.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EHedgehogState.spikeIn);
    }

    private void ChangeToIdle()
    {
        ChangeState(_nmEnemiesIdleState);
        _boxCollider2D.size = _prevCollider2DSize;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (_state is NMEnemiesIdleState)
            base.OnTriggerEnter2D(collision);
    }
}
