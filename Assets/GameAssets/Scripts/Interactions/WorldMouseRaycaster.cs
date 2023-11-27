using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

public class WorldMouseRaycaster : MonoBehaviour
{
    [SerializeField] private LayerMask m_layerMask = 0;
    private Camera m_mainCam;

    private bool m_isMouseDown;
    private IClickHandler m_initClickHandler;
    private IHoverHandler m_currentHoverHandler;

    private GameManager m_gameManager;
    private bool m_shouldUpdate = true;

    private void Awake()
    {
        m_mainCam = Camera.main;
        m_gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (m_gameManager.GameState == GameState.Skill && m_shouldUpdate == true)
        {
            m_shouldUpdate = false;
        }
        if (m_gameManager.GameState == GameState.Free && m_shouldUpdate == false)
        {
            StartCoroutine(ShouldUpdateDelayCoroutine());
        }

        if (m_shouldUpdate)
        {
            CheckRaycast();
        }
    }

    private void CheckRaycast()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (m_currentHoverHandler != null)
            {
                m_currentHoverHandler.OnHoverExit();
                m_currentHoverHandler = null;
            }

            return;
        }

        RaycastHit hitInfo;
        Ray ray = m_mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, m_layerMask))
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red);

            GameObject hitGO = hitInfo.collider.gameObject;

            IHoverHandler hoverHandler = hitGO.GetComponentInParent<IHoverHandler>();
            IClickHandler clickHandler = hitGO.GetComponentInParent<IClickHandler>();

            // We are over a IClickHandler
            if (clickHandler != null)
            {
                // Check if MouseDown this frame
                if (Input.GetMouseButtonDown(0))
                {
                    m_initClickHandler = clickHandler;
                }
                // Check if MouseUp this frame
                else if (Input.GetMouseButtonUp(0))
                {
                    if (clickHandler == m_initClickHandler)
                    {
                        clickHandler.OnClick();
                    }
                }
            }
            // We are over a IHoverHandler
            if (hoverHandler != null)
            {
                // Same IHoverHandler as last frame
                if (hoverHandler == m_currentHoverHandler)
                {
                    return;
                }
                // Different IHoverHandler from last frame
                else if (m_currentHoverHandler != null)
                {
                    m_currentHoverHandler.OnHoverExit();
                    m_currentHoverHandler = hoverHandler;
                    m_currentHoverHandler.OnHoverEnter();
                }
                // No IHoverHandler last frame
                else
                {
                    hoverHandler.OnHoverEnter();
                    m_currentHoverHandler = hoverHandler;
                }
            }
            // We aren't over a IHoverHandler
            else if (m_currentHoverHandler != null)
            {
                m_currentHoverHandler.OnHoverExit();
                m_currentHoverHandler = null;
            }
        }
        // We aren't over any MouseInputHandlers
        else if (m_currentHoverHandler != null)
        {
            m_currentHoverHandler.OnHoverExit();
            m_currentHoverHandler = null;
        }
    }

    // Without this coroutine on GameState switch to Free, the user will instantly click
    // the object under the cursor on Skill use
    private IEnumerator ShouldUpdateDelayCoroutine()
    {
        yield return new WaitForEndOfFrame();
        m_shouldUpdate = true;
    }
}