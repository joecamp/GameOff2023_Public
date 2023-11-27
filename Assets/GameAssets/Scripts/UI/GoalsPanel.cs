using UnityEngine;

public class GoalsPanel : MonoBehaviour
{
    private GameManager m_gameManager;

    private void Awake()
    {
        m_gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        m_gameManager.OnLevelComplete += Disable;
    }

    private void OnDisable()
    {
        m_gameManager.OnLevelComplete -= Disable;
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
