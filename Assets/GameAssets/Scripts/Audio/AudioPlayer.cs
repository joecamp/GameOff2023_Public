using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> m_audioClips;
    [SerializeField] private float m_minPitch;
    [SerializeField] private float m_maxPitch;
    [SerializeField] private bool m_playOnAwake = false;
    [ShowIf("m_playOnAwake", true)][SerializeField] private bool m_destroyOnCompletion = false;
    [SerializeField] private bool m_fadeInOnAwake = false;

    private AudioSource m_audioSource;
    private float m_baseVolume;
    private bool m_initPlay = false;
    private float m_fadeInDuration = .5f;
    private float m_fadeOutDuration = 1.5f;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_baseVolume = m_audioSource.volume;

        if (m_playOnAwake)
        {
            if (m_destroyOnCompletion)
            {
                m_initPlay = true;
            }

            Play();
        }

        if(m_fadeInOnAwake)
        {
            FadeIn();
        }
    }

    private void Update()
    {
        if (m_initPlay && m_audioSource.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }

    public void Play()
    {
        if (m_audioClips.Count == 0)
        {
            Debug.LogError("No audio clips defined");
            return;
        }

        m_audioSource.pitch = Random.Range(m_minPitch, m_maxPitch);
        AudioClip randomClip = m_audioClips[Random.Range(0, m_audioClips.Count)];

        m_audioSource.PlayOneShot(randomClip);
    }

    public void TogglePause(bool value)
    {
        if (value)
        {
            m_audioSource.Pause();
        }
        else
        {
            m_audioSource.UnPause();
        }
    }

    public void ToggleMute(bool value)
    {
        m_audioSource.mute = value;
    }

    [Button]
    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeVolumeCoroutine(0f, m_baseVolume, m_fadeInDuration));
    }

    [Button]
    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeVolumeCoroutine(m_audioSource.volume, 0f, m_fadeOutDuration));
    }

    private IEnumerator FadeVolumeCoroutine(float startVolume, float endVolume, float duration)
    {
        m_audioSource.volume = startVolume;
        float counter = 0f;

        while(counter < duration)
        {
            counter += Time.deltaTime;

            m_audioSource.volume = Mathf.Lerp(startVolume, endVolume, counter / duration);

            yield return null;
        }

        m_audioSource.volume = endVolume;
    }
}