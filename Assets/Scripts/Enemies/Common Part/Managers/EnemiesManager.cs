using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : CharactersManager
{
    [Header("Player Check")]
    [SerializeField] protected Transform _playerCheck;

    [Header("SO")]
    [SerializeField] protected EnemiesStats _enemiesSO;

    protected bool _hasDetectedPlayer;
    protected bool _hasGotHit;
    protected Collider2D _collider2D;
    protected SpriteRenderer _spriteRenderer;
    protected bool _hasNotified;
    protected bool _notPlayDeadSfx;

    #region GETTER

    public bool HasDetectedPlayer { get { return _hasDetectedPlayer; } }

    public Collider2D GetCollider2D { get { return _collider2D; } set { _collider2D = value; } }

    public SpriteRenderer GetSpriteRenderer { get => _spriteRenderer; }

    public EnemiesStats EnemiesSO { get => _enemiesSO; }

    public bool NotPlayDeadSfx { get => _notPlayDeadSfx; }

    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void GetReferenceComponents()
    {
        base.GetReferenceComponents();
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnEnable()
    {
        EventsManager.Instance.SubcribeToAnEvent(GameEnums.EEvents.ObjectOnRestart, OnRestartID);
        EventsManager.Instance.SubcribeToAnEvent(GameEnums.EEvents.BossOnSummonMinion, ReceiveBossCommand);
        EventsManager.Instance.SubcribeToAnEvent(GameEnums.EEvents.BossOnDie, HandleIfBossDie);
    }

    protected virtual void OnDestroy()
    {
        EventsManager.Instance.UnSubcribeToAnEvent(GameEnums.EEvents.ObjectOnRestart, OnRestartID);
        EventsManager.Instance.UnSubcribeToAnEvent(GameEnums.EEvents.BossOnSummonMinion, ReceiveBossCommand);
        EventsManager.Instance.UnSubcribeToAnEvent(GameEnums.EEvents.BossOnDie, HandleIfBossDie);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void SetUpProperties()
    {
        if (Math.Abs(transform.rotation.eulerAngles.y) >= 180f)
            _isFacingRight = true;
    }

    protected override void Update()
    {
        base.Update();
        DetectPlayer();
        DrawRayDetectPlayer();
        HandleIfIsSpecialEnemy();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GameConstants.PLAYER_TAG))
            EventsManager.Instance.NotifyObservers(GameEnums.EEvents.PlayerOnTakeDamage, _isFacingRight);
    }

    protected virtual void DetectPlayer()
    {
        if (BuffsManager.Instance.GetTypeOfBuff(GameEnums.EBuffs.Invisible).IsAllowToUpdate)
        {
            _hasDetectedPlayer = false;
            return;
        }

        if (!_isFacingRight)
        {
            _hasDetectedPlayer = Physics2D.Raycast(_playerCheck.position, Vector2.left, _enemiesSO.PlayerCheckDistance, _enemiesSO.PlayerLayer);
        }
        else
        {
            _hasDetectedPlayer = Physics2D.Raycast(_playerCheck.position, Vector2.right, _enemiesSO.PlayerCheckDistance, _enemiesSO.PlayerLayer);
        }
    }

    protected virtual void DrawRayDetectPlayer()
    {
        if (_hasDetectedPlayer)
        {
            if (!_isFacingRight)
                Debug.DrawRay(_playerCheck.position, Vector2.left * _enemiesSO.PlayerCheckDistance, Color.red);
            else
                Debug.DrawRay(_playerCheck.position, Vector2.right * _enemiesSO.PlayerCheckDistance, Color.red);
        }
        else
        {
            if (!_isFacingRight)
                Debug.DrawRay(_playerCheck.position, Vector2.left * _enemiesSO.PlayerCheckDistance, Color.green);
            else
                Debug.DrawRay(_playerCheck.position, Vector2.right * _enemiesSO.PlayerCheckDistance, Color.green);
        }
    }

    protected virtual void SelfDestroy()
    {
        Destroy(gameObject);
    }

    protected void OnRestartID(object obj)
    {
        _ID = null;
    }

    protected virtual void HandleIfBossDie(object obj) { }

    protected void ReceiveBossCommand(object obj)
    {
        if ((bool)obj != _isFacingRight)
            FlippingSprite();
        _isFacingRight = (bool)obj;
    }

    protected void HandleIfIsSpecialEnemy()
    {
        if (_hasGotHit && !_hasNotified && _needTutor)
        {
            _hasNotified = true;
            EventsManager.Instance.NotifyObservers(GameEnums.EEvents.TutorOnDestroy, _tutorRef);

            if (_isApplySkillToPlayer)
                StartCoroutine(NotifyUnlockSkill());
        }
    }

}
