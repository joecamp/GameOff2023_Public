public class GrassFoodSource : FoodSource
{
    public override string GetDisplayName()
    {
        return "Grass";
    }

    public override float GetFoodValue()
    {
        return 10f;
    }

    public override void OnEaten()
    {
        OnDestroy?.Invoke();

        Destroy(gameObject);
    }
}