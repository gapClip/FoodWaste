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
        // ゴミを燃やし終えたら、ターンのゴミを累計CO₂と満足度に反映する（1ターンに1回だけ）
        GameState.Incinerate();

        clearButton.SetActive(true);
    }
}