using UnityEngine;

public class BatPatrolState : MEnemiesPatrolState
{
    private int _randomDirection;
    private BatManager _batManager;

    public override void EnterState(CharactersManager charactersManager)
    {
        _mEnemiesManager = (MEnemiesManager)charactersManager;
        _batManager = (BatManager)charactersManager;
        _entryTime = Time.time;
        _batManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EBatState.patrol);
        _randomDirection = Random.Range(0, 4);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Update()
    {
        if (CheckIfCanRest())
        {
            _batManager.BatIdleState.HasPatrol = true; 
            _batManager.ChangeState(_batManager.BatIdleState);
        }
        else if (CheckIfCanAttack())
            _batManager.ChangeState(_batManager.BatAttackState); 
        else if (CheckIfNeedRetreat())
            _batManager.ChangeState(_batManager.BatRetreatState);
        else if (!_hasChangedState)
            HandleFlyPatrol();
    }

    protected override bool CheckIfCanAttack()
    {
        return _batManager.HasDetectedPlayer;
    }

    private void HandleFlyPatrol()
    {
        switch (_randomDirection)
        {
            case 0:
                if (_batManager.GetIsFacingRight())
                    _batManager.FlipLeft();
                _batManager.GetRigidbody2D().velocity = new Vector2(-1f, 1f) * _batManager.MEnemiesSO.PatrolSpeed;
                break;
            case 1:
                if (!_batManager.GetIsFacingRight())
                    _batManager.FlipRight();
                _batManager.GetRigidbody2D().velocity = _batManager.MEnemiesSO.PatrolSpeed;
                break;
            case 2:
                if (_batManager.GetIsFacingRight())
                    _batManager.FlipLeft();
                _batManager.GetRigidbody2D().velocity = new Vector2(-1f, -1f) * _batManager.MEnemiesSO.PatrolSpeed;
                break;
            case 3:
                if (!_batManager.GetIsFacingRight())
                    _batManager.FlipRight();
                _batManager.GetRigidbody2D().velocity = new Vector2(1f, -1f) * _batManager.MEnemiesSO.PatrolSpeed;
                break;
        }
    }

    public override void FixedUpdate() { }

    private bool CheckIfNeedRetreat()
    {
        if (_batManager.transform.position.x >= _batManager.BoundaryRight.position.x)
        {
            _batManager.FlipLeft();
            return true;
        }
        else if (_batManager.transform.position.x <= _batManager.BoundaryLeft.position.x)
        {
            _batManager.FlipRight();
            return true;
        }
        return false;
    }
}
