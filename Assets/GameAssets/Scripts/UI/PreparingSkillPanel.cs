using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PreparingSkillPanel : MonoBehaviour
{
    private CanvasGroup m_canvasGroup;

    private bool m_enabled;

    private void Awake()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_canvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        SkillsManager.OnActiveSkillChanged += OnSkillChanged;
    }

    private void OnDisable()
    {
        SkillsManager.OnActiveSkillChanged -= OnSkillChanged;
    }

    private void OnSkillChanged(int skillIndex)
    {
        if (skillIndex != -1 && m_enabled == false)
        {
            m_enabled = true;
            m_canvasGroup.DOFade(1f, .5f);
        }
        else if (skillIndex == -1)
        {
            m_enabled = false;
            m_canvasGroup.DOFade(0f, .5f);
        }
    }
}