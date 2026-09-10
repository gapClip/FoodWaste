using UnityEngine;
using TMPro;

public class ShopPanelMove : MonoBehaviour
{
    [SerializeField] private RectTransform panel;

    [Header("Y座標")]
    [SerializeField] private float panelUpY = 0f;
    [SerializeField] private float panelDownY = -500f;

    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private bool isShop = true;

    private bool isUp = true;
    private Vector2 targetPos;

    private void Start()
    {
        targetPos = panel.anchoredPosition;
        
    }

    private void Update()
    {
        panel.anchoredPosition = Vector2.Lerp(
            panel.anchoredPosition,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }

    public void PanelMove()
    {
        if (isUp)
        {
            targetPos = new Vector2(panel.anchoredPosition.x, panelDownY);
        }
        else
        {
            targetPos = new Vector2(panel.anchoredPosition.x, panelUpY);
        }

        isUp = !isUp;
    }

    public void PanelClose()
    {
        targetPos = new Vector2(panel.anchoredPosition.x, panelUpY);
    }
}