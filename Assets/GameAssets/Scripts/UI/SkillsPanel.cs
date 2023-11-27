using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SkillsPanel : MonoBehaviour
{
    [SerializeField] private List<SkillButton> m_skillButtons;

    private SkillButton m_activeSkillButton;
    private SkillsManager m_skillsManager;

    private void Awake()
    {
        m_skillsManager = FindObjectOfType<SkillsManager>();
    }

    private void OnEnable()
    {
        SkillsManager.OnSkillUsed += ClearActiveSkillButton;

        foreach (SkillButton button in m_skillButtons)
        {
            button.OnClick += SetActiveSkillButton;
        }
    }

    private void OnDisable()
    {
        foreach (SkillButton button in m_skillButtons)
        {
            button.OnClick -= SetActiveSkillButton;
        }
    }

    [Button]
    public void SetActiveSkillButton(SkillButton button, int skillIndex)
    {
        if (m_activeSkillButton == button)
        {
            ClearActiveSkillButton(0, 0);
            return;
        }

        button.ToggleButtonSelected(true);

        if (m_activeSkillButton != null)
        {
            m_activeSkillButton.ToggleButtonSelected(false);
        }

        m_activeSkillButton = button;

        m_skillsManager.SetActiveSkill(skillIndex);
    }

    public void ClearActiveSkillButton(int skillIndex, float cooldown)
    {
        foreach (SkillButton button in m_skillButtons)
        {
            button.ToggleButtonSelected(false);
        }

        m_activeSkillButton = null;

        m_skillsManager.SetActiveSkill(-1);
    }
}