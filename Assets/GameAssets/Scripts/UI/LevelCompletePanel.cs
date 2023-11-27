using DG.Tweening;

using System.Collections.Generic;

using UnityEngine;

public class LevelCompletePanel : MonoBehaviour
{
    [SerializeField] private List<DOTweenAnimation> m_anims;

    private GameManager m_gameManager;
    private void Awake()
    {
        m_gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        m_gameManager.OnLevelComplete += PlayAnims;
    }

    private void OnDisable()
    {
        m_gameManager.OnLevelComplete -= PlayAnims;
    }

    private void PlayAnims()
    {
        foreach(DOTweenAnimation anim in m_anims)
        {
            anim.DOPlay();
        }
    }
}