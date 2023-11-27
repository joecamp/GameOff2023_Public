using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class WolfTrackerPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_tmp;
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private Color m_highPopColor;
    [SerializeField] private Color m_lowPopColor;

    [SerializeField] private Color m_textNormalColor;
    [SerializeField] private Color m_textHighColor;
    [SerializeField] private Color m_textLowColor;

    private EntityManager m_entityManager;

    private void Awake()
    {
        m_entityManager = FindObjectOfType<EntityManager>();
    }

    private void OnEnable()
    {
        m_entityManager.OnWolfCountChanged += TrackerUpdate;
    }

    private void OnDisable()
    {
        m_entityManager.OnWolfCountChanged -= TrackerUpdate;
    }

    private void TrackerUpdate(int wolfCount)
    {
        float ratio = Mathf.Clamp01((float)wolfCount / (float)m_entityManager.MaxWolvesAllowed);
        wolfCount = Mathf.Clamp(wolfCount, 0, m_entityManager.MaxWolvesAllowed);

        m_tmp.text = wolfCount + "/" + m_entityManager.MaxWolvesAllowed;

        m_tmp.color = m_textNormalColor;
        if (ratio == 0f)
        {
            m_tmp.color = m_textLowColor;
        }
        else if (ratio == 1f)
        {
            m_tmp.color = m_textHighColor;
        }

        m_backgroundImage.color = Color.Lerp(m_lowPopColor, m_highPopColor, ratio);
    }
}