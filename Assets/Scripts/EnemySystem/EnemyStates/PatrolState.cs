using UnityEngine;

public class PatrolState : IEnemyState
{
    public void Enter(Enemy enemy)
    {
        enemy.TargetPosition = GetRandomPatrolPosition(enemy);
        RotateTowardsTargetPosition(enemy, enemy.TargetPosition);
    }

    public void Update(Enemy enemy)
    {
        if (MoveTowardsPosition(enemy, enemy.TargetPosition))
        {
            enemy.ChangeState(enemy.IdleState);
        }
    }

    public void Exit(Enemy enemy)
    {
        // Optional cleanup
    }

    #region Helper Methods
    private Vector3 GetRandomPatrolPosition(Enemy enemy)
    {
        Vector2 randomPointInCircle = Random.insideUnitCircle * enemy.WanderRadiusFromSpawnPoint;
        Vector3 newPos = new Vector3(
            enemy.SpawnPosition.x + randomPointInCircle.x,
            enemy.transform.position.y,
            enemy.SpawnPosition.z + randomPointInCircle.y
        );
        //newPos.y = Terrain.activeTerrain.SampleHeight(newPos); // Only needed if we use terrain.
        return newPos;
    }

    private void RotateTowardsTargetPosition(Enemy enemy, Vector3 targetPos)
    {
        enemy.transform.LookAt(new Vector3(targetPos.x, enemy.transform.position.y, targetPos.z), enemy.transform.up);
        ServerSend.EnemyRotation(enemy);
    }

    private bool MoveTowardsPosition(Enemy enemy, Vector3 targetPos)
    {
        Vector3 _movement = enemy.transform.forward * enemy.PatrolSpeed * Time.fixedDeltaTime;

        if (enemy.CharacterController.isGrounded)
        {
            enemy.YVelocity = 0f;
        }
        enemy.YVelocity += enemy.Gravity * (Time.fixedDeltaTime * Time.fixedDeltaTime);

        _movement.y = enemy.YVelocity;
        enemy.CharacterController.Move(_movement);

        ServerSend.EnemyPosition(enemy);

        if (Vector2.Distance(new Vector2(enemy.transform.position.x, enemy.transform.position.z), new Vector2(targetPos.x, targetPos.z)) < 0.1f)
        {
            return true;
        }

        return false;
    }
    #endregion
}