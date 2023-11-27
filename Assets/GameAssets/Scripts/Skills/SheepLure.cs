using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SheepLure : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 2f);

        CallNearbySheepToPosition();
    }

    private void CallNearbySheepToPosition()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);

        List<Entity> entities = new List<Entity>();

        foreach (Collider collider in colliders)
        {
            Entity entity = collider.GetComponentInParent<Entity>();

            if (entity != null && !entities.Contains(entity))
            {
                if (entity.tag == "Sheep")
                {
                    entities.Add(entity);
                }
            }
        }

        foreach (Entity entity in entities)
        {
            entity.CallToPosition(transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
}