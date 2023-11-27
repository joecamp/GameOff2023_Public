using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(CanvasGroup))]
public class NextLevelButton : MonoBehaviour
{
    [SerializeField] private ContinueDialogPanel m_continueDialogPanel;

    private Button m_button;
    private CanvasGroup m_canvasGroup;
    private GameManager m_gameManager;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_gameManager = FindObjectOfType<GameManager>();

        m_canvasGroup.alpha = 0f;
        m_canvasGroup.interactable = false;
        m_canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        m_gameManager.OnLevelComplete += Enable;
        m_button.onClick.AddListener(() => OnClick());
    }

    private void OnDisable()
    {
        m_gameManager.OnLevelComplete -= Enable;
        m_button.onClick.RemoveAllListeners();
    }

    private void Enable()
    {
        m_canvasGroup.interactable = true;
        m_canvasGroup.blocksRaycasts = true;

        m_canvasGroup.DOFade(1f, 1f);
    }

    private void OnClick()
    {
        m_continueDialogPanel.ToggleActive(true);
    }
}