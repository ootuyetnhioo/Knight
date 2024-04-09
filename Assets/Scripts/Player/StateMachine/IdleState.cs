using UnityEngine;

public class IdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager playerStateManager)
    {
        base.EnterState(playerStateManager);
        _playerStateManager.GetAnimator().SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EPlayerState.idle);
        _playerStateManager.GetRigidBody2D().velocity = Vector2.zero;
        HandleIfInteractWithNPC();
        HandleIfPrevStateWallSlide();
    }

    public override void ExitState() { }

    public override void Update()
    {
        if(!_playerStateManager.IsInteractingWithNPC)
        {
            if (CheckIfCanRun())
                _playerStateManager.ChangeState(_playerStateManager.runState);
            else if (CheckIfCanJump())
                _playerStateManager.ChangeState(_playerStateManager.jumpState);
            else if (CheckIfCanFall())
                _playerStateManager.ChangeState(_playerStateManager.fallState);
            else if (CheckIfCanDash())
                _playerStateManager.ChangeState(_playerStateManager.dashState);        }
    }

    private bool CheckIfCanRun()
    {
        return _playerStateManager.GetDirX() != 0 && _playerStateManager.GetIsOnGround();
    }

    private bool CheckIfCanJump()
    {
        return Input.GetButtonDown(GameConstants.JUMP_BUTTON) && _playerStateManager.CanJump;
    }

    private bool CheckIfCanFall()
    {
        return _playerStateManager.GetDirY() == 0 && !_playerStateManager.GetIsOnGround() && !_playerStateManager.CanJump;
    }

    private bool CheckIfCanDash()
    {
        return Input.GetButtonDown(GameConstants.DASH_BUTTON)
             && Time.time - _playerStateManager.dashState.DashDelayStart >= _playerStateManager.GetPlayerStats.DelayDashTime
             || Input.GetButtonDown(GameConstants.DASH_BUTTON) && _playerStateManager.dashState.IsFirstTimeDash;
    }

    private void HandleIfInteractWithNPC()
    {
        if (_playerStateManager.IsInteractingWithNPC)
            if (!_playerStateManager.HasDetectedNPC)
            {
                _playerStateManager.FlippingSprite();
                Debug.Log("!Detected, Must Flip");
            }
    }

    private void HandleIfPrevStateWallSlide()
    { 
        if (_playerStateManager.GetPrevStateIsWallSlide())
            _playerStateManager.FlipSpriteAfterWallSlide();
    }

    public override void FixedUpdate(){}
}