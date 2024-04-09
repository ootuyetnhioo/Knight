using UnityEngine;

public class MEnemiesPatrolState : MEnemiesBaseState
{
    protected float _entryTime;
    protected bool _hasChangeDirection = false;
    protected bool _hasChangedState;
    protected bool _canRdDirection = true;
    protected bool _hasJustHitWall = false;
    protected int _rdLeftRight; 

    public bool CanRdDirection { set => _canRdDirection = value; }

    public void SetHasJustHitWall(bool para) { _hasJustHitWall = para; }

    public override void EnterState(CharactersManager charactersManager)
    {
        base.EnterState(charactersManager);
        _mEnemiesManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EMEnemiesState.patrol);
        _entryTime = Time.time;
        if (_canRdDirection)
            HandleRandomChangeDirection();
    }

    public override void ExitState()
    {
        HandleCanRandomNextPatrolState();
        ResetDataForNextPatrolState();
    }

    public override void Update()
    {
        LogicUpdate();
    }

    protected virtual void LogicUpdate()
    {
        if (CheckIfCanRest())
        {
            _mEnemiesManager.CancelInvoke();
            _mEnemiesManager.ChangeState(_mEnemiesManager.MEnemiesIdleState);
        }
        else if (CheckIfCanAttack())
        {
            _hasChangedState = true;
            _mEnemiesManager.Invoke("AllowAttackPlayer", _mEnemiesManager.EnemiesSO.AttackDelay);
        }

        if (CheckIfCanChangeDirection())
        {
            _hasChangeDirection = true;
            _mEnemiesManager.FlippingSprite();
        }
    }

    protected bool CheckIfCanRest()
    {
        return Time.time - _entryTime >= _mEnemiesManager.MEnemiesSO.PatrolTime 
            && !_mEnemiesManager.HasDetectedPlayer;
    }

    protected virtual bool CheckIfCanAttack()
    {
        return _mEnemiesManager.HasDetectedPlayer && !_hasChangedState;
    }

    protected virtual bool CheckIfCanChangeDirection()
    {
        return _mEnemiesManager.HasCollidedWall || !_mEnemiesManager.HasDetectedGround;
    }

    public override void FixedUpdate()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (_mEnemiesManager.GetIsFacingRight())
            _mEnemiesManager.GetRigidbody2D().velocity = new Vector2(_mEnemiesManager.MEnemiesSO.PatrolSpeed.x, _mEnemiesManager.GetRigidbody2D().velocity.y);
        else
            _mEnemiesManager.GetRigidbody2D().velocity = new Vector2(-_mEnemiesManager.MEnemiesSO.PatrolSpeed.x, _mEnemiesManager.GetRigidbody2D().velocity.y);
    }

    protected virtual void HandleRandomChangeDirection()
    {
        _rdLeftRight = Random.Range(0, 2);
        if (_rdLeftRight == 1 && !_mEnemiesManager.GetIsFacingRight())
            _mEnemiesManager.FlippingSprite();
        else if (_rdLeftRight == 0 && _mEnemiesManager.GetIsFacingRight())
            _mEnemiesManager.FlippingSprite();
    }

    private void HandleCanRandomNextPatrolState()
    {
        if (_hasChangeDirection && !_hasJustHitWall)
            _canRdDirection = false;
        else
            _canRdDirection = true;
    }

    private void ResetDataForNextPatrolState()
    {
        _hasChangeDirection = false;
        _hasJustHitWall = false;
        _hasChangedState = false;
    }
}
