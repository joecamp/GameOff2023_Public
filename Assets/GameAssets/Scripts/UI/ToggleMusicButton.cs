using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(CanvasGroup))]
public class ToggleMusicButton : MonoBehaviour
{
    private Button m_button;
    private CanvasGroup m_canvasGroup;
    private GameMusicManager m_gameMusicManager;
    private bool m_enabled = true;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        m_gameMusicManager = FindObjectOfType<GameMusicManager>();

        if(!m_gameMusicManager.IsPlaying)
        {
            m_enabled = false;

            m_canvasGroup.alpha = .45f;
        }
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
        m_enabled = !m_enabled;

        m_gameMusicManager.ToggleMusic(m_enabled);

        m_canvasGroup.alpha = m_enabled ? 1f : .45f;
    }
}