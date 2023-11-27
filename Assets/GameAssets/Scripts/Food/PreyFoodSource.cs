using UnityEngine;

public class PreyFoodSource : FoodSource
{
    [SerializeField] private GameObject m_audioPrefab;

    private Entity preyEntity;

    private void Awake()
    {
        preyEntity = GetComponent<Entity>();
    }

    public override string GetDisplayName()
    {
        return "Sheep";
    }

    public override float GetFoodValue()
    {
        return 40f;
    }

    public override void OnEaten()
    {
        OnDestroy?.Invoke();

        preyEntity.Die();

        GameObject spawnedPrefab = Instantiate(m_audioPrefab, transform.position, Quaternion.identity);

        Destroy(this);
    }
}