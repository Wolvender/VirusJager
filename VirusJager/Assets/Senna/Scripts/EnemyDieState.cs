using UnityEngine;

public class EnemyDieState : IEnemyState
{
    private EnemnyFSM enemy;
    private bool executed = false;

    public EnemyDieState(EnemnyFSM enemy) { this.enemy = enemy; }

    public void Enter()
    {
        if (!executed)
        {
            executed = true;
            enemy.animator.SetTrigger("Die");
            Object.Destroy(enemy.gameObject, 3f); // Delay to finish animation
        }
    }

    public void Execute() { }
    public void Exit() { }
}
