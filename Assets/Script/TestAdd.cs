using UnityEngine;
using UnityEngine.InputSystem;

public class TestAdd : MonoBehaviour
{
    public Inventory inventory;
    public FoodData testFood;
     private InputAction jumpAction;
     private FoodSlotUI foodSlotUI;

void Start()
    {
     jumpAction = InputSystem.actions.FindAction("Jump");   
    }
    public void addFood()
    {
        // if (jumpAction.triggered)
        // {
             inventory.AddFood(testFood);
        // }
    }
}