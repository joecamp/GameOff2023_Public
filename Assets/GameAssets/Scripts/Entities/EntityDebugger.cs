using TMPro;

using UnityEngine;

public class EntityDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_stateTmp;
    [SerializeField] private TextMeshProUGUI m_hungerTmp;

    private Entity m_entity;

    private void Awake()
    {
        m_entity = GetComponentInParent<Entity>();
        OnEntityStateChanged(m_entity.State);
    }

    private void Update()
    {
        m_hungerTmp.text = "HUNGER: " + m_entity.Hunger.ToString("F0");
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
        m_stateTmp.text = newState.ToString().ToUpper();
    }

    private void OnEntityDeath()
    {
        Destroy(gameObject);
    }
}