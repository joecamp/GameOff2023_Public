using DG.Tweening;
using Sirenix.OdinInspector;

using UnityEngine;

public class HoverObject : MonoBehaviour
{
    [SerializeField] private GameObject m_model;
    [SerializeField] private Vector3 m_positionOffset;

    private bool m_enabled = true;
    private Transform m_hoveredObjectTransform = null;
    private Tween m_currentTween = null;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        ClickableObject.OnHoverEnterObject += OnHoverEnterObject;
        ClickableObject.OnHoverExitObject += OnHoverExitObject;
    }

    private void OnDisable()
    {
        ClickableObject.OnHoverEnterObject -= OnHoverEnterObject;
        ClickableObject.OnHoverExitObject -= OnHoverExitObject;
    }

    private void Update()
    {
        if (m_enabled == false) return;

        if (m_hoveredObjectTransform)
        {
            transform.position = m_hoveredObjectTransform.position + m_positionOffset;
        }
    }

    [Button]
    public void ToggleEnabled(bool enabled)
    {
        if (enabled == m_enabled)
        {
            return;
        }

        m_enabled = enabled;

        if (m_enabled == false)
        {
            m_model.SetActive(false);
        }
        else
        {
            m_model.SetActive(true);
        }
    }

    private void OnHoverEnterObject(ClickableObject clickableObject)
    {
        if (m_enabled == false) return;

        m_hoveredObjectTransform = clickableObject.transform;
        transform.position = m_hoveredObjectTransform.position + m_positionOffset;

        if (m_currentTween != null)
        {
            m_currentTween.Complete();
        }

        m_currentTween = transform.DOScale(Vector3.one, .5f);
    }

    private void OnHoverExitObject(ClickableObject clickableObject)
    {
        if (m_enabled == false) return;

        m_hoveredObjectTransform = null;

        m_currentTween = transform.DOScale(Vector3.zero, .5f);
    }
}