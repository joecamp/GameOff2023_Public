public class MeatFoodSource : FoodSource
{
    public override string GetDisplayName()
    {
        return "Meat";
    }

    public override float GetFoodValue()
    {
        return 50f;
    }

    public override void OnEaten()
    {
        OnDestroy?.Invoke();

        Destroy(gameObject);
    }
}