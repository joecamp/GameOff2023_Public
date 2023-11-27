using FM;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    Free,
    Skill,
    Paused
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraRig m_cameraRig;
    [SerializeField] private WorldMouseRaycaster m_worldMouseRaycaster;
    [SerializeField] private GrassSpawner m_grassSpawner;
    [SerializeField] private ObjectInfoPanel m_objectInfoPanel;
    [SerializeField] private AudioPlayer m_clickAudioPlayer;
    [SerializeField] private AudioSource m_onLevelCompleteAudioSource;
    [SerializeField] private AudioSource m_gameMusicAudioSource;
    [SerializeField] private MenuDialogPanel m_menuDialogPanel;
    [SerializeField] private string m_nextLevelName;

    public GameState GameState = GameState.Free;

    public bool IsGoalComplete = false;

    private ClickableObject m_selectedObject;
    private Entity m_selectedEntity;

    public UnityAction OnLevelComplete;

    private void OnEnable()
    {
        ClickableObject.OnClickObject += OnClickObject;
        SkillsManager.OnActiveSkillChanged += OnActiveSkillChanged;
    }

    private void OnDisable()
    {
        ClickableObject.OnClickObject -= OnClickObject;
        SkillsManager.OnActiveSkillChanged -= OnActiveSkillChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_menuDialogPanel.ToggleActive(true);
            ToggleGamePaused(true);
        }

        if (GameState == GameState.Free)
        {
            UpdateFreeState();
        }

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift))
        {
            OnGoalComplete();
        }
    }

    public void OnGoalComplete()
    {
        if(IsGoalComplete)
        {
            return;
        }
        else
        {
            IsGoalComplete = true;
            OnLevelComplete?.Invoke();
            m_onLevelCompleteAudioSource.Play();
            Debug.Log("Goal Complete!");
        }
    }

    private void UpdateFreeState()
    {
        /*float xAxisInput = Input.GetAxisRaw("Camera X");
        float yAxisInput = Input.GetAxisRaw("Camera Y");

        if (xAxisInput != 0 || yAxisInput != 0)
        {
            ClearClickedObject();
        }*/

        if(m_cameraRig.IsMoving)
        {
            ClearClickedObject();
        }
    }

    private void OnActiveSkillChanged(int skillIndex)
    {
        if (skillIndex == -1)
        {
            GameState = GameState.Free;
        }
        else
        {
            ClearClickedObject();
            m_cameraRig.StopFollowingTarget();

            GameState = GameState.Skill;
        }
    }

    public void ToggleMusic(bool musicEnabled)
    {
        if (musicEnabled)
        {
            m_gameMusicAudioSource.Play();
        }
        else
        {
            m_gameMusicAudioSource.Pause();
        }
    }

    private void OnClickObject(ClickableObject clickableObject)
    {
        m_clickAudioPlayer.Play();

        if (m_selectedObject != null)
        {
            m_selectedObject.HighlightEffect.SetHighlighted(false);
        }
        if (m_selectedEntity != null)
        {
            m_selectedEntity.ToggleVisionRadiusIndicator(false);
        }

        m_selectedObject = clickableObject;
        Debug.Log("Clicked on " + clickableObject.name);

        m_selectedObject.HighlightEffect.SetHighlighted(true);
        m_objectInfoPanel.OnClickedObject(clickableObject);
        m_cameraRig.FollowTarget(clickableObject.transform);

        Entity entity = m_selectedObject.GetComponent<Entity>();
        if (entity)
        {
            m_selectedEntity = entity;
            m_selectedEntity.ToggleVisionRadiusIndicator(true);
        }
    }

    private void ClearClickedObject()
    {
        if (m_selectedObject != null)
        {
            m_selectedObject.HighlightEffect.SetHighlighted(false);
        }
        if (m_selectedEntity != null)
        {
            m_selectedEntity.ToggleVisionRadiusIndicator(false);
        }

        m_objectInfoPanel.Clear();
    }

    GameState m_prevState;
    public void ToggleGamePaused(bool pause)
    {
        if (pause)
        {
            m_prevState = GameState;
            Time.timeScale = 0f;
            GameState = GameState.Paused;
        }
        else
        {
            Time.timeScale = 1f;

            if(m_prevState == GameState.Paused) 
            {
                GameState = GameState.Free;
            }
            else
            {
                GameState = m_prevState;
            }
        }
    }

    public void LoadNextLevel()
    {
        SceneLoader.LoadScene(m_nextLevelName);
    }

    public void ReloadCurrentScene()
    {
        int currentSceneIdx = SceneManager.GetActiveScene().buildIndex;
        //SceneManager.LoadScene(currentSceneIdx);

        SceneLoader.LoadScene(currentSceneIdx);
    }
}