using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public class EntityManager : MonoBehaviour
{
    [SerializeField] private int m_maxSheepAllowed;
    public int MaxSheepAllowed { get { return m_maxSheepAllowed; } }
    [SerializeField] private int m_maxWolvesAllowed;
    public int MaxWolvesAllowed { get { return m_maxWolvesAllowed; } }

    private int m_sheepCount;
    public int SheepCount 
    {
        get
        {
            return m_sheepCount;
        }
        private set
        {
            m_sheepCount = value;
            OnSheepCountChanged?.Invoke(m_sheepCount);
        }
    }
    
    private int m_wolfCount;
    public int WolfCount 
    { 
        get
        {
            return m_wolfCount;
        }
        private set
        {
            if(m_wolfCount != value)
            {
                m_wolfCount = value;
                OnWolfCountChanged?.Invoke(m_wolfCount);
            }
            
        }
    }

    public UnityAction<int> OnSheepCountChanged;
    public UnityAction<int> OnWolfCountChanged;

    private void OnEnable()
    {
        Entity.OnEntitySpawn += UpdateEntityCounts;
        Entity.OnEntityDeath += UpdateEntityCounts;
    }

    private void OnDisable()
    {
        Entity.OnEntitySpawn -= UpdateEntityCounts;
        Entity.OnEntityDeath -= UpdateEntityCounts;
    }

    private void UpdateEntityCounts()
    {
        StartCoroutine(UpdateEntityCountsCoroutine());
    }

    private IEnumerator UpdateEntityCountsCoroutine()
    {
        yield return new WaitForEndOfFrame();

        GameObject[] sceneSheeps = GameObject.FindGameObjectsWithTag("Sheep");
        SheepCount = sceneSheeps.Length;

        GameObject[] sceneWolves = GameObject.FindGameObjectsWithTag("Wolf");
        WolfCount = sceneWolves.Length;
    }

    public bool CanSpawnSheep()
    {
        return SheepCount < MaxSheepAllowed;
    }

    public bool CanSpawnWolf()
    {
        return WolfCount < MaxWolvesAllowed;
    }
}