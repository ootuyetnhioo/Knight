﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeManager : NPCManagers
{
    [SerializeField] private int _startIndexIfGotHit;

    private SlimeTalkState _slimeTalkState = new();
    private SlimeGotHitState _slimeGotHitState = new();
    private bool _hasGotHit;
    private bool _hasStartConversationPassive;

    public int GetStartIndexIfGotHit() { return _startIndexIfGotHit; }

    public bool HasStartConversationPassive { get {  return _hasStartConversationPassive; } set { _hasStartConversationPassive = value; } }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _npcTalkState = _slimeTalkState;
        if (Mathf.Abs(transform.eulerAngles.y) == 180f)
            _isFacingRight = true;
        Debug.Log("IsFR: " + _isFacingRight);
    }

    protected override void Update()
    {
        base.Update();
        HandleFlippingSprite();
    }

    private void HandleFlippingSprite()
    {
        if (transform.position.x >= _playerReference.transform.position.x)
        {
            if (_isFacingRight)
                FlippingSprite();
        }
        else if (transform.position.x < _playerReference.transform.position.x)
        {
            if (!_isFacingRight)
                FlippingSprite();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == GameConstants.PLAYER_TAG && !_hasGotHit)
        {
            _hasGotHit = true;
            EventsManager.Instance.NotifyObservers(GameEnums.EEvents.PlayerOnJumpPassive, null);
            ChangeState(_slimeGotHitState);
            _hasStartConversationPassive = true;
        }
    }

    private void BackToIdle()
    {
        ChangeState(_npcIdleState);
        _hasGotHit = false;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
