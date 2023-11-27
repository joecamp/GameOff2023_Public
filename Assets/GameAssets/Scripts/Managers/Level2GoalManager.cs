using UnityEngine;

public class Level2GoalManager : MonoBehaviour
{
    private GameManager m_gameManager;
    private EntityManager m_entityManager;

    private void Awake()
    {
        m_gameManager = FindObjectOfType<GameManager>();
        m_entityManager = FindObjectOfType<EntityManager>();
    }

    private void OnEnable()
    {
        m_entityManager.OnSheepCountChanged += CheckIfGoalComplete;
        m_entityManager.OnWolfCountChanged += CheckIfGoalComplete;
    }

    private void OnDisable()
    {
        m_entityManager.OnSheepCountChanged -= CheckIfGoalComplete;
        m_entityManager.OnWolfCountChanged -= CheckIfGoalComplete;
    }

    private void CheckIfGoalComplete(int entityCount)
    {
        if (m_entityManager.SheepCount >= 50)
        {
            m_gameManager.OnGoalComplete();
        }
    }
}