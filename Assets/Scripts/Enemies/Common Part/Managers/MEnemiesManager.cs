using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MEnemiesManager : EnemiesManager
{
    protected MEnemiesIdleState _mEnemiesIdleState = new();
    protected MEnemiesPatrolState _mEnemiesPatrolState = new();
    protected MEnemiesAttackState _mEnemiesAttackState = new();
    protected MEnemiesGotHitState _mEnemiesGotHitState = new();

    [Header("SO2")]
    [SerializeField] protected MEnemiesStats _mEnemiesSO;

    [Header("Wall Check")]
    [SerializeField] protected Transform _wallCheck;
    protected bool _hasCollidedWall;

    [Header("Ground Check")]
    [SerializeField] protected Transform _groundCheck;
    protected bool _hasDetectedGround;

    #region GETTER

    public MEnemiesIdleState MEnemiesIdleState { get { return _mEnemiesIdleState; } }
    
    public MEnemiesPatrolState MEnemiesPatrolState { get { return _mEnemiesPatrolState;} set { _mEnemiesPatrolState = value; } }

    public bool HasCollidedWall { get { return _hasCollidedWall; } }

    public bool HasDetectedGround { get { return _hasDetectedGround; } }

    public MEnemiesStats MEnemiesSO { get { return _mEnemiesSO; } }
    
    #endregion
    
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void SetUpProperties()
    {
        base.SetUpProperties();
        _state = _mEnemiesIdleState;
        _state.EnterState(this);
    }

    protected override void Update()
    {
        base.Update();
        DetectWall();
        DrawRayDetectWall();
        DetectGround();
        DrawRayDetectGround();
    }

    protected virtual void FixedUpdate()
    {
        _state.FixedUpdate();
    }

    public override void ChangeState(CharacterBaseState state)
    {
        if (_state is MEnemiesGotHitState) 
            return;

        _state.ExitState();
        _state = state;
        _state.EnterState(this);
    }

    protected virtual bool DetectWall()
    {
        if (!_isFacingRight)
            _hasCollidedWall = Physics2D.Raycast(_wallCheck.position, Vector2.left, _mEnemiesSO.WallCheckDistance, _mEnemiesSO.WallLayer);
        else
            _hasCollidedWall = Physics2D.Raycast(_wallCheck.position, Vector2.right, _mEnemiesSO.WallCheckDistance, _mEnemiesSO.WallLayer);
    
        return _hasCollidedWall;
    }

    protected virtual void DrawRayDetectWall()
    {
        if (!_hasDetectedGround)
        {
            if (!_isFacingRight)
                Debug.DrawRay(_wallCheck.position, Vector2.left * _mEnemiesSO.WallCheckDistance, Color.green);
            else
                Debug.DrawRay(_wallCheck.position, Vector2.right * _mEnemiesSO.WallCheckDistance, Color.green);
        }
        else
        {
            if (!_isFacingRight)
                Debug.DrawRay(_wallCheck.position, Vector2.left * _mEnemiesSO.WallCheckDistance, Color.red);
            else
                Debug.DrawRay(_wallCheck.position, Vector2.right * _mEnemiesSO.WallCheckDistance, Color.red);
        }
    }

    protected virtual void DetectGround()
    {
        _hasDetectedGround = Physics2D.Raycast(_groundCheck.position, Vector2.down, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
    }

    protected virtual void DrawRayDetectGround()
    {
        if (!_hasDetectedGround)
            Debug.DrawRay(_groundCheck.position, Vector2.down * _mEnemiesSO.GroundCheckDistance, Color.green);
        else
            Debug.DrawRay(_groundCheck.position, Vector2.down * _mEnemiesSO.GroundCheckDistance, Color.red);
    }

    protected virtual void AllowAttackPlayer()
    {
        if (BuffsManager.Instance.GetTypeOfBuff(GameEnums.EBuffs.Invisible).IsAllowToUpdate)
        {
            ChangeState(_mEnemiesIdleState);
            return;
        }

        ChangeState(_mEnemiesAttackState);
    }

    protected virtual void ChangeToIdle()
    {
        ChangeState(_mEnemiesIdleState);
    }

    protected override void HandleIfBossDie(object obj)
    {
        _hasGotHit = true;
        _notPlayDeadSfx = true;
        ChangeState(_mEnemiesGotHitState);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.collider.CompareTag(GameConstants.BULLET_TAG))
        {
            string bulletID = collision.collider.GetComponent<BulletController>().BulletID;
            EventsManager.Instance.NotifyObservers(GameEnums.EEvents.BulletOnHit, bulletID);
            ChangeState(_mEnemiesGotHitState);
        }
        else if (collision.collider.CompareTag(GameConstants.BOX_TAG) && _state is MEnemiesPatrolState)
            FlippingSprite();
        else if (collision.collider.CompareTag(GameConstants.TRAP_TAG) && !_hasGotHit && _state is not MEnemiesGotHitState)
        {
            _hasGotHit = true;
            ChangeState(_mEnemiesGotHitState);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.PLAYER_TAG) && !_hasGotHit && _state is not MEnemiesGotHitState)
        {
            _hasGotHit = true;
            EventsManager.Instance.NotifyObservers(GameEnums.EEvents.PlayerOnJumpPassive, null);
            ChangeState(_mEnemiesGotHitState);
        }
        else if (collision.CompareTag(GameConstants.TRAP_TAG) && !_hasGotHit && _state is not MEnemiesGotHitState
            || collision.CompareTag(GameConstants.DEAD_ZONE_TAG) && !_hasGotHit && _state is not MEnemiesGotHitState)
        {
            _hasGotHit = true;
            ChangeState(_mEnemiesGotHitState);
        }
    }
}
