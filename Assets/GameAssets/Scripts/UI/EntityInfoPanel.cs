using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class EntityInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_nameTmp;
    [SerializeField] private TextMeshProUGUI m_stateTmp;
    [SerializeField] private TextMeshProUGUI m_hungerTmp;
    [SerializeField] private TextMeshProUGUI m_speedTmp;
    [SerializeField] private TextMeshProUGUI m_dietTmp;
    [SerializeField] private TextMeshProUGUI m_breedingReqTmp;

    private CanvasGroup m_canvasGroup;
    private Entity m_selectedEntity = null;

    private void Awake()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetEntity(Entity entity)
    {
        m_selectedEntity = entity;

        m_nameTmp.text = entity.DisplayName;
        m_stateTmp.text = entity.State.ToString();
        m_hungerTmp.text = "Hunger: " + entity.Hunger;
        m_speedTmp.text = "Speed: " + entity.MovementSpeed.ToString();
        m_dietTmp.text = "Eats: " + entity.GetDietAsString();
        m_breedingReqTmp.text = ">" + entity.BreedingHungerRequirement.ToString() + " Hunger to Breed";

        m_selectedEntity.OnStateChanged += OnEntityStateChanged;
        m_selectedEntity.OnHungerChanged += OnHungerStateChanged;

        m_selectedEntity.OnDeath += Clear;

        TogglePanelVisibility(true);
    }

    public void Clear()
    {
        if (m_selectedEntity != null)
        {
            m_selectedEntity.OnStateChanged -= OnEntityStateChanged;
            m_selectedEntity.OnHungerChanged -= OnHungerStateChanged;
            m_selectedEntity.OnDeath -= Clear;

            m_selectedEntity = null;
        }

        TogglePanelVisibility(false);
    }

    public void TogglePanelVisibility(bool visible)
    {
        m_canvasGroup.alpha = visible ? 1f : 0f;
    }

    private void OnEntityStateChanged(EntityState newState)
    {
        m_stateTmp.text = newState.ToString();
    }

    private void OnHungerStateChanged(float newHunger)
    {
        m_hungerTmp.text = "Hunger: " + newHunger;
    }
}