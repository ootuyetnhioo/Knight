using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailManager : MEnemiesManager
{
    [Header("Z Rotation When Move")]
    [SerializeField] private float _degreeEachRotation;
    [SerializeField] private float _rotateTime;
    [SerializeField] private float _timeEachRotation;

    [Header("Delay Rotate")]
    [SerializeField] private float _delayRotate;

    [Header("Offset")]
    [SerializeField] private float _offSet;

    private SnailPatrolState _snailPatrolState = new();
    private SnailGotHitState _snailGotHitState = new();
    private bool _hasRotate;
    private float _entryTime;
    private bool _doneRotate;
    private bool _isMovingVertical;
    private int _direction = 1;
    private bool _hasStart;
    private bool _rotateByWall;
    private RaycastHit2D _groundHit;

    public bool HasRotate { get {  return _hasRotate; } }

    public bool DoneRotate { get { return _doneRotate; } }

    public bool IsMovingVertical { get { return _isMovingVertical; } }   

    public int Direction { get {  return _direction; } }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void GetReferenceComponents()
    {
        base.GetReferenceComponents();
        _collider2D = GetComponent<Collider2D>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void SetUpProperties()
    {
        base.SetUpProperties();
        _state = _snailPatrolState;
        _state.EnterState(this);
    }

    protected override void Update()
    {
        base.Update();
        if (_state is SnailGotHitState)
            return;

        DrawRayDetectPlayer();
        DrawRayDetectWall();

        if (_hasCollidedWall && !_hasStart)
            StartCoroutine(StartTickWallRotation());
        else if (!_hasDetectedGround && !_hasStart)
            StartCoroutine(StartTickGroundRotation());

        if (_rotateByWall)
            RotateIfDetectedWall();
        else
            RotateIfNotDetectedGround();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameConstants.PLAYER_TAG) && !_hasGotHit)
        {
            StopAllCoroutines();
            EventsManager.Instance.NotifyObservers(GameEnums.EEvents.PlayerOnJumpPassive, null);
            _hasGotHit = true;
            ChangeState(_snailGotHitState);
        }
    }

    private IEnumerator StartTickWallRotation()
    {
        _rotateByWall = true;
        _hasStart = true;
        _hasRotate = true;
        _doneRotate = false;
        _entryTime = Time.time;
        yield return null;
    }

    private IEnumerator StartTickGroundRotation()
    {
        _hasStart = true;

        yield return new WaitForSeconds(_delayRotate);

        _hasRotate = true;
        _doneRotate = false;
        _entryTime = Time.time;
    }

    private float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    private void RotateIfDetectedWall()
    {
        if (_rotateByWall)
        {
            if (Time.time - _entryTime >= _timeEachRotation && _hasRotate && !_doneRotate)
            {
                float currentZAngles = WrapAngle(transform.localEulerAngles.z);
                currentZAngles -= _degreeEachRotation;

                if (!_isMovingVertical)
                {
                    if (currentZAngles <= 90f && _direction == 3)
                    {
                        currentZAngles = 90f;
                        _direction = 2;
                        _hasStart = false;
                        _doneRotate = true;
                        _isMovingVertical = true;
                        _hasRotate = false;
                        _rotateByWall = false;
                        transform.position = new Vector3(transform.position.x + _groundHit.distance, transform.position.y, transform.position.z);
                    }
                    else if (currentZAngles <= -90f && _direction == 1)
                    {
                        currentZAngles = -90f;
                        _direction = 4;
                        _hasStart = false;
                        _doneRotate = true;
                        _isMovingVertical = true;
                        _hasRotate = false;
                        _rotateByWall = false;
                        transform.position = new Vector3(transform.position.x - _groundHit.distance, transform.position.y, transform.position.z);
                    }
                }
                else
                {
                    if (currentZAngles <= -180f)
                    {
                        currentZAngles = -180f;
                        _direction = 3;
                        _hasStart = false;
                        _doneRotate = true;
                        _isMovingVertical = false;
                        _hasRotate = false;
                        _rotateByWall = false;
                    }
                    else if (currentZAngles <= 0f && _direction == 2)
                    {
                        currentZAngles = 0f;
                        _direction = 1;
                        _hasStart = false;
                        _doneRotate = true;
                        _isMovingVertical = false;
                        _hasRotate = false;
                        _rotateByWall = false;
                    }
                }

                float currentYAngles = WrapAngle(transform.localEulerAngles.y);
                transform.rotation = Quaternion.Euler(0f, currentYAngles, currentZAngles);
                _entryTime = Time.time;
            }
        }
    }

    private void RotateIfNotDetectedGround()
    {
        if (Time.time - _entryTime >= _timeEachRotation && _hasRotate && !_doneRotate)
        {
            float currentZAngles = WrapAngle(transform.localEulerAngles.z);
            currentZAngles += _degreeEachRotation;

            if (!_isMovingVertical)
            {
                if (currentZAngles >= -90f && currentZAngles < 0f && _direction == 3)
                {
                    currentZAngles = -90f;
                    _direction = 4;
                    _hasStart = false;
                    _doneRotate = true;
                    _isMovingVertical = true;
                    _hasRotate = false;
                }
                else if (currentZAngles >= 90f && _direction == 1)
                {
                    currentZAngles = 90f;
                    _direction = 2;
                    _hasStart = false;
                    _doneRotate = true;
                    _isMovingVertical = true;
                    _hasRotate = false;
                }
            }
            else
            {
                if (currentZAngles >= 180f)
                {
                    currentZAngles = 180f;
                    _direction = 3;
                    _hasStart = false;
                    _doneRotate = true;
                    _isMovingVertical = false;
                    _hasRotate = false;
                    transform.position = new Vector3(transform.position.x, transform.position.y + _offSet, transform.position.z);
                }
                else if( currentZAngles >= 0f && _direction == 4)
                {
                    currentZAngles = 0f;
                    _direction = 1;
                    _hasStart = false;
                    _doneRotate = true;
                    _isMovingVertical = false;
                    _hasRotate = false;
                    transform.position = new Vector3(transform.position.x, transform.position.y - _offSet, transform.position.z);
                }
            }

            float currentYAngles = WrapAngle(transform.localEulerAngles.y);
            transform.rotation = Quaternion.Euler(0f, currentYAngles, currentZAngles);
            _entryTime = Time.time;
        }
    }

    protected override bool DetectWall()
    {
        if (!_isMovingVertical)
        {
            if (_direction == 1)
                _hasCollidedWall = Physics2D.Raycast(_wallCheck.position, Vector2.left, _mEnemiesSO.WallCheckDistance, _mEnemiesSO.WallLayer);
            else
                _hasCollidedWall = Physics2D.Raycast(_wallCheck.position, Vector2.right, _mEnemiesSO.WallCheckDistance, _mEnemiesSO.WallLayer);
        }
        else
        {
            if (_direction == 2)
                _hasCollidedWall = Physics2D.Raycast(_wallCheck.position, Vector2.down, _mEnemiesSO.WallCheckDistance, _mEnemiesSO.WallLayer);
            else
                _hasCollidedWall = Physics2D.Raycast(_wallCheck.position, Vector2.up, _mEnemiesSO.WallCheckDistance, _mEnemiesSO.WallLayer);
        }

        return _hasCollidedWall;
    }

    protected override void DrawRayDetectWall()
    {
        if (!_isMovingVertical)
        {
            if (_hasCollidedWall)
            {
                if (_direction == 1)
                    Debug.DrawRay(_wallCheck.position, Vector2.left, Color.red);
                else
                    Debug.DrawRay(_wallCheck.position, Vector2.right, Color.red);
            }
            else
            {
                if (_direction == 1)
                    Debug.DrawRay(_wallCheck.position, Vector2.left, Color.green);
                else
                    Debug.DrawRay(_wallCheck.position, Vector2.right, Color.green);
            }
        }
        else
        {
            if (_hasCollidedWall)
            {
                if (_direction == 2)
                    Debug.DrawRay(_wallCheck.position, Vector2.down, Color.red);
                else
                    Debug.DrawRay(_wallCheck.position, Vector2.up, Color.red);
            }
            else
            {
                if (_direction == 2)
                    Debug.DrawRay(_wallCheck.position, Vector2.down, Color.green);
                else
                    Debug.DrawRay(_wallCheck.position, Vector2.up, Color.green);
            }
        }
    }

    protected override void DetectGround()
    {
        if (!_isMovingVertical)
        {
            if (_direction == 1)
            {
                _groundHit = Physics2D.Raycast(_groundCheck.position, Vector2.down, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
                _hasDetectedGround = Physics2D.Raycast(_groundCheck.position, Vector2.down, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
                _groundHit = Physics2D.Raycast(_groundCheck.position, Vector2.down, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
            }
            else
            {
                _groundHit = Physics2D.Raycast(_groundCheck.position, Vector2.up, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
                _hasDetectedGround = Physics2D.Raycast(_groundCheck.position, Vector2.up, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
                _groundHit= Physics2D.Raycast(_groundCheck.position, Vector2.up, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
            }
        }
        else
        {
            if (_direction == 2)
            {
                _groundHit = Physics2D.Raycast(_groundCheck.position, Vector2.right, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
                _hasDetectedGround = Physics2D.Raycast(_groundCheck.position, Vector2.right, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
                _groundHit= Physics2D.Raycast(_groundCheck.position, Vector2.right, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
            }
            else
            {
                _groundHit = Physics2D.Raycast(_groundCheck.position, Vector2.left, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
                _hasDetectedGround = Physics2D.Raycast(_groundCheck.position, Vector2.left, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
                _groundHit= Physics2D.Raycast(_groundCheck.position, Vector2.left, _mEnemiesSO.GroundCheckDistance, _mEnemiesSO.WallLayer);
            }
        }
    }

    protected override void DrawRayDetectGround()
    {
        if (!_isMovingVertical)
        {
            if (_direction == 1)
                Debug.DrawRay(_groundCheck.position, Vector2.down, Color.green);
            else
                Debug.DrawRay(_groundCheck.position, Vector2.up, Color.green);
        }
        else
        {
            if (_direction == 2)
                Debug.DrawRay(_groundCheck.position, Vector2.right, Color.green);
            else
                Debug.DrawRay(_groundCheck.position, Vector2.left, Color.green);
        }
    }
}
