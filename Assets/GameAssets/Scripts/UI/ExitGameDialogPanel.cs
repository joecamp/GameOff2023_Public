using UnityEngine;
using UnityEngine.UI;

public class ExitGameDialogPanel : MonoBehaviour
{
    [SerializeField] private Button m_yesButton;
    [SerializeField] private Button m_noButton;
    private CanvasGroup m_canvasGroup;

    private void Awake()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();

        ToggleActive(false);
    }

    private void OnEnable()
    {
        m_yesButton.onClick.AddListener(() => { ExitGame(); });
        m_noButton.onClick.AddListener(() => ToggleActive(false));
    }

    private void OnDisable()
    {
        m_yesButton.onClick.RemoveAllListeners();
        m_noButton.onClick.RemoveAllListeners();
    }

    public void ToggleActive(bool active)
    {
        m_canvasGroup.alpha = active ? 1 : 0;
        m_canvasGroup.interactable = active;
        m_canvasGroup.blocksRaycasts = active;
    }

    public void ExitGame()
    {
#if !UNITY_WEBGL
        Application.Quit();
#endif
        ToggleActive(false);
    }
}