using UnityEngine;

[System.Serializable]
public class DialogueData
{
    [TextArea(2, 5)]
    public string text;

    public Sprite image;
}