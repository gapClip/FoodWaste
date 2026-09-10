using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Camera maincamera;
    [SerializeField] private float moveSpeed = 40f;
    [SerializeField] private float moveDistance = 20f;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private GameObject leftButton;

    private Vector3 targetPosition;
    private int currentPositionIndex = 0;

    void Start()
    {
        targetPosition = maincamera.transform.position;
    }

    void Update()
    {
        if (currentPositionIndex<=-1)
        {
            currentPositionIndex = -1;
            leftButton.SetActive(false);
        }
        else
        {
            leftButton.SetActive(true);
        }
        if (currentPositionIndex >= 1)
        {
            currentPositionIndex = 1;
            rightButton.SetActive(false);
        }
        else
        {
            rightButton.SetActive(true);
        }
        maincamera.transform.position = Vector3.MoveTowards(
            maincamera.transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    // 右へ移動
    public void MoveRight()
    {
        currentPositionIndex++;
        targetPosition += new Vector3(moveDistance, 0f, 0f);

        Debug.Log("Camera Move Right");
    }

    // 左へ移動
    public void MoveLeft()
    {
        currentPositionIndex--;
        targetPosition += new Vector3(-moveDistance, 0f, 0f);

        Debug.Log("Camera Move Left");
    }
}