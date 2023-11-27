using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class FootstepAudioController : MonoBehaviour
{
    private AudioPlayer m_audioPlayer;

    private void Awake()
    {
        m_audioPlayer = GetComponent<AudioPlayer>();
    }

    private void Footstep()
    {
        m_audioPlayer.Play();
    }
}