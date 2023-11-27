using FM;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadMenuButton : MonoBehaviour
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        m_button.onClick.AddListener(() => { LoadMenu(); });
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveAllListeners();
    }

    private void LoadMenu()
    {
        SceneLoader.LoadScene("Title Menu");
    }
}