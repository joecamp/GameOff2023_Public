public class FruitFoodSource : FoodSource
{
    public override string GetDisplayName()
    {
        return "Fruit";
    }

    public override float GetFoodValue()
    {
        return 25f;
    }

    public override void OnEaten()
    {
        OnDestroy?.Invoke();

        Destroy(gameObject);
    }
}