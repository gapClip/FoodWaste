using UnityEngine;

public class speechBubbleCtrl : MonoBehaviour
{
    [SerializeField] private GameObject speechBubble;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(GameBalance.Instance.GetTrashBagCount(GameState.TurnTrash)==0)
        {
            speechBubble.SetActive(true);
        }
        else
        {
            speechBubble.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
