using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameEnums;

public class PlayerStateManager : MonoBehaviour
{
    private PlayerBaseState _state;
    public IdleState idleState = new();
    public RunState runState = new();
    public JumpState jumpState = new();
    public FallState fallState = new();
    public DoubleJumpState doubleJumpState = new();
    public WallSlideState wallSlideState = new();
    public GotHitState gotHitState = new();
    public WallJumpState wallJumpState = new();
    public DashState dashState = new();

    private float dirX, dirY;
    private float _jumpStart;
    private float _startCoyote;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer _spriteRenderer;
    private CapsuleCollider2D _capCollider2D;
    private RaycastHit2D wallHit;
    private bool isOnGround = false;
    private bool _canDbJump = false;
    private bool IsWallTouch = false;
    private bool isFacingRight = true;
    private bool prevStateIsWallSlide = false;
    private bool hasSpawnDust = false;
    private bool _isInteractingWithNPC;
    private bool _hasChange;
    private bool _hasFlip;
    private bool _hasDetectedNPC;
    private bool _hasBeenDisabled;
    private bool _isApplyGotHitEffect;
    private bool _hasStartCoroutine;
    private Vector2 _interactPosition;
    private bool _isVunerable;
    private bool _isHitFromRightSide;
    private bool _isOnPlatform;
    private bool _hasDead;
    private bool _hasStart;
    private bool _canJump;
    private bool _forceApply;
    private bool _hasWinGame;

    private bool _unlockedDbJump;
    private bool _unlockedWallSlide;
    private bool _unlockedDash;

    [Header("Dust")]
    [SerializeField] ParticleSystem dustPS;
    private ParticleSystem.VelocityOverLifetimeModule dustVelocity;  

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 1.0f;

    [Header("Wall Check")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance;

    [Header("NPC Check")]
    [SerializeField] private Transform _npcCheck;
    [SerializeField] private LayerMask _npcLayer;
    [SerializeField] private float _npcCheckDistance;

    [Header("SO")]
    [SerializeField] private PlayerStats _playerStats;
   
    [Header("Trail Renderer")]
    [SerializeField] private TrailRenderer _trailRenderer;

    [Header("Dashable Sign")]
    [SerializeField] private Transform _dashableSignPos;

    public Transform DashableSignPosition { get { return _dashableSignPos; } }

    public float JumpStart { get { return _jumpStart; } set { _jumpStart = value; } }

    public float GetDirX() { return dirX; }

    public float GetDirY() { return dirY; }

    public bool GetIsOnGround() { return isOnGround; }

    public Rigidbody2D GetRigidBody2D() { return rb; }

    public Animator GetAnimator() { return anim; }

    public bool GetCanDbJump() { return _canDbJump; }

    public bool GetIsWallTouch() { return IsWallTouch; }

    public bool GetPrevStateIsWallSlide() { return prevStateIsWallSlide; }
    
    public bool GetIsFacingRight() { return isFacingRight; }

    public bool IsInteractingWithNPC { get { return _isInteractingWithNPC; } set { _isInteractingWithNPC = value; } }

    public ParticleSystem GetDustPS() { return dustPS; }

    public TrailRenderer GetTrailRenderer() { return _trailRenderer; }

    public RaycastHit2D WallHit { get { return wallHit; } }

    public bool HasDetectedNPC { get { return _hasDetectedNPC; } }

    public bool IsHitFromRightSide { get => _isHitFromRightSide; set => _isHitFromRightSide = value; }

    public bool IsVunerable { set => _isVunerable = value; }

    public bool CanJump { get => _canJump; }

    public bool ForceApply { get => _forceApply; }

    public void SetCanDbJump(bool para) { _canDbJump = para; }

    public Vector2 InteractPosition { get { return _interactPosition; } set { _interactPosition = value; } }

    public PlayerStats GetPlayerStats { get { return _playerStats; } set { _playerStats = value; } } 

    public bool IsApplyGotHitEffect { set { _isApplyGotHitEffect = value; } }
    
    private void Awake()
    {
        GetReferenceComponents();
    }

    private void GetReferenceComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        dustVelocity = GameObject.Find("Dust").GetComponent<ParticleSystem>().velocityOverLifetime;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _capCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void UpdatePosition()
    {
        if (PlayerPrefs.HasKey(ESpecialStates.PlayerPositionUpdatedX.ToString()))
        {
            float x = PlayerPrefs.GetFloat(ESpecialStates.PlayerPositionUpdatedX.ToString());
            float y = PlayerPrefs.GetFloat(ESpecialStates.PlayerPositionUpdatedY.ToString());
            float z = PlayerPrefs.GetFloat(ESpecialStates.PlayerPositionUpdatedZ.ToString());
            transform.position = new Vector3(x, y, z);
        }
    }

    private void HandlePlayerSkills()
    {
        if (PlayerPrefs.HasKey(ESpecialStates.PlayerSkillUnlockedLV2.ToString()))
            _unlockedDbJump = _unlockedWallSlide = _unlockedDash = true;
        else if (PlayerPrefs.HasKey(ESpecialStates.PlayerSkillUnlockedLV1.ToString()))
            _unlockedDbJump = _unlockedWallSlide = true;
        else
        {
            if (PlayerPrefs.HasKey(ESpecialStates.SkillUnlocked + EPlayerState.doubleJump.ToString()))
                _unlockedDbJump = true;
            if (PlayerPrefs.HasKey(ESpecialStates.SkillUnlocked + EPlayerState.wallSlide.ToString()))
                _unlockedWallSlide = true;
            if (PlayerPrefs.HasKey(ESpecialStates.SkillUnlocked + EPlayerState.dash.ToString()))
                _unlockedDash = true;
        }
    }

    private void Start()
    {
        RegisterFunction();
        SetupProperties();
        UpdatePosition();
        HandlePlayerSkills();
    }

    private void RegisterFunction()
    {
        EventsManager.Instance.SubcribeToAnEvent(EEvents.PlayerOnTakeDamage, BeingDamaged);
        EventsManager.Instance.SubcribeToAnEvent(EEvents.PlayerOnJumpPassive, JumpPassive);
        EventsManager.Instance.SubcribeToAnEvent(EEvents.PlayerOnInteractWithNPCs, InteractWithNPC);
        EventsManager.Instance.SubcribeToAnEvent(EEvents.PlayerOnStopInteractWithNPCs, StopInteractWithNPC);
        EventsManager.Instance.SubcribeToAnEvent(EEvents.PlayerOnBeingPushedBack, PushBack);
        EventsManager.Instance.SubcribeToAnEvent(EEvents.PlayerOnUpdateRespawnPosition, UpdateRespawnPosition);
        EventsManager.Instance.SubcribeToAnEvent(EEvents.PlayerOnUnlockSkills, UnlockSkill);
        EventsManager.Instance.SubcribeToAnEvent(EEvents.PlayerOnWinGame, HandleWinGame);
    }

    private void SetupProperties()
    {
        _state = idleState;
        _state.EnterState(this);
        rb.gravityScale = _playerStats.GravScale;
    }

    private void OnDestroy()
    {
        UnsubcribeAllEvents();
    }

    private void UnsubcribeAllEvents()
    {
        EventsManager.Instance.UnSubcribeToAnEvent(EEvents.PlayerOnTakeDamage, BeingDamaged);
        EventsManager.Instance.UnSubcribeToAnEvent(EEvents.PlayerOnJumpPassive, JumpPassive);
        EventsManager.Instance.UnSubcribeToAnEvent(EEvents.PlayerOnInteractWithNPCs, InteractWithNPC);
        EventsManager.Instance.UnSubcribeToAnEvent(EEvents.PlayerOnStopInteractWithNPCs, StopInteractWithNPC);
        EventsManager.Instance.UnSubcribeToAnEvent(EEvents.PlayerOnBeingPushedBack, PushBack);
        EventsManager.Instance.UnSubcribeToAnEvent(EEvents.PlayerOnUpdateRespawnPosition, UpdateRespawnPosition);
        EventsManager.Instance.UnSubcribeToAnEvent(EEvents.PlayerOnUnlockSkills, UnlockSkill);
        EventsManager.Instance.UnSubcribeToAnEvent(EEvents.PlayerOnWinGame, HandleWinGame);
    }

    public void ChangeState(PlayerBaseState state)
    {
        if (_isInteractingWithNPC && state is not RunState && state is not IdleState)
            return;

        if (state is GotHitState && Time.time - gotHitState.EntryTime <= _playerStats.InvulnerableTime && _isVunerable)
            return;

        if (_hasWinGame && state is not FallState && state is not IdleState)
            return;

        if (state is GotHitState && BuffsManager.Instance.GetTypeOfBuff(EBuffs.Shield).IsAllowToUpdate)
        {
            return;
        }

        if (state is DoubleJumpState && !_unlockedDbJump)
            return;
        else if (state is WallSlideState && !_unlockedWallSlide)
            return;
        else if (state is DashState && !_unlockedDash)
            return;

        _state.ExitState();
        _state = state;
        if (state is WallSlideState)
            prevStateIsWallSlide = true;
        _state.EnterState(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GameConstants.GROUND_TAG) || collision.collider.CompareTag(GameConstants.PLATFORM_TAG))
            HandleCollideGround();
        else if (collision.collider.CompareTag(GameConstants.TRAP_TAG) && _state is not GotHitState)
        {
            gotHitState.IsHitByTrap = true;
            ChangeState(gotHitState);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GameConstants.TRAP_TAG) && _state is not GotHitState)
        {
            gotHitState.IsHitByTrap = true;
            ChangeState(gotHitState);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.PLATFORM_TAG))
        {
            transform.SetParent(collision.gameObject.transform);
            _isOnPlatform = true;
        }
        else if (collision.CompareTag(GameConstants.TRAP_TAG) && _state is not GotHitState)
        {
            gotHitState.IsHitByTrap = true;
            ChangeState(gotHitState);
        }
        else if (collision.CompareTag(GameConstants.DEAD_ZONE_TAG))
            HandleDeadState();
        else if (collision.CompareTag(GameConstants.PORTAL_TAG))
        {
            SoundsManager.Instance.PlaySfx(ESoundName.GreenPortalSfx, 1.0f);
            anim.SetTrigger(GameConstants.DEAD_ANIMATION);
            rb.bodyType = RigidbodyType2D.Static;
            GameManager.Instance.SwitchToScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.TRAP_TAG) && _state is not GotHitState)
        {
            gotHitState.IsHitByTrap = true;
            ChangeState(gotHitState);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.PLATFORM_TAG))
        {
            transform.SetParent(null);
            _isOnPlatform = false;
        }
    }

    private void BeingDamaged(object obj)
    {
        _isHitFromRightSide = (bool)obj;
        ChangeState(gotHitState);
    }

    private void JumpPassive(object obj)
    {
        _canDbJump = true;
        if (obj != null)
            jumpState.JumpForceApplied = (float)obj;
        ChangeState(jumpState);
    }

    private void InteractWithNPC(object obj)
    {
        _isInteractingWithNPC = true;
        _interactPosition = (Vector2)obj;
        Debug.Log("inter: " + _interactPosition);
    }

    private void StopInteractWithNPC(object obj)
    {
        _isInteractingWithNPC = false;
    }

    void Update()
    {
        if (_hasBeenDisabled)
            return;

        if (PlayerHealthManager.Instance.CurrentHP == 0)
        {
            HandleDeadState();
            return;
        }

        NPCCheck();
        DrawRayDetectNPC();

        if (_isInteractingWithNPC)
        {
            HandleInteractWithNPC();
            return;
        }
        else
        {
            _hasChange = false;
            _hasFlip = false;
        }

        LockIfOutMinBound();
        UpdateLayer();
        HandleInput();
        GroundAndWallCheck();
        HandleCoyoteTime();
        _state.Update();
        HandleFlipSprite();
        HandleAlphaValueGotHit();
        HandleDustVelocity();
        SpawnDust();
    }

    private void UpdateLayer()
    {
        if (Time.time - gotHitState.EntryTime <= _playerStats.InvulnerableTime && _isVunerable)
            gameObject.layer = LayerMask.NameToLayer(GameConstants.IGNORE_ENEMIES_LAYER);
        else if (_state is not DashState)
        {
            _isVunerable = false;
            gameObject.layer = LayerMask.NameToLayer(GameConstants.PLAYER_LAYER);
        }
    }

    private void HandleInteractWithNPC()
    {
        HandleFlipTowardsInteractPos();
        AllowMoveTowardsInteractPos();
        _state.Update();
    }

    private void HandleFlipTowardsInteractPos()
    {
        if (!_hasFlip)
        {
            _hasFlip = true;

            if (Mathf.Abs(transform.position.x - _interactPosition.x) < GameConstants.NEAR_CONVERSATION_RANGE)
                return;

            if (isFacingRight && transform.position.x > InteractPosition.x + GameConstants.CAN_START_CONVERSATION_RANGE)
            {
                FlippingSprite();
            }
            else if (!isFacingRight && transform.position.x < InteractPosition.x - GameConstants.CAN_START_CONVERSATION_RANGE)
            {
                FlippingSprite();
            }
        }
    }

    private void AllowMoveTowardsInteractPos()
    {
        if (!_hasChange)
        {
            _hasChange = true;
            if (Mathf.Abs(transform.position.x - _interactPosition.x) < GameConstants.NEAR_CONVERSATION_RANGE)
                ChangeState(idleState);
            else
            {
                Debug.Log("run");
                Invoke(nameof(ChangeToRun), GameConstants.DELAYPLAYERRUNSTATE);
            }
        }
    }

    private void FixedUpdate()
    {
        _state.FixedUpdate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (isFacingRight)
            Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        else
            Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x - wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

    private void HandleInput()
    {
        if (_hasWinGame) return;
        dirX = Input.GetAxisRaw(GameConstants.HORIZONTAL_AXIS);
        dirY = Input.GetAxisRaw(GameConstants.VERTICAL_AXIS);
    }

    public void FlippingSprite()
    {
        if(_state is WallJumpState)
        {
            if(wallJumpState.IsEndDisable)
            {
                isFacingRight = !isFacingRight;
                transform.Rotate(0, 180, 0);
            }
        }
        else
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180, 0);
        }
    }

    public void FlipSpriteAfterWallSlide()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
        prevStateIsWallSlide = false;
    }

    private void HandleFlipSprite()
    {
        if (_state is DashState) 
            return;

        if(!IsWallTouch)
        {
            if (dirX > 0f && !isFacingRight)
            {
                FlippingSprite();
            }
            else if (dirX < 0f && isFacingRight)
            {
                FlippingSprite();
            }
        }
    }

    private void GroundAndWallCheck()
    {
        if (!_isOnPlatform)
            _canJump = isOnGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, wallLayer);
        else
            _canJump = true;
        if (isFacingRight)
        {
            IsWallTouch = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, wallLayer);
            wallHit = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, wallLayer);
        }
        else
        {
            IsWallTouch = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, wallLayer);
            wallHit = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, wallLayer);
        }
    }

    private void NPCCheck()
    {
        if(isFacingRight)
            _hasDetectedNPC = Physics2D.Raycast(_npcCheck.position, Vector2.right, _npcCheckDistance, _npcLayer);
        else
            _hasDetectedNPC = Physics2D.Raycast(_npcCheck.position, Vector2.left, _npcCheckDistance, _npcLayer);
    }

    private void DrawRayDetectNPC()
    {
        if(!_hasDetectedNPC)
        {
            if (isFacingRight)
                Debug.DrawRay(_npcCheck.position, Vector2.right * _npcCheckDistance, Color.green);
            else
                Debug.DrawRay(_npcCheck.position, Vector2.left * _npcCheckDistance, Color.green);
        }
        else
        {
            if (isFacingRight)
                Debug.DrawRay(_npcCheck.position, Vector2.right * _npcCheckDistance, Color.red);
            else
                Debug.DrawRay(_npcCheck.position, Vector2.left * _npcCheckDistance, Color.red);
        }
    }

    private void ChangeToIdle()
    {
        ChangeState(idleState);
    }

    private void ChangeToRun()
    {
        ChangeState(runState);
    }

    private void AllowUpdateDash()
    {
        dashState.AllowUpdate = true;
    }

    public void HandleDeadState()
    {
        if (!_hasDead)
            _hasDead = true;
        else
            return;

        if (PlayerHealthManager.Instance.CurrentHP > 0)
        {
            PlayerHealthManager.Instance.ChangeHPState(GameConstants.HP_STATE_LOST);
            if (PlayerHealthManager.Instance.CurrentHP == 0)
                UIManager.Instance.StartCoroutine(UIManager.Instance.PopUpLoosePanel());
            else
                GameManager.Instance.SwitchToScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
            UIManager.Instance.StartCoroutine(UIManager.Instance.PopUpLoosePanel());

        anim.SetTrigger(GameConstants.DEAD_ANIMATION);
        rb.bodyType = RigidbodyType2D.Static;
        _capCollider2D.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Enemies");
        SoundsManager.Instance.PlaySfx(ESoundName.PlayerDeadSfx, 1.0f);
    }

    private void HandleCoyoteTime()
    {
        if (!isOnGround && !_hasStart)
        {
            _hasStart = true;
            _startCoyote = Time.time;
        }
        else if (isOnGround)
            _hasStart = false; 

        if (Time.time - _startCoyote <= _playerStats.CoyoteTime && _hasStart)
            _canJump = true;
        else if (Time.time - _startCoyote > _playerStats.CoyoteTime && _hasStart)
            _canJump = false;
    }

    private void HandleCollideGround()
    {
        isOnGround = true;
        _canDbJump = true;
    }

    private void SpawnDust()
    {
        if (isOnGround && !hasSpawnDust)
        {
            hasSpawnDust = true;
            dustPS.Play();
        }
        else if (!isOnGround)
            hasSpawnDust = false;
    }

    private void HandleDustVelocity()
    {
        if (isFacingRight)
            dustVelocity.x = -0.3f;
        else
            dustVelocity.x = 0.3f;
    }

    public void Disable()
    {
        ChangeState(idleState);
        _hasBeenDisabled = true;
    }

    public void Enable()
    {
        _hasBeenDisabled = false;
        Debug.Log("Enable");
    }

    private IEnumerator Twinkling()
    {
        _hasStartCoroutine = true;
        _spriteRenderer.color = new Color(1f, 1f, 1f, _playerStats.AlphaValueGotHit);

        yield return new WaitForSeconds(_playerStats.TimeEachApplyAlpha);

        if (BuffsManager.Instance.GetTypeOfBuff(EBuffs.Invisible).IsAllowToUpdate)
        {
            _hasStartCoroutine = false;
            yield return null;
        }
        else
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);

        yield return new WaitForSeconds(_playerStats.TimeEachApplyAlpha);

        _hasStartCoroutine = false;
    }

    private void HandleAlphaValueGotHit()
    {
        if (BuffsManager.Instance.GetTypeOfBuff(EBuffs.Invisible).IsAllowToUpdate)
            return;

        if (Time.time - gotHitState.EntryTime <= _playerStats.InvulnerableTime && !_hasStartCoroutine && _isApplyGotHitEffect)
            StartCoroutine(Twinkling());
        else if (Time.time - gotHitState.EntryTime > _playerStats.InvulnerableTime)
        {
            if (BuffsManager.Instance.GetTypeOfBuff(EBuffs.Invisible).IsAllowToUpdate)
            {
                _isApplyGotHitEffect = false;
                return;
            }

            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            _isApplyGotHitEffect = false;
        }
    }

    private void SpawnDashableEffect()
    {
        GameObject dEff = Pool.Instance.GetObjectInPool(EPoolable.Dashable);
        dEff.SetActive(true);
    }

    private void LockIfOutMinBound()
    {
        if (transform.position.x < GameConstants.GAME_MIN_BOUNDARY)
            transform.position = new Vector3(GameConstants.GAME_MIN_BOUNDARY, transform.position.y, transform.position.z);
        else if(transform.position.x > GameConstants.GAME_MAX_BOUNDARY)
            transform.position = new Vector3(GameConstants.GAME_MAX_BOUNDARY, transform.position.y, transform.position.z);
    }

    private void PushBack(object obj)
    {
        PushBackInfor pInfo = (PushBackInfor)obj;
        if (rb)
            rb.AddForce(pInfo.IsPushFromRight ? pInfo.PushForce * new Vector2(-1f, 1f) : pInfo.PushForce);
        else
            Debug.Log("RB Player nULL");
    }

    private void UnlockSkill(object obj)
    {
        switch ((EPlayerState)obj)
        {
            case EPlayerState.doubleJump:
                _unlockedDbJump = true; 
                break;
            case EPlayerState.wallSlide:
                _unlockedWallSlide = true;
                break;
            case EPlayerState.dash:
                _unlockedDash = true;
                PlayerPrefs.SetString(ESpecialStates.PlayerSkillUnlockedLV2.ToString(), "FullUnlock");
                PlayerPrefs.Save();
                break;
        }
    }

    private void UpdateRespawnPosition(object obj)
    {
        Vector3 checkPointPos = (Vector3)obj;
        PlayerPrefs.SetFloat(ESpecialStates.PlayerPositionUpdatedX.ToString(), checkPointPos.x);
        PlayerPrefs.SetFloat(ESpecialStates.PlayerPositionUpdatedY.ToString(), checkPointPos.y);
        PlayerPrefs.SetFloat(ESpecialStates.PlayerPositionUpdatedZ.ToString(), checkPointPos.z);
        PlayerPrefs.Save();
    }

    private void HandleWinGame(object obj)
    {
        _hasWinGame = true;
    }

}