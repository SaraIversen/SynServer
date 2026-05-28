using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Id;
    public string Username;

    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _shootOrigin;

    private float _gravity = -9.81f;
    private float _moveSpeed = 5f;
    private float _jumpSpeed = 5f;

    public float CurrentHealth;
    public float MaxHealth = 100f;

    public int ItemAmount = 0;
    private int _maxItemAmount = 10;

    private float _throwForce = 1200f;

    private bool[] _inputs;
    private float yVelocity = 0;


    public bool IsDead => CurrentHealth <= 0;


    public void Initialize(int id, string username)
    {
        Id = id;
        Username = username;

        CurrentHealth = MaxHealth;

        _inputs = new bool[5];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        if (CurrentHealth <= 0f) return;

        Vector2 _inputDirection = Vector2.zero;
        if (_inputs[0]) _inputDirection.y += 1;
        if (_inputs[1]) _inputDirection.y -= 1;
        if (_inputs[2]) _inputDirection.x -= 1;
        if (_inputs[3]) _inputDirection.x += 1;

        Move(_inputDirection);
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 inputDirection)
    {
        Vector3 moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
        moveDirection *= _moveSpeed * Time.fixedDeltaTime;

        if (_controller.isGrounded)
        {
            yVelocity = 0f;
            if (_inputs[4])
            {
                yVelocity = _jumpSpeed * Time.fixedDeltaTime;
            }
        }
        yVelocity += _gravity * (Time.fixedDeltaTime * Time.fixedDeltaTime);

        moveDirection.y = yVelocity;
        _controller.Move(moveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] inputs, Quaternion rotation)
    {
        _inputs = inputs;
        transform.rotation = rotation;
    }

    public void Shoot(Vector3 viewDirection)
    {
        if (IsDead) return;

        ProjectileManager.Instance.SpawnProjectile(_shootOrigin).Initialize(viewDirection, _throwForce, Id);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHealth -= damage;
        if (IsDead)
        {
            CurrentHealth = 0f;
            _controller.enabled = false;
            //ServerSend.PlayerKilled(this);    
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        CurrentHealth = MaxHealth;
        Transform newSpawnPos = PlayerManager.Instance.GetRandomSpawnPosition();
        transform.position = newSpawnPos.transform.position;
        transform.rotation = newSpawnPos.transform.rotation;
        _controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem()
    {
        if (ItemAmount >= _maxItemAmount)
        {
            return false;
        }

        ItemAmount++;
        return true;
    }
}
