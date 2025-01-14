﻿using UnityEngine;

public class BatIdleState : MEnemiesIdleState
{
    private BatManager _batManager;
    private bool _hasPatrol;

    public bool HasPatrol { set => _hasPatrol = value; }

    public override void EnterState(CharactersManager charactersManager)
    {
        base.EnterState(charactersManager);
        _batManager = (BatManager)charactersManager;
    }

    public override void ExitState() 
    {
        base.ExitState();
        _hasPatrol = false;
    }

    public override void Update()
    {
        if (CheckIfCanAttack())
            _batManager.ChangeState(_batManager.BatAttackState);
        else if (CheckIfCanPatrol())
            _batManager.ChangeState(_batManager.BatPatrolState);
        else if (CheckIfCanFlyBack())
            _batManager.ChangeState(_batManager.BatFlyBackState);
    }

    protected override bool CheckIfCanPatrol()
    {
        return base.CheckIfCanPatrol() && !_hasPatrol;
    }

    private bool CheckIfCanFlyBack()
    {
        return _hasPatrol && Time.time - _entryTime >= _batManager.MEnemiesSO.RestTime;
    }

    public override void FixedUpdate() { }
}
