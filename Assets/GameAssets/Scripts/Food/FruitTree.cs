using DG.Tweening;
using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class FruitTree : MonoBehaviour
{
    [SerializeField] private int m_maxFruitSpawns = 10;
    [SerializeField] private float m_fruitMaturityDuration = 15f;
    [SerializeField] private float m_fruitSpawnCooldown = 5f;
    [SerializeField] private float m_spawnRadius = 5f;
    [SerializeField] private Color m_spawnColor;
    [SerializeField] private Color m_deathColor;
    [SerializeField] private DOTweenAnimation m_spawnFruitAnim;
    [SerializeField] private Renderer m_treeRenderer;
    [SerializeField] private List<Transform> m_fruitModelTransforms;
    [SerializeField] private AudioPlayer m_audioPlayer;
    [SerializeField] private GameObject m_fruitPrefab;

    private float m_treeMaxLifetime;
    private float m_lifeCounter = 0f;
    private int m_fruitSpawnCounter = 0;
    private float m_spawnCounter = 0f;
    private float m_cooldownCounter = 0f;
    private bool m_isOnCooldown = false;

    private bool m_isDead = false;

    public UnityAction OnDeath;

    private void Awake()
    {
        m_treeMaxLifetime = m_maxFruitSpawns * m_fruitMaturityDuration;
    }

    private void Update()
    {
        if (m_isDead) return;

        if (m_isOnCooldown)
        {
            m_cooldownCounter += Time.deltaTime;

            if (m_cooldownCounter > m_fruitSpawnCooldown)
            {
                m_cooldownCounter = 0f;
                m_isOnCooldown = false;
            }
            else
            {
                return;
            }
        }

        m_spawnCounter += Time.deltaTime;

        UpdateFruitScale();

        if (m_spawnCounter >= m_fruitMaturityDuration)
        {
            SpawnFruitOnNavMesh();
            m_spawnCounter = 0f;
            m_isOnCooldown = true;
        }

        UpdateTreeLife();
    }

    private void UpdateFruitScale()
    {
        Vector3 newScale = Vector3.Lerp(Vector3.zero, Vector3.one, m_spawnCounter / m_fruitMaturityDuration);
        foreach (Transform t in m_fruitModelTransforms)
        {
            t.localScale = newScale;
        }
    }

    private void UpdateTreeLife()
    {
        m_lifeCounter += Time.deltaTime;

        if (m_fruitSpawnCounter == m_maxFruitSpawns)
        {
            Death();
            return;
        }

        m_treeRenderer.material.color = Color.Lerp(m_spawnColor, m_deathColor, m_lifeCounter / m_treeMaxLifetime);
    }

    private void Death()
    {
        m_isDead = true;

        OnDeath?.Invoke();

        Tween deathAnim = transform.DOScale(Vector3.zero, 1f);
        deathAnim.SetEase(Ease.InElastic);

        Destroy(gameObject, 1.5f);
    }

    private void SpawnFruit()
    {
        m_spawnFruitAnim.DOPlay();
        m_audioPlayer.Play();

        foreach (Transform t in m_fruitModelTransforms)
        {
            // End position of fruit
            Vector3 endPos = Random.onUnitSphere * m_spawnRadius;
            endPos.y = 0f;
            endPos += transform.position;

            GameObject newFruit = Instantiate(m_fruitPrefab, t.position, Quaternion.identity);

            newFruit.transform.DOLocalJump(endPos, 5f, 1, .5f);

            t.localScale = Vector3.zero;
        }

        m_fruitSpawnCounter++;
    }

    private void SpawnFruitOnNavMesh()
    {
        m_spawnFruitAnim.DOPlay();
        m_audioPlayer.Play();

        foreach (Transform t in m_fruitModelTransforms)
        {
            // Get a random direction and ensure it's on the ground plane by setting y to 0
            Vector3 randomDirection = Random.insideUnitSphere * m_spawnRadius;
            randomDirection.y = 0f;
            randomDirection += transform.position;

            // Attempt to find the nearest point on the NavMesh within m_spawnRadius
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, m_spawnRadius, NavMesh.AllAreas))
            {
                // If a valid position is found, use it as the end position
                Vector3 endPos = hit.position;

                // Instantiate the fruit and animate it
                GameObject newFruit = Instantiate(m_fruitPrefab, t.position, Quaternion.identity);
                newFruit.transform.DOLocalJump(endPos, 5f, 1, .5f);

                t.localScale = Vector3.zero;
            }
            else
            {
                Debug.LogWarning("SpawnFruit: No valid NavMesh position found within radius.");
            }
        }

        m_fruitSpawnCounter++;
    }

    [Button]
    public float GetTreeLifePercent()
    {
        if (m_isDead) return 0f;

        return 1 - (m_lifeCounter / m_treeMaxLifetime);
    }

    [Button]
    public float GetTimeToNextSpawnPercent()
    {
        if (m_isDead) return 0f;

        return m_spawnCounter / m_fruitMaturityDuration;
    }
}