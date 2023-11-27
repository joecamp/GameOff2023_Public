using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class FinishGamePanel : MonoBehaviour
{
    [SerializeField] private Button m_confirmButton;
    [SerializeField] private float m_delay;

    private GameManager m_gameManager;
    private CanvasGroup m_canvasGroup;

    private void Awake()
    {
        m_gameManager = FindObjectOfType<GameManager>();
        m_canvasGroup = GetComponent<CanvasGroup>();

        ToggleActive(false);
    }

    private void OnEnable()
    {
        m_confirmButton.onClick.AddListener(() => ToggleActive(false));
        m_gameManager.OnLevelComplete += LevelComplete;
    }

    private void OnDisable()
    {
        m_confirmButton.onClick.RemoveAllListeners();
        m_gameManager.OnLevelComplete -= LevelComplete;
    }

    public void ToggleActive(bool active)
    {
        m_gameManager.ToggleGamePaused(active);

        m_canvasGroup.alpha = active ? 1 : 0;
        m_canvasGroup.interactable = active;
        m_canvasGroup.blocksRaycasts = active;
    }

    private void LevelComplete()
    {
        StartCoroutine(LevelCompleteCoroutine());
    }

    private IEnumerator LevelCompleteCoroutine()
    {
        yield return new WaitForSeconds(m_delay);

        ToggleActive(true);
    }
}