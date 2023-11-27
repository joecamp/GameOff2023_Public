using UnityEngine;

public class EntitySpawn : MonoBehaviour
{
    [SerializeField] private GameObject m_entityPrefab;
    [SerializeField] private float m_spawnDelay;

    private void Awake()
    {
        Invoke("Spawn", m_spawnDelay);
    }

    private void Spawn()
    {
        GameObject newEntityGO = Instantiate(m_entityPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}