using FM;

using UnityEngine;
using UnityEngine.UI;

public class HowToPlayButton : MonoBehaviour
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        m_button.onClick.AddListener(() => { LoadScene(); });
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveAllListeners();
    }

    private void LoadScene()
    {
        SceneLoader.LoadScene("HowToPlay");
    }
}