using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FruitTreeInfoPanel : MonoBehaviour
{
    [SerializeField] private Slider m_spawnSlider;
    [SerializeField] private Slider m_lifeSlider;

    private CanvasGroup m_canvasGroup;
    private FruitTree m_selectedTree;

    private void Awake()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (m_selectedTree != null)
        {
            m_spawnSlider.value = m_selectedTree.GetTimeToNextSpawnPercent();
            m_lifeSlider.value = m_selectedTree.GetTreeLifePercent();
        }
    }

    public void SetTree(FruitTree tree)
    {
        m_selectedTree = tree;

        m_selectedTree.OnDeath += Clear;

        TogglePanelVisibility(true);
    }

    public void Clear()
    {
        if (m_selectedTree != null)
        {
            m_selectedTree.OnDeath -= Clear;

            m_selectedTree = null;
        }

        TogglePanelVisibility(false);
    }

    public void TogglePanelVisibility(bool visible)
    {
        m_canvasGroup.alpha = visible ? 1f : 0f;
    }
}