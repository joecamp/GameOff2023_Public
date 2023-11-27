using DG.Tweening;

using UnityEngine;

public class ObjectInfoPanel : MonoBehaviour
{
    [SerializeField] private EntityInfoPanel m_entityInfoPanel;
    [SerializeField] private FoodSourceInfoPanel m_foodSourceInfoPanel;
    [SerializeField] private FruitTreeInfoPanel m_fruitTreeInfoPanel;

    public void OnClickedObject(ClickableObject clickableObject)
    {
        m_entityInfoPanel.Clear();
        m_foodSourceInfoPanel.Clear();
        m_fruitTreeInfoPanel.Clear();

        Entity entity = clickableObject.GetComponent<Entity>();
        FoodSource foodSource = clickableObject.GetComponent<FoodSource>();
        FruitTree tree = clickableObject.GetComponent<FruitTree>();

        if (entity != null)
        {
            m_entityInfoPanel.SetEntity(entity);
            return;
        }
        if (foodSource != null)
        {
            m_foodSourceInfoPanel.SetFoodSource(foodSource);
            return;
        }
        if (tree != null)
        {
            m_fruitTreeInfoPanel.SetTree(tree);
            return;
        }
    }

    public void Clear()
    {
        m_entityInfoPanel.Clear();
        m_foodSourceInfoPanel.Clear();
        m_fruitTreeInfoPanel.Clear();
    }
}