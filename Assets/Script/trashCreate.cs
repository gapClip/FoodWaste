using UnityEngine;

public class trashCreate : MonoBehaviour
{
    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private int trashMeasure = 5;

    [SerializeField] private float Minx = -3.5f;
    [SerializeField] private float Maxx = 0f;
    [SerializeField] private float Miny = -3.5f;
    [SerializeField] private float Maxy = 3.5f;

    void Start()
    {
        int trashCount = Mathf.FloorToInt(ResultData.sceneLeftover / trashMeasure);

        for (int i = 0; i < trashCount; i++)
        {
            float x = Random.Range(Minx, Maxx);
            float y = Random.Range(Miny, Maxy);

            Vector3 spawnPosition = new Vector3(x, y, 0f);

            Instantiate(trashPrefab, spawnPosition, Quaternion.identity);
        }
    }
}