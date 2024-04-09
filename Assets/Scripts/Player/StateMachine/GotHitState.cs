using UnityEngine;
using static GameEnums;

public class GotHitState : PlayerBaseState
{
    private bool _isHitByTrap; 
    private float _entryTime = 0; 

    public bool IsHitByTrap { set { _isHitByTrap = value; } }

    public float EntryTime { get {  return _entryTime; } }

    public override void EnterState(PlayerStateManager playerStateManager)
    {
        base.EnterState(playerStateManager);
        HandleGotHit();
        if (PlayerHealthManager.Instance.CurrentHP > 0)
            _playerStateManager.GetAnimator().SetInteger(GameConstants.ANIM_PARA_STATE, (int)EPlayerState.gotHit);
    }

    public override void ExitState() 
    { 
        _isHitByTrap = false;
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    private void KnockBack()
    {
        if (!BuffsManager.Instance.GetTypeOfBuff(EBuffs.Absorb).IsAllowToUpdate)
        {
            if (_isHitByTrap)
            {
                if (_playerStateManager.GetIsFacingRight())
                    _playerStateManager.GetRigidBody2D().AddForce(new Vector2(-1 * _playerStateManager.GetPlayerStats.KnockBackForce.x, 0f));
                else
                    _playerStateManager.GetRigidBody2D().AddForce(new Vector2(_playerStateManager.GetPlayerStats.KnockBackForce.x, 0f));
            }
            else
            {
                if (_playerStateManager.IsHitFromRightSide)
                    _playerStateManager.GetRigidBody2D().AddForce(new Vector2(_playerStateManager.GetPlayerStats.KnockBackForce.x, 0f));
                else
                    _playerStateManager.GetRigidBody2D().AddForce(new Vector2(-_playerStateManager.GetPlayerStats.KnockBackForce.x, 0f));
            }
        }
    }

    private void HandleGotHit()
    {     
        if (!BuffsManager.Instance.GetTypeOfBuff(EBuffs.Absorb).IsAllowToUpdate)
            PlayerHealthManager.Instance.ChangeHPState(GameConstants.HP_STATE_LOST);
        else
            PlayerHealthManager.Instance.ChangeHPState(GameConstants.HP_STATE_TEMP);

        _playerStateManager.IsVunerable = true;
        _entryTime = Time.time;
        KnockBack();
        _playerStateManager.gameObject.layer = LayerMask.NameToLayer(GameConstants.IGNORE_ENEMIES_LAYER);
        _playerStateManager.IsApplyGotHitEffect = true;
        if (PlayerHealthManager.Instance.CurrentHP > 0)
            SoundsManager.Instance.PlaySfx(ESoundName.PlayerGotHitSfx, 1.0f);
        EventsManager.Instance.NotifyObservers(EEvents.CameraOnShake, null);
    }
}
