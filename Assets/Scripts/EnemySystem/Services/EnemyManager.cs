using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private static Dictionary<int, Enemy> _enemies = new Dictionary<int, Enemy>();
    private static int _nextEnemyId = 1;


    private void Awake()
    {
        Singleton.Initialize(ref Instance, this);
    }

    public IEnumerable<Enemy> GetAllEnemies()
    {
        return _enemies.Values;
    }

    public Enemy SpawnEnemy(GameObject enemyPrefab, Vector3 spawnPosition, EnemySpawner enemySpawner)
    {
        int enemyId = _nextEnemyId++;

        Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();
        enemy.Initialize(enemyId, enemySpawner);
        enemy.IsSpawned = true;

        _enemies.Add(enemyId, enemy);

        ServerSend.SpawnEnemy(enemy);

        return enemy;
    }

    public void DestroyEnemy(int enemyId)
    {
        if (!_enemies.TryGetValue(enemyId, out Enemy enemy))
        {
            Debug.Log("Could not find enemy to destroy!");
            return;
        }

        _enemies.Remove(enemyId);
        Destroy(enemy.gameObject);
    }
}
