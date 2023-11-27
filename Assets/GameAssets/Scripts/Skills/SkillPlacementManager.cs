using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

public class SkillPlacementManager : MonoBehaviour
{
    [SerializeField] private SkillPlacementIndicator m_skill1PlacementIndicator;
    [SerializeField] private SkillPlacementIndicator m_skill2PlacementIndicator;
    [SerializeField] private SkillPlacementIndicator m_skill3PlacementIndicator;

    private SkillPlacementIndicator m_activeSkillPlacementIndicator;
    private int m_activeSkillIndex = -1;
    private Camera m_mainCamera;
    private Plane m_yZeroPlane;
    private float m_validCheckInterval = .25f;
    private float m_checkCounter = 0f;
    private GameManager m_gameManager;

    private void Awake()
    {
        m_mainCamera = Camera.main;
        m_yZeroPlane = new Plane(Vector3.up, Vector3.zero);
        m_gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        SkillsManager.OnActiveSkillChanged += SetActiveSkillPlacementIndicator;
    }

    private void OnDisable()
    {
        SkillsManager.OnActiveSkillChanged -= SetActiveSkillPlacementIndicator;
    }

    private void Update()
    {
        UpdateActiveSkillPlacementIndicator();
    }

    private void UpdateActiveSkillPlacementIndicator()
    {
        if (m_gameManager.GameState == GameState.Paused) return;

        if (m_activeSkillPlacementIndicator != null)
        {
            UpdatePlacementIndicatorPosition();

            m_checkCounter += Time.deltaTime;

            if (m_checkCounter >= m_validCheckInterval)
            {
                CheckIfPositionIsValid();

                m_checkCounter = 0f;
            }
        }
    }

    private void UpdatePlacementIndicatorPosition()
    {
        Vector3 hitPoint = GetMousePositionWithYZero();
        m_activeSkillPlacementIndicator.transform.position = hitPoint;
    }

    public bool GetActiveSkillPosition(out Vector3 position)
    {
        position = m_activeSkillPlacementIndicator.transform.position;
        return IsPointValid(position);
    }

    private void CheckIfPositionIsValid()
    {
        if (IsPointValid(m_activeSkillPlacementIndicator.transform.position))
        {
            m_activeSkillPlacementIndicator.SetIsValid(true);
        }
        else
        {
            m_activeSkillPlacementIndicator.SetIsValid(false);
        }
    }

    private void SetActiveSkillPlacementIndicator(int skillIndex)
    {
        m_activeSkillIndex = skillIndex;

        m_skill1PlacementIndicator.Hide();
        m_skill2PlacementIndicator.Hide();
        m_skill3PlacementIndicator.Hide();

        switch (m_activeSkillIndex)
        {
            case -1:
                m_activeSkillPlacementIndicator = null;
                return;
            case 1:
                m_activeSkillPlacementIndicator = m_skill1PlacementIndicator;
                m_activeSkillPlacementIndicator.SetIsValid(true);
                return;
            case 2:
                m_activeSkillPlacementIndicator = m_skill2PlacementIndicator;
                m_activeSkillPlacementIndicator.SetIsValid(true);
                return;
            case 3:
                m_activeSkillPlacementIndicator = m_skill3PlacementIndicator;
                m_activeSkillPlacementIndicator.SetIsValid(true);
                return;
        }
    }

    private bool IsPointValid(Vector3 point)
    {
        bool checkForObjects = false;
        if (m_activeSkillIndex == 1)
        {
            checkForObjects = true;
        }

        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(point, out navMeshHit, .5f, NavMesh.AllAreas))
        {
            if (checkForObjects)
            {
                Collider[] colliders = Physics.OverlapSphere(point, .5f);
                // Filter out colliders
                colliders = colliders.Where(collider => collider.GetComponent<IgnoreSkillPlacement>() == null).ToArray();

                if (colliders.Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    private Vector3 GetMousePositionWithYZero()
    {
        Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);

        if (m_yZeroPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return new Vector3(hitPoint.x, 0, hitPoint.z);
        }

        return Vector3.zero;
    }
}