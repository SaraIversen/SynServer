using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;

    private static Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
    private static int _nextProjectileId = 1;

    [SerializeField] private GameObject _projectilePrefab;

    private void Awake()
    {
        Singleton.Initialize(ref Instance, this);
    }

    public Projectile SpawnProjectile(Transform shootOrigin)
    {
        int projectileId = _nextProjectileId++;

        Projectile projectile = Instantiate(_projectilePrefab, shootOrigin.position + shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
        projectile.Id = projectileId;

        _projectiles.Add(projectileId, projectile);

        return projectile;
    }

    public void DestroyProjectile(int projectileId)
    {
        if (!_projectiles.TryGetValue(projectileId, out Projectile projectile))
        {
            Debug.Log("Could not find projectile to destroy!");
            return;
        }

        _projectiles.Remove(projectileId);
        Destroy(projectile.gameObject);
    }
}
