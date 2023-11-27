using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class SheepTrackerPanel : MonoBehaviour
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
        m_entityManager.OnSheepCountChanged += TrackerUpdate;
    }

    private void OnDisable()
    {
        m_entityManager.OnSheepCountChanged -= TrackerUpdate;
    }

    private void TrackerUpdate(int sheepCount)
    {
        float ratio = Mathf.Clamp01((float)sheepCount / (float)m_entityManager.MaxSheepAllowed);
        sheepCount = Mathf.Clamp(sheepCount, 0, m_entityManager.MaxSheepAllowed);

        m_tmp.text = sheepCount + "/" + m_entityManager.MaxSheepAllowed;

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