using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class SkillPlacementIndicator : MonoBehaviour
{
    [SerializeField] private GameObject validObject;
    [SerializeField] private GameObject invalidObject;

    private void Awake()
    {
        validObject.SetActive(false);
        invalidObject.SetActive(false);
    }

    public void Hide()
    {
        validObject.SetActive(false);
        invalidObject.SetActive(false);
    }

    public void SetIsValid(bool isValidPlacement)
    {
        if (isValidPlacement)
        {
            validObject.SetActive(true);
            invalidObject.SetActive(false);
        }
        else
        {
            validObject.SetActive(false);
            invalidObject.SetActive(true);
        }
    }
}