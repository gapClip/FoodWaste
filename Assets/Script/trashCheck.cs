using UnityEngine;

public class trashCheck : MonoBehaviour
{
    [SerializeField] private GameObject clearButton;
    private int trashCount;

    void Start()
    {
        clearButton.SetActive(false);
    }

    void Update()
    {
        trashCount =
            GameObject.FindGameObjectsWithTag("Trash").Length;

        if (trashCount == 0)
        {
            ButtonActive();
        }
    }
    void ButtonActive()
    {
        clearButton.SetActive(true);
    }
}