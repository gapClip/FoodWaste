using UnityEngine;

using UnityEngine.UI;

public class smokeLv : MonoBehaviour
{
    [Header("煙のImage")]
    [SerializeField] private Image smokeImage;

    [Header("段階ごとの透明度（アルファ値）")]
    [SerializeField, Range(0f, 1f)]
    private float[] alphaByStage = { 0.1f, 0.25f, 0.4f, 0.65f, 0.85f, 1.0f };

    private GameBalance gameBalance;
    [SerializeField] private float minScale = 1.1f;
[SerializeField] private float maxScale = 1.5f;
[SerializeField] private float scaleSpeed = 0.3f;

private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
        // GameBalanceを取得
        gameBalance = GameBalance.Instance;

        if (gameBalance == null)
        {
            Debug.LogError("GameBalanceが見つかりません。");
            return;
        }

        if (smokeImage == null)
        {
            smokeImage = GetComponent<Image>();
        }

        if (smokeImage == null)
        {
            Debug.LogError("煙のImageが見つかりません。");
            return;
        }

        // スタート時点の累計CO2を取得
        int totalCo2 = GameState.totalCo2;

        // CO2の段階を取得（0～5）
        int stage = gameBalance.GetCo2Stage(totalCo2);

        // 段階に対応するアルファ値を設定
        int index = Mathf.Clamp(stage, 0, alphaByStage.Length - 1);

        Color color = smokeImage.color;
        color.a = alphaByStage[index];
        smokeImage.color = color;

        Debug.Log($"累計CO2: {totalCo2}, 段階: {stage}, 透明度: {color.a}");
    }
    void Update()
    {
         // 1.1～1.5の間を往復
    float scale = Mathf.Lerp(
        minScale,
        maxScale,
        Mathf.PingPong(Time.time * scaleSpeed, 1f)
    );

    transform.localScale = baseScale * scale;
    }
}
