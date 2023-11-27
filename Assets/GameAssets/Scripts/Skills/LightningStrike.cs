using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    [SerializeField] private float m_spookRadius = 10f;
    [SerializeField] private float m_spookDuration = 4f;

    private void Awake()
    {
        SpookEntitiesInRange();
        Destroy(gameObject, m_spookDuration);
    }

    private void SpookEntitiesInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_spookRadius);

        List<Entity> entities = new List<Entity>();

        foreach (Collider collider in colliders)
        {
            Entity entity = collider.GetComponentInParent<Entity>();

            if (entity != null && !entities.Contains(entity))
            {
                entities.Add(entity);
            }
        }

        foreach(Entity entity in entities)
        {
            entity.Spook(transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_spookRadius);
    }
}