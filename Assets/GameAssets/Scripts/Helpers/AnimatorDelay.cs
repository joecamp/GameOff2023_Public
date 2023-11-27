using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorDelay : MonoBehaviour
{
    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();

        m_animator.speed = Random.Range(.5f, 1f);
    }
}