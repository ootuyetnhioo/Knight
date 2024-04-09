using UnityEngine;

public class SlimeTalkState : NPCTalkState
{
    private SlimeManager _slimeManager;

    public override void EnterState(CharactersManager charactersManager)
    {
        _slimeManager = (SlimeManager)charactersManager;
        _npcManager = _slimeManager;
        _slimeManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.ESlimState.idle);
        _slimeManager.GetRigidbody2D().velocity = Vector2.zero;

        if (_slimeManager.HasStartConversationPassive)
            HandleInteractWithPlayer(_slimeManager, _slimeManager.GetStartIndexIfGotHit());
        else
            HandleInteractWithPlayer(_slimeManager, _slimeManager.GetStartIndex());
        Debug.Log("Slime Talk");
    }

    public override void ExitState() 
    { 
        base.ExitState();
        _slimeManager.HasStartConversationPassive = false;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate() { }
}