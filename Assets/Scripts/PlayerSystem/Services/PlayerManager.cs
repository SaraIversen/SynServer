using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPositions;

    private int _lastSpawnIndex = -1;


    private void Awake()
    {
        Singleton.Initialize(ref Instance, this);
    }

    public Player SpawnPlayer(int id, string username)
    {
        Transform spawnPos = GetRandomSpawnPosition();

        Player newPlayer = Instantiate(
            _playerPrefab,
            spawnPos.transform.position,
            spawnPos.transform.rotation
        ).GetComponent<Player>();

        if (newPlayer != null)
        {
            newPlayer.Initialize(id, username);
        }
        return newPlayer;
    }

    public Transform GetRandomSpawnPosition()
    {
        int randomIndex = _lastSpawnIndex;

        while (randomIndex == _lastSpawnIndex)
        {
            randomIndex = Random.Range(0, _spawnPositions.Length);
        }

        _lastSpawnIndex = randomIndex;

        return _spawnPositions[randomIndex];
    }
}
