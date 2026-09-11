using System.Collections;
using UnityEngine;

public class trashDrag : MonoBehaviour
{
    private bool isDragging = false;
    private bool isInBonfire = false;
    private bool isIncinerating = false;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;

    private Vector3 offset;
[SerializeField] private GameObject smokePrefab;
    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -mainCamera.transform.position.z;

            Vector3 worldPosition =
                mainCamera.ScreenToWorldPoint(mousePosition);

            transform.position = worldPosition + offset;
        }
    }

    // ごみをクリックした
    void OnMouseDown()
    {
        if (isIncinerating)
            return;

        isDragging = true;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -mainCamera.transform.position.z;

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        // クリックした場所とごみの中心のズレを維持
        offset = transform.position - worldPosition;
    }

    // マウスを離した
    void OnMouseUp()
{
    isDragging = false;

    Incineration();
}

void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Bonfire"))
    {
        isInBonfire = true;

        Incineration();
    }
}

void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Bonfire"))
    {
        isInBonfire = false;
    }
}

void Incineration()
{
    if (!isDragging && isInBonfire && !isIncinerating)
    {
        isIncinerating = true;

         // 煙を発生
            Instantiate(
                smokePrefab,
                transform.position,
                Quaternion.identity
            );

        StartCoroutine(FadeOut());
    }
}

    IEnumerator FadeOut()
    {
        float duration = 1.0f;
        float time = 0f;

        Color color = spriteRenderer.color;

        while (time < duration)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(1f, 0f, time / duration);

            spriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}