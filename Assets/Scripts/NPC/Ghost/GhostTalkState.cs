using UnityEngine;

public class GhostTalkState : NPCTalkState
{
    private GhostManager _ghostManager;

    public override void EnterState(CharactersManager charactersManager)
    {
        _npcManager = (NPCManagers)charactersManager;
        _ghostManager = (GhostManager)charactersManager;
        _ghostManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EGhostState.appear);
        HandleInteractWithPlayer(_ghostManager, _ghostManager.GetStartIndex());
    }

    public override void ExitState() { }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate() { }
}
