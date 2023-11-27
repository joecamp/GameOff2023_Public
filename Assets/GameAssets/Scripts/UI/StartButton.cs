using FM;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour
{
    private Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        m_button.onClick.AddListener(() => { StartGame(); });
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveAllListeners();
    }

    private void StartGame()
    {
        //SceneManager.LoadScene("Sandbox");
        SceneLoader.LoadScene("Level1");
    }
}