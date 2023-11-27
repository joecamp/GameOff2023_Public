using Sirenix.OdinInspector;

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SkillCooldownImage : MonoBehaviour
{
    private Image m_cooldownImage;

    private void Awake()
    {
        m_cooldownImage = GetComponent<Image>();
    }

    [Button]
    public void PlayCooldownAnimation(float duration)
    {
        StopAllCoroutines();

        StartCoroutine(CooldownAnimationCoroutine(duration));
    }

    private IEnumerator CooldownAnimationCoroutine(float duration)
    {
        // Set fillAmount to 1 at the start
        m_cooldownImage.fillAmount = 1f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fill = 1f - (elapsed / duration);
            m_cooldownImage.fillAmount = fill;
            yield return null;
        }

        // Ensure fillAmount is set to 0 at the end
        m_cooldownImage.fillAmount = 0f;
    }
}