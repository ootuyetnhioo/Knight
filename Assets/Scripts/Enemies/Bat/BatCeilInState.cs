using UnityEngine;

public class BatCeilInState : MEnemiesBaseState
{
    private BatManager _batManager;

    public override void EnterState(CharactersManager charactersManager)
    {
        base.EnterState(charactersManager);
        _mEnemiesManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EBatState.ceilIn);
        _batManager = (BatManager)charactersManager;
        _batManager.GetRigidbody2D().velocity = Vector2.zero;
    }

    public override void ExitState() { }

    public override void Update() { }

    public override void FixedUpdate() { }
}
