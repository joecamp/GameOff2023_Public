using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SkillsManager : MonoBehaviour
{
    [SerializeField] private float m_skill1Cooldown;
    [SerializeField] private float m_skill2Cooldown;
    [SerializeField] private float m_skill3Cooldown;

    [SerializeField] private GameObject m_skill1Prefab;
    [SerializeField] private GameObject m_skill2Prefab;
    [SerializeField] private GameObject m_skill3Prefab;

    [SerializeField] private SkillsPanel m_skillsPanel;
    [SerializeField] private SkillPlacementManager m_skillPlacementManager;

    public float m_skill1CooldownTimer = 0f;
    public float m_skill2CooldownTimer = 0f;
    public float m_skill3CooldownTimer = 0f;

    public bool m_skill1OnCooldown = false;
    public bool m_skill2OnCooldown = false;
    public bool m_skill3OnCooldown = false;

    public int m_activeSkillIndex = -1;

    // <int, float> -> <skillIndex, cooldown>
    public static UnityAction<int> OnActiveSkillChanged;
    public static UnityAction<int, float> OnSkillUsed;

    private void Update()
    {
        UpdateCooldownTimers();

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            TryUseActiveSkill();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            SetActiveSkill(-1);
            m_skillsPanel.ClearActiveSkillButton(0, 0);
        }
    }

    private void UpdateCooldownTimers()
    {
        if (m_skill1OnCooldown)
        {
            m_skill1CooldownTimer += Time.deltaTime;
            if (m_skill1CooldownTimer >= m_skill1Cooldown)
            {
                m_skill1OnCooldown = false;
                m_skill1CooldownTimer = 0f;
            }
        }

        if (m_skill2OnCooldown)
        {
            m_skill2CooldownTimer += Time.deltaTime;
            if (m_skill2CooldownTimer >= m_skill2Cooldown)
            {
                m_skill2OnCooldown = false;
                m_skill2CooldownTimer = 0f;
            }
        }

        if (m_skill3OnCooldown)
        {
            m_skill3CooldownTimer += Time.deltaTime;
            if (m_skill3CooldownTimer >= m_skill3Cooldown)
            {
                m_skill3OnCooldown = false;
                m_skill3CooldownTimer = 0f;
            }
        }
    }

    public void SetActiveSkill(int skillIndex)
    {
        if (m_activeSkillIndex == skillIndex) return;

        switch (skillIndex)
        {
            case 1:
                if (m_skill1OnCooldown) return;
                break;
            case 2:
                if (m_skill2OnCooldown) return;
                break;
            case 3:
                if (m_skill3OnCooldown) return;
                break;
        }

        m_activeSkillIndex = skillIndex;

        OnActiveSkillChanged?.Invoke(skillIndex);
    }

    private void TryUseActiveSkill()
    {
        if (m_activeSkillIndex == -1) return;

        Vector3 skillPos;

        if (m_skillPlacementManager.GetActiveSkillPosition(out skillPos))
        {
            UseActiveSkill(skillPos);
        }
        else
        {
            return;
        }
    }

    public void UseActiveSkill(Vector3 position)
    {
        if (m_activeSkillIndex == -1) return;

        switch (m_activeSkillIndex)
        {
            case 1:
                if (m_skill1OnCooldown) return;

                m_skill1OnCooldown = true;
                UseSkill1(position);
                OnSkillUsed?.Invoke(1, m_skill1Cooldown);
                Debug.Log("Used Skill 1");
                m_activeSkillIndex = -1;

                break;
            case 2:
                if (m_skill2OnCooldown) return;

                m_skill2OnCooldown = true;
                UseSkill2(position);
                OnSkillUsed?.Invoke(2, m_skill2Cooldown);
                Debug.Log("Used Skill 2");
                m_activeSkillIndex = -1;

                break;
            case 3:
                if (m_skill3OnCooldown) return;

                m_skill3OnCooldown = true;
                UseSkill3(position);
                OnSkillUsed?.Invoke(3, m_skill3Cooldown);
                Debug.Log("Used Skill 3");
                m_activeSkillIndex = -1;

                break;
        }
    }

    private void UseSkill1(Vector3 position)
    {
        Instantiate(m_skill1Prefab, position, Quaternion.identity);
    }

    private void UseSkill2(Vector3 position)
    {
        Instantiate(m_skill2Prefab, position, Quaternion.identity);
    }

    private void UseSkill3(Vector3 position)
    {
        Instantiate(m_skill3Prefab, position, Quaternion.identity);
    }
}