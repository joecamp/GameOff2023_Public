using DG.Tweening;
using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(CanvasGroup))]
public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int m_skillIndex;
    [SerializeField] private Image m_borderImage;
    [SerializeField] private CanvasGroup m_hoverPanelCanvasGroup;
    [SerializeField] private SkillCooldownImage m_cooldownImage;
    [SerializeField] private Color m_normalColor;
    [SerializeField] private Color m_activatedColor;

    public UnityAction<SkillButton, int> OnClick;

    private Button m_button;
    private CanvasGroup m_canvasGroup;

    private bool m_isSelected = false;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        m_button.onClick.AddListener(() => OnClick?.Invoke(this, m_skillIndex));
        SkillsManager.OnSkillUsed += OnAnySkillUse;
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveAllListeners();
        SkillsManager.OnSkillUsed -= OnAnySkillUse;
    }

    [Button]
    public void ToggleButtonSelected(bool isSelected)
    {
        m_borderImage.color = isSelected ? m_activatedColor : m_normalColor;

        m_isSelected = isSelected;
    }

    private void SetButtonOnCooldown()
    {
        m_button.interactable = false;
        m_canvasGroup.alpha = .7f;
    }

    private void SetButtonOffCooldown()
    {
        m_button.interactable = true;
        m_canvasGroup.alpha = 1f;
    }

    private void OnAnySkillUse(int skillIndex, float skillCooldown)
    {
        // Check if this is our skill index
        if (m_skillIndex == skillIndex)
        {
            m_cooldownImage.PlayCooldownAnimation(skillCooldown);

            SetButtonOnCooldown();

            Invoke("SetButtonOffCooldown", skillCooldown);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_hoverPanelCanvasGroup.DOFade(1f, .25f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_hoverPanelCanvasGroup.DOFade(0f, .25f);
    }
}