using UnityEngine;

public class LimitedLifespanObject : MonoBehaviour
{
    [SerializeField] private float m_lifetimeInSeconds = 1;

    private void Awake()
    {
        Destroy(gameObject, m_lifetimeInSeconds);
    }
}