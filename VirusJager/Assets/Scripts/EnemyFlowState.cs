using UnityEngine;

public class EnemyFlowState : IEnemyState
{
    private readonly EnemnyFSM _fsm;

    public EnemyFlowState(EnemnyFSM fsm)
    {
        _fsm = fsm;
    }

    public void Enter() { }
    public void Execute() { }
    public void Exit() { }

    public void OnPlayerHit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player hit virus!");
            //add oxygen 
            //add score 

            _fsm.ChangeState(_fsm.dieState);
        }
    }

}
