using UnityEngine;

public class DashState : PlayerBaseState
{
    private bool _allowUpdate;
    private float _dashDelayStart; 
    private bool _isFirstTimeDash = true;

    public bool AllowUpdate { set {  _allowUpdate = value; } }

    public float DashDelayStart { get { return _dashDelayStart; } }

    public bool IsFirstTimeDash { get { return _isFirstTimeDash; } }

    public override void EnterState(PlayerStateManager playerStateManager)
    {
        base.EnterState(playerStateManager);
        _playerStateManager.GetAnimator().SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EPlayerState.dash);
        _playerStateManager.GetDustPS().Play();
        _isFirstTimeDash = false;
        HandleIfPrevStateWallSlide();
        HandleDash();
    }

    public override void ExitState() 
    { 
        _allowUpdate = false;
        _dashDelayStart = Time.time;
        _playerStateManager.GetTrailRenderer().emitting = false;
        _playerStateManager.gameObject.layer = LayerMask.NameToLayer(GameConstants.PLAYER_LAYER);
        BuffsManager.Instance.GetTypeOfBuff(GameEnums.EBuffs.Shield).gameObject.layer = LayerMask.NameToLayer(GameConstants.SHIELD_LAYER);
        _playerStateManager.GetRigidBody2D().gravityScale = _playerStateManager.GetPlayerStats.GravScale;
    }

    public override void Update()
    {
        if (_allowUpdate)
        {
            _playerStateManager.GetRigidBody2D().gravityScale = _playerStateManager.GetPlayerStats.GravScale;

            if (CheckIfCanIdle())
                _playerStateManager.ChangeState(_playerStateManager.idleState);
            else if(CheckIfCanRun())
                _playerStateManager.ChangeState(_playerStateManager.runState);
            else if (CheckIfCanFall())
            {
                _playerStateManager.GetRigidBody2D().velocity = Vector2.zero;

                _playerStateManager.ChangeState(_playerStateManager.fallState);
            }
            else if (CheckIfCanWallSlide())
                _playerStateManager.ChangeState(_playerStateManager.wallSlideState);
        }
    }

    private bool CheckIfCanIdle()
    {
        return Mathf.Abs(_playerStateManager.GetDirX()) < GameConstants.NEAR_ZERO_THRESHOLD && _playerStateManager.GetIsOnGround();
    }

    private bool CheckIfCanRun()
    {
        return Mathf.Abs(_playerStateManager.GetDirX()) > GameConstants.NEAR_ZERO_THRESHOLD && _playerStateManager.GetIsOnGround(); 
    }

    private bool CheckIfCanFall()
    {
        return _playerStateManager.GetRigidBody2D().velocity.y < -GameConstants.NEAR_ZERO_THRESHOLD;
    }

    private bool CheckIfCanWallSlide()
    {
        return _playerStateManager.GetIsWallTouch()
            && _playerStateManager.GetDirX() * _playerStateManager.WallHit.normal.x < 0f;
    }

    private void HandleDash()
    {
        _playerStateManager.GetRigidBody2D().gravityScale = 0f;
        SoundsManager.Instance.PlaySfx(GameEnums.ESoundName.PlayerDashSfx, 0.5f);
        _playerStateManager.GetTrailRenderer().emitting = true;
        _playerStateManager.gameObject.layer = LayerMask.NameToLayer(GameConstants.IGNORE_ENEMIES_LAYER);
        BuffsManager.Instance.GetTypeOfBuff(GameEnums.EBuffs.Shield).gameObject.layer = LayerMask.NameToLayer(GameConstants.IGNORE_ENEMIES_LAYER);

        if (_playerStateManager.GetIsFacingRight())
            _playerStateManager.GetRigidBody2D().velocity = new Vector2(_playerStateManager.GetPlayerStats.DashForce.x, 0f);
        else
            _playerStateManager.GetRigidBody2D().velocity = new Vector2(-_playerStateManager.GetPlayerStats.DashForce.x, 0f);
    }

    private void HandleIfPrevStateWallSlide()
    {
        if (_playerStateManager.GetPrevStateIsWallSlide())
            _playerStateManager.FlipSpriteAfterWallSlide();
    }

    public override void FixedUpdate(){}
}