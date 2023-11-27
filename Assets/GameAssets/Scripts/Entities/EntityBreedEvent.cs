using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EntityBreedEvent : MonoBehaviour
{
    public GameObject SheepPrefab;
    public GameObject WolfPrefab;
    public GameObject AudioPrefab;

    public Transform EntityPoint1;
    public Transform EntityPoint2;

    public Entity Entity1;
    public Entity Entity2;

    public bool IsSetup = false;
    public float MaxDuration = 5f;

    public float Counter;

    private EntityManager m_entityManager;

    private void Awake()
    {
        m_entityManager = FindObjectOfType<EntityManager>();
    }

    private void Update()
    {
        if(IsSetup)
        {
            Counter += Time.deltaTime;
            if(Counter >= MaxDuration)
            {
                CancelBreeding();
            }

            if(Entity1.State != EntityState.Breeding || Entity2.State != EntityState.Breeding || Entity1.IsDead || Entity2.IsDead)
            {
                CancelBreeding();
            }
            if(Entity1.IsInPositionToBreed && Entity2.IsInPositionToBreed)
            {
                FinishBreeding();
            }
        }
    }

    [Button]
    public void Setup(Entity entity1, Entity entity2)
    {
        Entity1 = entity1;
        Entity2 = entity2;

        // Find point equidistant between Entity1 and Entity2
        Vector3 centerPoint = (Entity1.transform.position + Entity2.transform.position) / 2;
        transform.position = centerPoint;

        // Set up the breeding positions for Entity1 and Entity2
        EntityPoint1.transform.position = GetPointInDirectionOfTransform(centerPoint, Entity1.transform);
        EntityPoint2.transform.position = GetPointInDirectionOfTransform(centerPoint, Entity2.transform);

        Entity1.SetBreeding(this, EntityPoint1.transform.position);
        Entity2.SetBreeding(this, EntityPoint2.transform.position);

        IsSetup = true;
    }

    public void FinishBreeding()
    {
        Entity1.FinishBreeding();
        Entity2.FinishBreeding();

        if(Entity1.tag == "Sheep")
        {
            if (m_entityManager.CanSpawnSheep())
            {
                Instantiate(SheepPrefab, transform.position, Utils.GetRandomRotationYAxis());
                Instantiate(AudioPrefab, transform.position, Quaternion.identity);
            }
        }
        else if(Entity1.tag == "Wolf")
        {
            if (m_entityManager.CanSpawnWolf())
            {
                Instantiate(WolfPrefab, transform.position, Utils.GetRandomRotationYAxis());
                Instantiate(AudioPrefab, transform.position, Quaternion.identity);
            }
        }
        
        Destroy(gameObject);
    }

    public void CancelBreeding()
    {
        if(Entity1.State == EntityState.Breeding && !Entity1.IsDead)
        {
            Entity1.CancelBreeding();
        }
        if(Entity2.State == EntityState.Breeding && !Entity2.IsDead)
        {
            Entity2.CancelBreeding();
        }

        Destroy(gameObject);
    }

    [Button]
    public void SetupTest(Transform t1, Transform t2)
    {
        // Find point equidistant between Entity1 and Entity2
        Vector3 centerPoint = (t1.position + t2.position) / 2;
        transform.position = centerPoint;

        // Set up the breeding positions for Entity1 and Entity2
        EntityPoint1.transform.position = GetPointInDirectionOfTransform(centerPoint, t1);
        EntityPoint2.transform.position = GetPointInDirectionOfTransform(centerPoint, t2);
    }

    private Vector3 GetPointInDirectionOfTransform(Vector3 centerPoint, Transform t)
    {
        // Calculate and normalize direction vector
        Vector3 direction = (t.position - centerPoint).normalized;

        // Find point one unit away from center
        Vector3 targetPoint = centerPoint + direction * 1f;

        return targetPoint;
    }
}