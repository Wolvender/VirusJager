using UnityEngine;
using UnityEngine.AI;


public interface IEnemyState
{
    void Enter();
    void Execute();
    void Exit();
}


public class EnemnyFSM : MonoBehaviour
{
    [Header("Flow Path")]
    public Transform[] flowPath; // Assign transforms along the blood vessel in the inspector

    [HideInInspector] public IEnemyState currentState;

    // References for states
    [HideInInspector] public EnemyFlowState flowState;
    [HideInInspector] public EnemyDieState dieState;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerHealthManager playerHealth;
    [HideInInspector] public Animator animator;

    // Enemy stats
    public float enemyHealth = 100;

    void Start()
    {

        // Cache references
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealthManager>();

        // Initialize states
        flowState = new EnemyFlowState(this);
        dieState = new EnemyDieState(this);

        // Start in Roaming
        ChangeState(flowState);
    }

    void Update()
    {
        currentState.Execute();
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // Called when enemy takes damage
    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0)
        {
            ChangeState(dieState);
        }
    }
}
