using System;
using UnityEngine;

public class FallState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager playerStateManager)
    {
        base.EnterState(playerStateManager);
        _playerStateManager.GetAnimator().SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EPlayerState.fall);

        if (_playerStateManager.GetPrevStateIsWallSlide())
            _playerStateManager.FlipSpriteAfterWallSlide();
    }

    public override void ExitState() { }

    public override void Update()
    {
        if (CheckIfCanIdle())
        {
            SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.PlayerLandSfx, 1.0f);
            _playerStateManager.ChangeState(_playerStateManager.idleState);
        }
        else if (CheckIfCanRun())
        {
            SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.PlayerLandSfx, 1.0f);
            _playerStateManager.ChangeState(_playerStateManager.runState);
        }
        else if (CheckIfCanDbJump())
            _playerStateManager.ChangeState(_playerStateManager.doubleJumpState);
        else if (CheckIfCanWallSlide())
            _playerStateManager.ChangeState(_playerStateManager.wallSlideState);
        else if (CheckIfCanDash())
            _playerStateManager.ChangeState(_playerStateManager.dashState);
    }

    private bool CheckIfCanIdle()
    {
        return Math.Abs(_playerStateManager.GetRigidBody2D().velocity.x) < GameConstants.NEAR_ZERO_THRESHOLD
            && Math.Abs(_playerStateManager.GetRigidBody2D().velocity.y) < GameConstants.NEAR_ZERO_THRESHOLD
            && _playerStateManager.GetIsOnGround();
    }

    private bool CheckIfCanRun()
    {
        return Math.Abs(_playerStateManager.GetRigidBody2D().velocity.x) > GameConstants.NEAR_ZERO_THRESHOLD
            && Math.Abs(_playerStateManager.GetRigidBody2D().velocity.y) < GameConstants.NEAR_ZERO_THRESHOLD
            && _playerStateManager.GetIsOnGround();
    }

    private bool CheckIfCanDbJump()
    {
        return Input.GetButtonDown(GameConstants.JUMP_BUTTON) && _playerStateManager.GetCanDbJump();
    }

    private bool CheckIfCanWallSlide()
    {
        return _playerStateManager.GetIsWallTouch() && 
            _playerStateManager.GetDirX() * _playerStateManager.WallHit.normal.x < 0f;
    }

    private bool CheckIfCanDash()
    {
        return Input.GetButtonDown(GameConstants.DASH_BUTTON)
             && Time.time - _playerStateManager.dashState.DashDelayStart >= _playerStateManager.GetPlayerStats.DelayDashTime
             || Input.GetButtonDown(GameConstants.DASH_BUTTON) && _playerStateManager.dashState.IsFirstTimeDash;
    }

    public override void FixedUpdate()
    {
        if (_playerStateManager.GetDirX() != 0)
            if (!BuffsManager.Instance.GetTypeOfBuff(GameEnums.EBuffs.Speed).IsAllowToUpdate)
                _playerStateManager.GetRigidBody2D().velocity = new Vector2(_playerStateManager.GetPlayerStats.SpeedX * _playerStateManager.GetDirX(), _playerStateManager.GetRigidBody2D().velocity.y);
            else
                _playerStateManager.GetRigidBody2D().velocity = new Vector2(_playerStateManager.GetPlayerStats.SpeedX *  ((PlayerSpeedBuff)BuffsManager.Instance.GetTypeOfBuff(GameEnums.EBuffs.Speed)).SpeedMultiplier * _playerStateManager.GetDirX(), _playerStateManager.GetRigidBody2D().velocity.y);
    }
}
