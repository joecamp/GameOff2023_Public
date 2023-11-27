using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CameraZoomButton : MonoBehaviour
{
    [SerializeField] private bool m_shouldZoomIn = true;

    private Button m_button;
    private CameraRig m_cameraRig;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_cameraRig = FindObjectOfType<CameraRig>();
    }

    private void OnEnable()
    {
        m_button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        if(m_shouldZoomIn)
        {
            m_cameraRig.ZoomInButton();
        }
        else
        {
            m_cameraRig.ZoomOutButton();
        }
    }
}