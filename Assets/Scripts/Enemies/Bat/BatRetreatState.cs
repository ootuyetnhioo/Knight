using UnityEngine;

public class BatRetreatState : MEnemiesBaseState
{
    private BatManager _batManager;

    public override void EnterState(CharactersManager charactersManager)
    {
        base.EnterState(charactersManager);
        _batManager = (BatManager)charactersManager;
        _batManager.Animator.SetInteger(GameConstants.ANIM_PARA_STATE, (int)GameEnums.EBatState.retreat);
        Debug.Log("Rt");
    }

    public override void ExitState() { }

    public override void Update() 
    {
        _batManager.transform.position = Vector2.MoveTowards(_batManager.transform.position, _batManager.SleepPos.position, _batManager.MEnemiesSO.ChaseSpeed.x * Time.deltaTime);

        if (CheckIfBackHome(_batManager.transform.position, _batManager.SleepPos.position))
            _batManager.ChangeState(_batManager.BatCeilInState);
    }

    private bool CheckIfBackHome(Vector2 batPos, Vector2 sleepPos)
    {
        if (Vector2.Distance(batPos, sleepPos) < 0.1f)
            return true;
        return false;
    }

    public override void FixedUpdate() { }
}
