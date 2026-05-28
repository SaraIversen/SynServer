using UnityEngine;

public enum PacketId
{
    welcome = 1,
    welcomeReceived,

    ping,
    pong,

    spawnPlayer,
    playerDisconnected,

    playerPosition,
    playerRotation,
    playerMovement,
    playerShoot,
    playerHealth,
    playerRespawned,

    spawnProjectile,
    projectilePosition,
    projectileExploded,

    spawnEnemy,
    enemyPosition,
    enemyRotation,
    enemyHealth,
    enemyRespawned,
}
