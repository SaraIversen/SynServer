using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyToSpawn;
    [SerializeField] private float _respawnTime;


    private void Start()
    {
        EnemyManager.Instance.SpawnEnemy(_enemyToSpawn, this.transform.position, this);
    }

    public void RespawnEnemy(Enemy enemy)
    {
        StartCoroutine(Respawn(enemy));
    }

    private IEnumerator Respawn(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);

        enemy.CurrentHealth = enemy.MaxHealth;

        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;

        enemy.gameObject.SetActive(true);

        ServerSend.EnemyRespawned(enemy);
    }
}
