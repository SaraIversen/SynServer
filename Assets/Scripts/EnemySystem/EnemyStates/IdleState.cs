using UnityEngine;

public class IdleState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.IdleTimer = GetRandomDuration(enemy.MinIdleTime, enemy.MaxIdleTime);
    }

    public void Update(Enemy enemy)
    {
        enemy.IdleTimer -= Time.fixedDeltaTime;

        if (enemy.IdleTimer <= 0f)
        {
            enemy.ChangeState(enemy.PatrolState);
        }
    }

    public void Exit(Enemy enemy)
    {
        // Nothing for now...
    }

    #region Helper Methods
    private float GetRandomDuration(float min, float max)
    {
        return Random.Range(min, max);
    }
    #endregion
}
