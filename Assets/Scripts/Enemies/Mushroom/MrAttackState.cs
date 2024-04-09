using UnityEngine;

public class MrAttackState : MEnemiesAttackState
{
    private bool _hasChangeState;
    private MushroomManager _mushroomManager;

    public override void EnterState(CharactersManager charactersManager)
    {
        base.EnterState(charactersManager);
        _mushroomManager = (MushroomManager)charactersManager;
        _mushroomManager.Invoke("AllowUpdateAttack", 0.15f);
        _mushroomManager.FlippingSprite();
    }

    public override void ExitState()
    {
        _allowUpdate = false;
        _hasChangeState = false;
    }

    public override void Update()
    {
        if(_allowUpdate)
        {
            if(!_mushroomManager.IsDetected && !_hasChangeState)
            {
                if (!_mushroomManager.HasCollidedWall)
                    _mushroomManager.MEnemiesPatrolState.CanRdDirection = true;
                else
                {
                    _mushroomManager.FlippingSprite();
                    _mushroomManager.MEnemiesPatrolState.CanRdDirection = false;
                    _mushroomManager.MEnemiesPatrolState.SetHasJustHitWall(true);
                }
                _hasChangeState = true;
                _mushroomManager.ChangeState(_mushroomManager.MEnemiesIdleState);
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Attack()
    {
        if(_mushroomManager.GetIsFacingRight())
            _mushroomManager.GetRigidbody2D().velocity = new Vector2(_mushroomManager.MEnemiesSO.ChaseSpeed.x, _mushroomManager.GetRigidbody2D().velocity.y);
        else
            _mushroomManager.GetRigidbody2D().velocity = new Vector2(-_mushroomManager.MEnemiesSO.ChaseSpeed.x, _mushroomManager.GetRigidbody2D().velocity.y);
    }
}
