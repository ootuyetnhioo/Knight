using UnityEngine;

public class MEnemiesAttackState : MEnemiesBaseState
{
    protected bool _allowUpdate;

    public void SetAllowUpdate(bool para) { this._allowUpdate = para; }

    public override void EnterState(CharactersManager charactersManager)
    {
        base.EnterState(charactersManager);
        _mEnemiesManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EMEnemiesState.attack);
    }

    public override void ExitState() { }

    public override void Update()
    {
        LogicUpdate();
    }

    private void LogicUpdate()
    {
        if (!_mEnemiesManager.HasDetectedPlayer)
        {
            _mEnemiesManager.ChangeState(_mEnemiesManager.MEnemiesIdleState);
        }
    }

    public override void FixedUpdate()
    {
        Attack();
    }

    protected virtual void Attack()
    {
        if (_mEnemiesManager.GetIsFacingRight())
            _mEnemiesManager.GetRigidbody2D().velocity = new Vector2(_mEnemiesManager.MEnemiesSO.ChaseSpeed.x, _mEnemiesManager.GetRigidbody2D().velocity.y);
        else
            _mEnemiesManager.GetRigidbody2D().velocity = new Vector2(-_mEnemiesManager.MEnemiesSO.ChaseSpeed.x, _mEnemiesManager.GetRigidbody2D().velocity.y);
    }
}
