using UnityEngine;

public class WallSlideState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager playerStateManager)
    {
        base.EnterState(playerStateManager);
        _playerStateManager.GetAnimator().SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EPlayerState.wallSlide);
    }

    public override void ExitState() { }

    public override void Update()
    {
        if (CheckIfCanIdle())
        {
            SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.PlayerLandSfx, 1.0f);
            _playerStateManager.ChangeState(_playerStateManager.idleState);
        }
        else if (CheckIfCanWallJump())
            _playerStateManager.ChangeState(_playerStateManager.wallJumpState);
        else if (CheckIfCanFall())
            _playerStateManager.ChangeState(_playerStateManager.fallState);
        else if (CheckIfCanDash())
            _playerStateManager.ChangeState(_playerStateManager.dashState);
    }

    private bool CheckIfCanIdle()
    {
        return Mathf.Abs(_playerStateManager.GetRigidBody2D().velocity.x) < GameConstants.NEAR_ZERO_THRESHOLD
            && Mathf.Abs(_playerStateManager.GetRigidBody2D().velocity.y) < GameConstants.NEAR_ZERO_THRESHOLD
            && _playerStateManager.GetIsOnGround();
    }

    private bool CheckIfCanWallJump()
    {
        return Input.GetButtonDown(GameConstants.JUMP_BUTTON) && !_playerStateManager.GetIsOnGround();
    }

    private bool CheckIfCanFall()
    {
        return _playerStateManager.WallHit.normal.x * _playerStateManager.GetDirX() > 0
            && !Input.GetButtonDown(GameConstants.JUMP_BUTTON) && !_playerStateManager.GetIsOnGround()
            || !_playerStateManager.GetIsOnGround() && !_playerStateManager.GetIsWallTouch()
            && _playerStateManager.GetRigidBody2D().velocity.y < -GameConstants.NEAR_ZERO_THRESHOLD;
    }

    private bool CheckIfCanDash()
    {
        return Input.GetButtonDown(GameConstants.DASH_BUTTON)
             && Time.time - _playerStateManager.dashState.DashDelayStart >= _playerStateManager.GetPlayerStats.DelayDashTime
             || Input.GetButtonDown(GameConstants.DASH_BUTTON) && _playerStateManager.dashState.IsFirstTimeDash;
    }

    public override void FixedUpdate()
    {
        _playerStateManager.GetRigidBody2D().velocity = new Vector2(0f, -_playerStateManager.GetPlayerStats.WallSlideSpeed);
    }
}