using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_grassPrefabs;
    [SerializeField] private int m_initialGrassSpawns;
    [SerializeField] private float m_timeBetweenGrassSpawns;
    [SerializeField] private int m_grassCreatedPerSpawn;
    [SerializeField] private int m_maxGrassPerScene;

    private float m_spawnCounter = 0f;
    private Vector3 m_spawnPositionModifier = new Vector3(0f, -.05f, 0f);

    private void Awake()
    {
        for (int i = 0; i < m_initialGrassSpawns; i++)
        {
            SpawnGrass();
        }
    }

    private void Update()
    {
        m_spawnCounter += Time.deltaTime;

        if (m_spawnCounter >= m_timeBetweenGrassSpawns)
        {
            SpawnGrass();
            m_spawnCounter = 0f;
        }
    }

    [Button]
    public void SpawnGrass()
    {
        if (GetNumberOfGrassInScene() >= m_maxGrassPerScene) { return; }

        if (m_grassPrefabs.Count == 0) { return; }

        for (int i = 0; i < m_grassCreatedPerSpawn; i++)
        {
            Vector3 pos = RandomNavMeshPoint.GetRandomPointOnNavMesh();
            pos += m_spawnPositionModifier;
            int prefabIndex = Random.Range(0, m_grassPrefabs.Count);

            Instantiate(m_grassPrefabs[prefabIndex], pos, Quaternion.identity);
        }
    }

    private int GetNumberOfGrassInScene()
    {
        GrassFoodSource[] grassInScene = FindObjectsOfType<GrassFoodSource>();
        return grassInScene.Length;
    }
}