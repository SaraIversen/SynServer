using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int Id;

    [SerializeField] private Rigidbody _rigidBody;

    private bool _exploded;
    private int _thrownByPlayer;
    private Vector3 _initialMovementDirection;
    private Vector3 _initialForce;

    [SerializeField] private float _explosionRadius = 1.5f;
    [SerializeField] private float _explosionDamage = 25f;

    public void Initialize(Vector3 initialMovementDirection, float initialForceStrength, int thrownByPlayer)
    {
        _initialMovementDirection = initialMovementDirection;
        _initialForce = initialMovementDirection * initialForceStrength;
        _thrownByPlayer = thrownByPlayer;
    }

    private void Start()
    {
        ServerSend.SpawnProjectile(this, _initialMovementDirection, _thrownByPlayer);

        _rigidBody.AddForce(_initialForce);
        StartCoroutine(ExplodeAfterTime());
    }

    private void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Explode();
    }

    private void Explode()
    {
        if (_exploded) return;
        _exploded = true;
        ServerSend.ProjectileExploded(this);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<Player>().TakeDamage(_explosionDamage);
            }
            else if (collider.CompareTag("Enemy"))
            {
                collider.GetComponent<Enemy>().TakeDamage(_explosionDamage);
            }
        }

        ProjectileManager.Instance.DestroyProjectile(Id);
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(10f);

        Explode();
    }
}
