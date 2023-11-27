using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameMusicManager : MonoBehaviour
{
    private AudioSource m_audioSource;
    public bool IsPlaying { get; private set; } = true;

    private void Awake()
    {
        if(FindObjectsOfType<GameMusicManager>().Length > 1)
        {
            Destroy(gameObject);
        }

        m_audioSource = GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Title Menu")
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMusic(bool musicEnabled)
    {
        IsPlaying = musicEnabled;

        if (musicEnabled)
        {
            m_audioSource.Play();
        }
        else
        {
            m_audioSource.Pause();
        }
    }
}