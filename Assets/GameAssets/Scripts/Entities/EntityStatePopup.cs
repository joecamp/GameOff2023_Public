using UnityEngine;

public class EntityStatePopup : MonoBehaviour
{
    [SerializeField] private GameObject m_searchingForFoodPopup;
    [SerializeField] private GameObject m_huntingFoodPopup;
    [SerializeField] private GameObject m_fleeingPopup;
    [SerializeField] private GameObject m_searchingForPartnerPopup;
    [SerializeField] private GameObject m_breedingPopup;

    private Entity m_entity;
    private GameObject m_activePopup = null;

    private void Awake()
    {
        m_entity = GetComponentInParent<Entity>();
    }

    private void OnEnable()
    {
        m_entity.OnStateChanged += OnEntityStateChanged;
        m_entity.OnDeath += OnEntityDeath;
    }

    private void OnDisable()
    {
        m_entity.OnStateChanged -= OnEntityStateChanged;
        m_entity.OnDeath -= OnEntityDeath;
    }

    private void OnEntityStateChanged(EntityState newState)
    {
        if (m_activePopup != null)
        {
            m_activePopup.SetActive(false);
        }

        switch (newState)
        {
            case EntityState.Idle:
                m_activePopup = null;
                break;
            case EntityState.Roaming:
                m_activePopup = null;
                break;
            case EntityState.HuntingFood:
                m_activePopup = m_huntingFoodPopup;
                break;
            case EntityState.Fleeing:
                m_activePopup = m_fleeingPopup;
                break;
            case EntityState.Breeding:
                m_activePopup = m_breedingPopup;
                break;
        }

        if (m_activePopup != null)
        {
            m_activePopup.SetActive(true);
        }
    }

    private void OnEntityDeath()
    {
        Destroy(gameObject);
    }
}