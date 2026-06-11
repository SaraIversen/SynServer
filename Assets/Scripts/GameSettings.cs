using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private int frameRate = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
    }
}
