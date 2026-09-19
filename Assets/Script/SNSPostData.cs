using UnityEngine;

[CreateAssetMenu(
    fileName = "SNSPostData",
    menuName = "SNS/SNS Post Data"
)]
public class SNSPostData : ScriptableObject
{
    [Header("判定値")]
    public float threshold;

    [Header("投稿画像")]
    public Sprite image;

    [Header("投稿文章")]
    [TextArea(2, 5)]
    public string text;
}
