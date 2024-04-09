using UnityEngine;

public abstract class PlayerBaseState : CharacterBaseState
{
    protected PlayerStateManager _playerStateManager;

    public virtual void EnterState(PlayerStateManager playerStateManager) { _playerStateManager = playerStateManager; }

    public virtual void ExitState() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }
}