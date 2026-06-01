using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int Id { get; private set; }

    #region SerializeFields
    [SerializeField] private CharacterController _characterController;

    [Space(10)]

    [SerializeField] private float _wanderRadiusFromSpawnPoint = 5;
    [SerializeField] private float _patrolSpeed = 2f;
    [SerializeField] private float _minIdleTime = 1;
    [SerializeField] private float _maxIdleTime = 5;
    #endregion

    #region Properties
    public CharacterController CharacterController { get { return _characterController; } }

    public IEnemyState CurrentState { get; private set; }
    public IdleState IdleState { get; private set; }
    public PatrolState PatrolState { get; private set; }

    public Vector3 SpawnPosition { get; private set; }
    public float IdleTimer { get; set; }

    public float WanderRadiusFromSpawnPoint { get { return _wanderRadiusFromSpawnPoint; } }
    public float PatrolSpeed { get { return _patrolSpeed; } }
    public float MinIdleTime { get { return _minIdleTime; } }
    public float MaxIdleTime { get { return _maxIdleTime; } }

    public float Gravity { get; } = -9.81f;
    public float YVelocity { get; set; }


    public Vector3 TargetPosition { get; set; }


    public float CurrentHealth;
    public float MaxHealth = 100f;
    public bool IsDead => CurrentHealth <= 0;

    public EnemySpawner EnemySpawner { get; private set; }
    public bool IsSpawned { get; set; }
    #endregion


    #region Methods

    public void Initialize(int id, EnemySpawner enemySpawner)
    {
        Id = id;
        EnemySpawner = enemySpawner;
        CurrentHealth = MaxHealth;
    }

    private void Awake()
    {
        IdleState = new IdleState();
        PatrolState = new PatrolState();
    }

    private void Start()
    {
        SpawnPosition = EnemySpawner.transform.position;

        ChangeState(IdleState);
    }

    private void FixedUpdate()
    {
        CurrentState?.Update(this);
    }

    public void ChangeState(IEnemyState newState)
    {
        CurrentState?.Exit(this);
        CurrentState = newState;
        CurrentState.Enter(this);
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (IsDead)
        {
            CurrentHealth = 0f;
            EnemySpawner.RespawnEnemy(this);
        }

        ServerSend.EnemyHealth(this);
    }
    #endregion
}