using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExitButton : MonoBehaviour
{
    [SerializeField] private ExitGameDialogPanel m_dialogPanel;

    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        m_button.onClick.AddListener(() => { OnClick(); });
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveAllListeners();
    }

    private void OnClick()
    {
        m_dialogPanel.ToggleActive(true);
    }
}