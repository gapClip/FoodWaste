using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SNSPostUI : MonoBehaviour
{
    [SerializeField] private Image postImage;
    [SerializeField] private TextMeshProUGUI postText;

    public void SetData(Sprite image, string text)
    {
        if (postImage != null)
        {
            postImage.sprite = image;
        }

        if (postText != null)
        {
            postText.text = text;
        }
    }
}