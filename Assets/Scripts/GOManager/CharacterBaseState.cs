using UnityEngine;

public class CharacterBaseState 
{
    protected CharactersManager _charactersManager;

    public virtual void EnterState(CharactersManager charactersManager) { _charactersManager = charactersManager; }

    public virtual void ExitState() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }
}
