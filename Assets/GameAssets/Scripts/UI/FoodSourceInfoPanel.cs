using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;

public class FoodSourceInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_nameTmp;
    [SerializeField] private TextMeshProUGUI m_valueTmp;

    private CanvasGroup m_canvasGroup;
    private FoodSource m_selectedFoodSource;

    private void Awake()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetFoodSource(FoodSource foodSource)
    {
        m_selectedFoodSource = foodSource;

        m_nameTmp.text = m_selectedFoodSource.GetDisplayName();
        m_valueTmp.text = "Value: " + m_selectedFoodSource.GetFoodValue().ToString();

        m_selectedFoodSource.OnDestroy += Clear;

        TogglePanelVisibility(true);
    }

    public void Clear()
    {
        if (m_selectedFoodSource != null)
        {
            m_selectedFoodSource.OnDestroy -= Clear;

            m_selectedFoodSource = null;
        }

        TogglePanelVisibility(false);
    }

    public void TogglePanelVisibility(bool visible)
    {
        m_canvasGroup.alpha = visible ? 1f : 0f;
    }
}