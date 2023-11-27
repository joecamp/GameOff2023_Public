using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class HowToPlayPanel : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels;

    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private CanvasGroup previousImageCanvasGroup;
    [SerializeField] private CanvasGroup nextImageCanvasGroup;

    public int m_panelIndex = 0;

    private void OnEnable()
    {
        previousButton.onClick.AddListener(OnPreviousButtonPressed);
        nextButton.onClick.AddListener(OnNextButtonPressed);
    }

    private void OnDisable()
    {
        previousButton.onClick.RemoveListener(OnPreviousButtonPressed);
        nextButton.onClick.RemoveListener(OnNextButtonPressed);
    }

    private void OnPreviousButtonPressed()
    {
        SetActivePanel(m_panelIndex - 1);
    }

    private void OnNextButtonPressed()
    {
        SetActivePanel(m_panelIndex + 1);
    }

    private void SetActivePanel(int index)
    {
        index = Mathf.Clamp(index, 0, panels.Count - 1);
        if(m_panelIndex == index)
        {
            return;
        }

        panels[m_panelIndex].SetActive(false);
        m_panelIndex = index;
        panels[m_panelIndex].SetActive(true);

        if(m_panelIndex == 0)
        {
            previousImageCanvasGroup.alpha = .15f;
        }
        else if(m_panelIndex == panels.Count - 1)
        {
            nextImageCanvasGroup.alpha = .15f;
        }
        else
        {
            nextImageCanvasGroup.alpha = 1f;
            previousImageCanvasGroup.alpha = 1f;
        }
    }
}