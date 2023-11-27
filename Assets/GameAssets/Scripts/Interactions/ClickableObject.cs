using HighlightPlus;

using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour, IClickHandler, IHoverHandler
{
    public HighlightEffect HighlightEffect;
    public static UnityAction<ClickableObject> OnClickObject;
    public static UnityAction<ClickableObject> OnHoverEnterObject;
    public static UnityAction<ClickableObject> OnHoverExitObject;

    public void OnClick()
    {
        OnClickObject?.Invoke(this);
    }

    public void OnHoverEnter()
    {
        OnHoverEnterObject?.Invoke(this);
    }

    public void OnHoverExit()
    {
        OnHoverExitObject?.Invoke(this);
    }
}