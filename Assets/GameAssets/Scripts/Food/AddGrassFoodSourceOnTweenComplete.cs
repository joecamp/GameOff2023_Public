using DG.Tweening;

using UnityEngine;

public class AddGrassFoodSourceOnTweenComplete : MonoBehaviour
{
    private Tween initTween;

    private void Awake()
    {
        initTween = transform.DOScale(.6f, 1f).SetEase(Ease.OutElastic);
        initTween.OnComplete(() => OnTweenComplete());
    }

    private void OnTweenComplete()
    {
        gameObject.AddComponent<GrassFoodSource>();
        Destroy(this);
    }
}