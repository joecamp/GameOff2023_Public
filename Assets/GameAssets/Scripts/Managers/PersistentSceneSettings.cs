using UnityEngine;

public class PersistentSceneSettings : MonoBehaviour
{
    public bool IsGameMusicOn;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}