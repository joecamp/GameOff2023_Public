using DG.Tweening;

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Entity))]
public class SpawnMeatOnDeath : MonoBehaviour
{
    [SerializeField] private GameObject m_meatPrefab;
    [SerializeField] private float m_spawnRadius = 3.5f;
    [SerializeField] private int m_numberOfMeatToSpawn = 2;

    private Entity m_entity;

    private void Awake()
    {
        m_entity = GetComponent<Entity>();
    }

    private void OnEnable()
    {
        m_entity.OnDeath += SpawnMeatOnNavMesh;
    }

    private void OnDisable()
    {
        m_entity.OnDeath -= SpawnMeatOnNavMesh;
    }

    private void SpawnMeat()
    {
        for (int i = 0; i < m_numberOfMeatToSpawn; i++)
        {
            // End position of meat
            Vector3 endPos = Random.onUnitSphere * m_spawnRadius;
            endPos.y = 0f;
            endPos += transform.position;

            Vector3 randomEuler = Vector3.zero;
            randomEuler.y = Random.Range(0, 360);

            GameObject newMeat = Instantiate(m_meatPrefab, transform.position, Quaternion.Euler(randomEuler));

            newMeat.transform.DOLocalJump(endPos, 1.5f, 1, .5f);
        }
    }

    private void SpawnMeatOnNavMesh()
    {
        for (int i = 0; i < m_numberOfMeatToSpawn; i++)
        {
            // End position of meat
            Vector3 randomDirection = Random.insideUnitSphere * m_spawnRadius;
            randomDirection += transform.position;

            NavMeshHit hit;

            // Sample the nearest point on the NavMesh within the spawn radius
            if (NavMesh.SamplePosition(randomDirection, out hit, m_spawnRadius, NavMesh.AllAreas))
            {
                Vector3 endPos = hit.position;

                Vector3 randomEuler = Vector3.up * Random.Range(0, 360);
                GameObject newMeat = Instantiate(m_meatPrefab, transform.position, Quaternion.Euler(randomEuler));

                // Animate meat to jump to the end position
                newMeat.transform.DOLocalJump(endPos, 1.5f, 1, .5f);
            }
            else
            {
                Debug.LogWarning("SpawnMeat: No valid NavMesh position found within radius.");
            }
        }
    }
}