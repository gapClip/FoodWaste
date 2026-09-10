using UnityEngine;
using UnityEngine.UI;

public class TestAdd : MonoBehaviour
{
    [SerializeField] private FoodData testFood;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetFood(FoodData food)
    {
        testFood = food;

        if (image != null)
        {
            image.sprite = food.foodSprite;
        }
    }

    public FoodData GetFood()
    {
        return testFood;
    }
}