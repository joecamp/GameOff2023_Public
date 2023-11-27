using FM;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class MenuDialogPanel : MonoBehaviour
{
    [SerializeField] private Button m_yesButton;
    [SerializeField] private Button m_noButton;

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
        m_yesButton.onClick.AddListener(() => { m_gameManager.ToggleGamePaused(false); SceneLoader.LoadScene("Title Menu"); });
        m_noButton.onClick.AddListener(() => ToggleActive(false));
    }

    private void OnDisable()
    {
        m_yesButton.onClick.RemoveAllListeners();
        m_noButton.onClick.RemoveAllListeners();
    }

    public void ToggleActive(bool active)
    {
        m_gameManager.ToggleGamePaused(active);

        m_canvasGroup.alpha = active ? 1 : 0;
        m_canvasGroup.interactable = active;
        m_canvasGroup.blocksRaycasts = active;
    }
}
