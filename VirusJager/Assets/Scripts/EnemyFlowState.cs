using UnityEngine;
using UnityEngine.AI;

public class EnemyFlowState : IEnemyState
{
    private EnemnyFSM enemy;
    private int currentPointIndex = 0;
    private Vector3 randomOffset;
    private float dodgeTimer = 0f;
    private float dodgeInterval = 2f; // Every 2 seconds, pick a new random offset
    private float offsetRange = 1.5f; // Max random offset from path

    public EnemyFlowState(EnemnyFSM enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.agent.isStopped = false;
        PickRandomOffset();
    }

    public void Execute()
    {
        if (enemy.player == null || enemy.flowPath.Length == 0) return;

        // Move along the path (blood vessel)
        Transform targetPoint = enemy.flowPath[currentPointIndex];
        if (Vector3.Distance(enemy.transform.position, targetPoint.position) < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % enemy.flowPath.Length;
        }

        // Start with the base path position
        Vector3 target = targetPoint.position;

        // Check distance to player for dodging
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
        float dodgeDistance = 5f; // dodge only if player is within this range

        if (distanceToPlayer < dodgeDistance)
        {
            // Update random dodge offset periodically
            dodgeTimer += Time.deltaTime;
            if (dodgeTimer >= dodgeInterval)
            {
                PickRandomOffset();
                dodgeTimer = 0f;
            }

            // Apply the dodge offset
            target += randomOffset;
        }

        // Move agent to the target
        enemy.agent.SetDestination(target);

        // Rotate smoothly towards movement
        Vector3 direction = (target - enemy.transform.position).normalized;
        if (direction != Vector3.zero)
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 5f
            );
    }



    public void Exit()
    {
        enemy.agent.isStopped = true;
    }

    private void PickRandomOffset()
    {
        // Only dodge on X and Z (horizontal plane)
        randomOffset = new Vector3(
            Random.Range(-offsetRange, offsetRange),
            0f,
            Random.Range(-offsetRange, offsetRange)
        );
    }

}
