using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_fruitTreePrefab;

    [Button]
    private void TrySpawnFruitTree(Vector3 spawnPosition)
    {
        NavMeshHit hit;

        // Sample the nearest point on the NavMesh from spawnPosition
        if (NavMesh.SamplePosition(spawnPosition, out hit, 1f, NavMesh.AllAreas))
        {
            GameObject fruitTreeGO = Instantiate(m_fruitTreePrefab, hit.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError(spawnPosition.ToString() + " is not a valid position on the NavMesh.");
        }
    }
}