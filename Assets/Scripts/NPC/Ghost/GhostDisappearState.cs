using UnityEngine;

public class GhostDisappearState : CharacterBaseState
{
    private GhostManager _ghostManager;
    private bool _allowToUpdate;

    public bool AllowToUpdate { set => _allowToUpdate = value; }

    public override void EnterState(CharactersManager charactersManager)
    {
        base.EnterState(charactersManager);
        _ghostManager = (GhostManager)charactersManager;
        _ghostManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EGhostState.disappear);
    }

    public override void ExitState() { _allowToUpdate = false; }

    public override void Update()
    {
        if (CheckIfCanAppear())
            _ghostManager.ChangeState(_ghostManager.GetGhostAppearState());
    }

    private bool CheckIfCanAppear()
    {
        return _ghostManager.GetIsPlayerNearBy() && _allowToUpdate;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}