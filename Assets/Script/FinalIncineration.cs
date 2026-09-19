using System.Collections;
using UnityEngine;

public class FinalIncineration : MonoBehaviour
{
    public static int finalTrashCount = 0;

    [Header("ごみを生成する場所")]
    [SerializeField] private Transform spawnPoint;

    [Header("生成するごみPrefab")]
    [SerializeField] private GameObject trashPrefab;

    [Header("カメラ")]
    [SerializeField] private Transform cameraTransform;

    [Header("カメラの上昇先Y座標")]
    [SerializeField] private float cameraTargetY = 10f;

    [Header("カメラ上昇速度")]
    [SerializeField] private float cameraMoveSpeed = 2f;

    [SerializeField] private GameObject button;


    private int spawnedCount = 0;
    private bool isSpawning = false;
    private bool isCameraMoving = false;


    private void Start()
    {
        StartIncineration();
        button.SetActive(false);
    }


    // ========================================
    // ごみ生成開始
    // ========================================

    private void StartIncineration()
    {
        if (trashPrefab == null)
        {
            Debug.LogWarning(
                "ごみPrefabが設定されていません。"
            );

            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning(
                "ごみの生成場所が設定されていません。"
            );

            return;
        }

        if (finalTrashCount <= 0)
        {
            Debug.Log(
                "生成するごみがありません。"
            );

            StartCameraMove();

            return;
        }

        StartCoroutine(SpawnTrash());
    }


    // ========================================
    // 0.5秒ごとにごみを生成
    // ========================================

    private IEnumerator SpawnTrash()
    {
        isSpawning = true;

        while (spawnedCount < finalTrashCount)
        {
            Instantiate(
                trashPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            spawnedCount++;

            Debug.Log(
                $"ごみ生成: {spawnedCount} / {finalTrashCount}"
            );

            // 次のごみまで0.5秒待つ
            yield return new WaitForSeconds(0.5f);
        }

        isSpawning = false;

        Debug.Log(
            "すべてのごみを生成しました。カメラ上昇開始。"
        );

        StartCameraMove();
    }


    // ========================================
    // カメラ上昇開始
    // ========================================

    private void StartCameraMove()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning(
                "カメラが設定されていません。"
            );

            return;
        }

        isCameraMoving = true;
    }


    // ========================================
    // カメラを指定高さまで上昇
    // ========================================

    private void Update()
    {
        if (!isCameraMoving)
        {
            return;
        }

        Vector3 position =
            cameraTransform.position;

        position.y =
            Mathf.MoveTowards(
                position.y,
                cameraTargetY,
                cameraMoveSpeed * Time.deltaTime
            );

        cameraTransform.position = position;


        // 指定位置まで到達
        if (Mathf.Approximately(
            position.y,
            cameraTargetY))
        {
            isCameraMoving = false;

            Debug.Log(
                "カメラが指定位置まで到達しました。"
            );
            button.SetActive(true);
        }
    }
}