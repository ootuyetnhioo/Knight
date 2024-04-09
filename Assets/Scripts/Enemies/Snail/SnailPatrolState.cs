using UnityEngine;

public class SnailPatrolState : MEnemiesPatrolState
{
    private SnailManager _snailManager;

    public override void EnterState(CharactersManager charactersManager)
    {
        _snailManager = (SnailManager)charactersManager;
        _snailManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EMEnemiesState.patrol);
        _entryTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void Update(){}

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Move()
    {
        if (!_snailManager.IsMovingVertical)
        {
            if (_snailManager.HasRotate && !_snailManager.DoneRotate)
            {
                if (_snailManager.Direction == 1)
                {
                    _snailManager.GetRigidbody2D().velocity = new Vector2(-_snailManager.MEnemiesSO.PatrolSpeed.x, -_snailManager.MEnemiesSO.PatrolSpeed.y);
                }
                else
                {
                    _snailManager.GetRigidbody2D().velocity = new Vector2(_snailManager.MEnemiesSO.PatrolSpeed.x, _snailManager.MEnemiesSO.PatrolSpeed.y);
                }
            }
            else
            {
                switch (_snailManager.Direction)
                {
                    case 1:
                        _snailManager.GetRigidbody2D().velocity = new Vector2(-_snailManager.MEnemiesSO.PatrolSpeed.x, 0f);
                        break;
                    case 3:
                        _snailManager.GetRigidbody2D().velocity = new Vector2(_snailManager.MEnemiesSO.PatrolSpeed.x, 0f);
                        break;
                }
            }
        }
        else
        {
            if (_snailManager.HasRotate && !_snailManager.DoneRotate)
            {
                if (_snailManager.Direction == 2)
                {
                    _snailManager.GetRigidbody2D().velocity = new Vector2(_snailManager.MEnemiesSO.PatrolSpeed.x, -_snailManager.MEnemiesSO.PatrolSpeed.y);
                }
                else
                {
                    _snailManager.GetRigidbody2D().velocity = new Vector2(-_snailManager.MEnemiesSO.PatrolSpeed.x, _snailManager.MEnemiesSO.PatrolSpeed.y);
                }
            }
            else
            {
                switch (_snailManager.Direction)
                {
                    case 2:
                        _snailManager.GetRigidbody2D().velocity = new Vector2(0f, -_snailManager.MEnemiesSO.PatrolSpeed.x);
                        break;
                    case 4:
                        _snailManager.GetRigidbody2D().velocity = new Vector2(0f, _snailManager.MEnemiesSO.PatrolSpeed.x);
                        break;
                }
            }
        }
    }

    protected override bool CheckIfCanAttack()
    {
        return _snailManager.HasDetectedPlayer && !_hasChangedState;
    }
}
