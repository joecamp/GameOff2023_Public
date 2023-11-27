using MTAssets.MobileInputControls;
using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class CameraRig : MonoBehaviour
{
    [Header("Free Move")]
    [SerializeField] private float m_freeMoveSpeed = 10f;
    [SerializeField] private float m_freeMoveSmoothing = 20f;
    [SerializeField] private bool m_useScreenEdgePanning = false;
    [SerializeField] private float m_screenEdgeMaxDelta = .5f;
    private Vector3 m_goalPosition;

    [Header("Drag Movement")]
    [SerializeField] private bool m_allowDrag = true;
    [SerializeField] private float m_dragSpeed = 1f;

    private Vector3 m_lastMousePosition;

    [Header("Target Follow")]
    [SerializeField] private float m_followSmoothing = 20f;

    [Header("Zoom")]
    [SerializeField] private float m_zoomSpeed = 5f;
    [SerializeField] private float m_zoomSmoothing = 20f;
    [SerializeField] private float m_maxZoom = -10;
    [SerializeField] private float m_minZoom = -50;
    [SerializeField] private float m_zoomIncrement = 5f;
    private float m_goalZoom;

    [Header("Movement Bounds")]
    [SerializeField] private float minX = -25f;
    [SerializeField] private float maxX = 25f;
    [SerializeField] private float minZ = -25f;
    [SerializeField] private float maxZ = 25f;

    [Header("Preferences")]
    [SerializeField] private bool m_lockCamera = false;

    [Header("References")]
    [SerializeField] private Camera m_mainCamera = null;
    [SerializeField] private Transform m_audioListenerTransform = null;
    [SerializeField] private JoystickAxis m_joystick;

    // Input
    private float m_xAxisInput;
    private float m_yAxisInput;
    private float m_zoomInput;
    private Vector2 m_mousePosition;

    // Follow Target
    private bool m_isFollowingTarget = false;
    private Transform m_followTarget;

    private const float m_listenerMaxDistance = 25f;

    public UnityAction OnMoveInput;
    public bool IsMoving;

    private void Awake()
    {
        Setup();
    }

    private void Update()
    {
        UpdateFollowTarget();

        StashInput();
        UpdateZoom();
        UpdateFreeMove();

        if (m_allowDrag)
        {
            HandleDrag();
        }
    }

    private void Setup()
    {
        m_goalPosition = transform.position;
        m_goalZoom = m_mainCamera.orthographicSize;
    }

    private void StashInput()
    {
        if (m_lockCamera) return;

        m_xAxisInput = Input.GetAxis("Camera X");
        m_yAxisInput = Input.GetAxis("Camera Y");

        m_zoomInput = Input.GetAxis("Mouse ScrollWheel");

        if (m_useScreenEdgePanning)
        {
            m_mousePosition = Input.mousePosition;
        }
    }

    [Button]
    public void FollowTarget(Transform target)
    {
        m_isFollowingTarget = true;
        m_followTarget = target;
    }

    [Button]
    public void StopFollowingTarget()
    {
        m_isFollowingTarget = false;
        m_followTarget = null;

        m_goalPosition = transform.position;
    }

    private void UpdateFollowTarget()
    {
        if (!m_isFollowingTarget)
        {
            return;
        }

        if (m_followTarget == null)
        {
            m_isFollowingTarget = false;
            m_goalPosition = transform.position;
            return;
        }

        transform.position = Vector3.Lerp(transform.position, m_followTarget.position, Time.deltaTime * m_followSmoothing);
    }

    private void UpdateFreeMove()
    {
        Vector3 moveInput = Vector3.zero;

        if (m_joystick.pressed == true)
        {
            moveInput = new Vector3(m_joystick.inputVector.x, 0, m_joystick.inputVector.y);
        }
        else
        {
            moveInput = new Vector3(m_xAxisInput, 0, m_yAxisInput);
        }

        IsMoving = moveInput != Vector3.zero;

        if (m_isFollowingTarget && moveInput.magnitude > 0)
        {
            m_isFollowingTarget = false;
            m_goalPosition = transform.position;
        }
        if (m_isFollowingTarget) return;

        if (m_useScreenEdgePanning)
        {
            moveInput = ApplyScreenEdgePanningToInput(moveInput);
        }

        moveInput = moveInput.normalized;
        moveInput = transform.TransformDirection(moveInput);

        m_goalPosition += moveInput * m_freeMoveSpeed * Time.deltaTime;

        // Clamp the goal position within the specified bounds
        m_goalPosition.x = Mathf.Clamp(m_goalPosition.x, minX, maxX);
        m_goalPosition.z = Mathf.Clamp(m_goalPosition.z, minZ, maxZ);

        transform.position = Vector3.Lerp(transform.position, m_goalPosition, Time.deltaTime * m_freeMoveSmoothing);
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - m_lastMousePosition;
            Vector3 move = new Vector3(-delta.x, 0, -delta.y) * m_dragSpeed * Time.deltaTime;

            // Normalize and transform the movement to be relative to the camera's rotation
            move = transform.TransformDirection(move);
            move.y = 0; // Ensure there's no vertical movement

            m_goalPosition += move;

            // Clamp the goal position within the specified bounds
            m_goalPosition.x = Mathf.Clamp(m_goalPosition.x, minX, maxX);
            m_goalPosition.z = Mathf.Clamp(m_goalPosition.z, minZ, maxZ);

            transform.position = Vector3.Lerp(transform.position, m_goalPosition, Time.deltaTime * m_freeMoveSmoothing);

            m_lastMousePosition = Input.mousePosition;
        }
    }

    private Vector3 ApplyScreenEdgePanningToInput(Vector3 moveInput)
    {
        // Right Edge
        if (m_mousePosition.x >= Screen.width - m_screenEdgeMaxDelta)
        {
            moveInput.x += 1;
        }
        // Left Edge
        else if (m_mousePosition.x <= m_screenEdgeMaxDelta)
        {
            moveInput.x += -1;
        }
        // Top Edge
        if (m_mousePosition.y >= Screen.height - m_screenEdgeMaxDelta)
        {
            moveInput.z += 1;
        }
        // Bottom Edge
        else if (m_mousePosition.y <= m_screenEdgeMaxDelta)
        {
            moveInput.z += -1;
        }

        return moveInput;
    }

    private void UpdateZoom()
    {
        m_goalZoom -= m_zoomInput * m_zoomSpeed;
        m_goalZoom = Mathf.Clamp(m_goalZoom, m_maxZoom, m_minZoom);

        m_mainCamera.orthographicSize = Mathf.Lerp(m_mainCamera.orthographicSize, m_goalZoom, Time.deltaTime * m_zoomSmoothing);

        Vector3 newPos = m_audioListenerTransform.localPosition;
        newPos.z = GetZoomRatio() * m_listenerMaxDistance;
        m_audioListenerTransform.localPosition = newPos;
    }

    public void ZoomInButton()
    {
        m_goalZoom -= m_zoomIncrement;
        m_goalZoom = Mathf.Clamp(m_goalZoom, m_maxZoom, m_minZoom);
    }

    public void ZoomOutButton()
    {
        m_goalZoom += m_zoomIncrement;
        m_goalZoom = Mathf.Clamp(m_goalZoom, m_maxZoom, m_minZoom);
    }

    public float GetZoomRatio()
    {
        float zoomRatio = Mathf.Clamp01(m_goalZoom / m_minZoom);

        return zoomRatio;
    }
}