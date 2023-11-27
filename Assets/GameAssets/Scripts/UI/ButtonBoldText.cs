using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonBoldText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI m_tmp;

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_tmp.fontStyle = FontStyles.Bold;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_tmp.fontStyle = FontStyles.Normal;
    }
}