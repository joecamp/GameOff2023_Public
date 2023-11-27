using UnityEngine;
using UnityEngine.Events;

public abstract class FoodSource : MonoBehaviour
{
    public UnityAction OnDestroy;

    public abstract string GetDisplayName();

    public abstract float GetFoodValue();

    public abstract void OnEaten();
}