using UnityEngine;

public class SpawnPrefabOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject m_prefab;

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;

        GameObject spawnedPrefab = Instantiate(m_prefab, transform.position, Quaternion.identity);
    }
}