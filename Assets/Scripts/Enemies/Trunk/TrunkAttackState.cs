using UnityEngine;

public class TrunkAttackState : MEnemiesAttackState
{
    private TrunkManager _trunkManager;

    public override void EnterState(CharactersManager charactersManager)
    {
        _charactersManager = charactersManager;
        _trunkManager = (TrunkManager)charactersManager;
        _trunkManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.ETrunkState.attack);
        _trunkManager.GetRigidbody2D().velocity = Vector2.zero;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Update()
    {
        if (!_trunkManager.HasDetectedPlayer)
        {
            _trunkManager.ChangeState(_trunkManager.GetTrunkIdleState());
        }
    }

    public override void FixedUpdate(){}

    protected override void Attack(){}
}
