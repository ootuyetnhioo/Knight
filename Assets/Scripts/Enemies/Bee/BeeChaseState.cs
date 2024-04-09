using UnityEngine;

public class BeeChaseState : MEnemiesAttackState
{
    private BeeManager _beeManager;
    private Vector2 _attackPos;

    public override void EnterState(CharactersManager charactersManager)
    {
        base.EnterState(charactersManager);
        _mEnemiesManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EMEnemiesState.patrol);
        _beeManager = (BeeManager)charactersManager;
        _attackPos = new Vector2(_beeManager.GetPlayer().position.x, _beeManager.GetPlayer().position.y + _beeManager.AdJustYAxisAttackRange());
        HandleLeftRightLogic();
        if (!_beeManager.MustAttack)
        {
            _beeManager.MustAttack = true;
        }
        _beeManager.GetRigidbody2D().velocity = Vector2.zero;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Update()
    {
        HandleLeftRightLogic();

        if (CheckIfCanAttack())
            _beeManager.ChangeState(_beeManager.GetBeeAttackState());
        else if (CheckIfCanIdle())
            _beeManager.ChangeState(_beeManager.GetBeeIdleState());
        else if (CheckIfOutOfMinMaxRange())
        {
            _beeManager.GetBeePatrolState().HasChangedDirection = true;
            _beeManager.GetBeePatrolState().CanRdDirection = false;
            _beeManager.FlippingSprite();
            _beeManager.ChangeState(_beeManager.GetBeePatrolState());
        }
        else
            Attack();
    }

    private bool CheckIfCanIdle()
    {
        return !_beeManager.HasDetectedPlayer;
    }

    private bool CheckIfCanAttack()
    {
        return Vector2.Distance(_beeManager.transform.position, _attackPos) < 0.1f;
    }

    private bool CheckIfOutOfMinMaxRange()
    {
        return _beeManager.transform.position.x >= _beeManager.BoundaryRight.position.x
             || _beeManager.transform.position.x <= _beeManager.BoundaryLeft.position.x;
    }

    public override void FixedUpdate(){}

    protected override void Attack()
    {
        _attackPos = new Vector2(_beeManager.GetPlayer().position.x, _beeManager.GetPlayer().position.y + _beeManager.AdJustYAxisAttackRange());
        _beeManager.transform.position = Vector2.MoveTowards(_beeManager.transform.position, _attackPos, _beeManager.MEnemiesSO.ChaseSpeed.x * Time.deltaTime);
    }

    private void HandleLeftRightLogic()
    {
        if (_beeManager.transform.position.x > _attackPos.x)
            _beeManager.SetIsFacingRight(false);
        else if (_beeManager.transform.position.x < _attackPos.x)
            _beeManager.SetIsFacingRight(true);
    }
}
