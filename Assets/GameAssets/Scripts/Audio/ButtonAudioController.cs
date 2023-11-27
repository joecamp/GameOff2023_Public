using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioPlayer), typeof(Button))]
public class ButtonAudioController : MonoBehaviour
{
    private AudioPlayer m_audioPlayer;
    private Button m_button;

    private void Awake()
    {
        m_audioPlayer = GetComponent<AudioPlayer>();
        m_button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        m_button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        m_audioPlayer.Play();
    }
}