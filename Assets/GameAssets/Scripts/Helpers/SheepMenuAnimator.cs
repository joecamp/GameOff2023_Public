using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SheepMenuAnimator : MonoBehaviour
{
    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();

        InvokeRepeating("IdleTrigger", 5f, 15f);
    }

    private void IdleTrigger()
    {
        m_animator.SetTrigger("IdleTrigger");
    }
}