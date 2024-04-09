using UnityEngine;

public class RhinoAttackState : MEnemiesAttackState
{
    protected RhinoManager _rhinoManager;
    private bool _hasChangedState;

    public override void EnterState(CharactersManager charactersManager)
    {
        base.EnterState(charactersManager);
        _rhinoManager = (RhinoManager)charactersManager;
        SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.RhinoAttackSfx, 1.0f);
    }

    public override void ExitState()
    {
        _hasChangedState = false;
    }

    public override void Update()
    {
        LogicUpdate();
    }

    private void LogicUpdate()
    {
        if (_rhinoManager.HasCollidedWall)
        {
            _hasChangedState = true;
            _rhinoManager.CancelInvoke();
            _rhinoManager.ChangeState(_rhinoManager.RhinoWallHitState);
        }
        else if (!_rhinoManager.HasDetectedPlayer && !_hasChangedState)
        {
            _hasChangedState = true;
            _rhinoManager.Invoke("ChangeToIdle", _rhinoManager.RestDelay);
        }
    }

    public override void FixedUpdate()
    {
        Attack();
    }

    protected override void Attack()
    {
        if (_rhinoManager.GetIsFacingRight())
            _rhinoManager.GetRigidbody2D().velocity = new Vector2(_rhinoManager.MEnemiesSO.ChaseSpeed.x, _rhinoManager.GetRigidbody2D().velocity.y);
        else
            _rhinoManager.GetRigidbody2D().velocity = new Vector2(-_rhinoManager.MEnemiesSO.ChaseSpeed.x, _rhinoManager.GetRigidbody2D().velocity.y);
    }
}
