using UnityEngine;
using TMPro;
public class turnCounter : MonoBehaviour
{
    GameBalance gameBalance;
        [SerializeField] private TextMeshProUGUI counterText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counterText.text = "ターン " + GameState.turn + " / " + GameBalance.Instance.turnCount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
