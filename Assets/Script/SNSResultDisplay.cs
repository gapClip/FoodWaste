using UnityEngine;

public class SNSResultDisplay : MonoBehaviour
{
    [Header("表示場所")]
    [SerializeField] private Transform postParent;

    [Header("SNS投稿Prefab")]
    [SerializeField] private GameObject postPrefab;

    [Header("成長段階：5段階")]
    [SerializeField] private SNSPostData[] growthPosts = new SNSPostData[5];

    [Header("味ランク：5段階")]
    [SerializeField] private SNSPostData[] tastePosts = new SNSPostData[5];

    [Header("余り率：5段階")]
    [SerializeField] private SNSPostData[] leftoverPosts = new SNSPostData[5];

    [Header("CO2排出量：5段階")]
    [SerializeField] private SNSPostData[] co2Posts = new SNSPostData[5];


    private ResultForEnding resultForEnding;


    private void Start()
    {
        resultForEnding =
            GetComponent<ResultForEnding>();

        if (resultForEnding == null)
        {
            Debug.LogWarning(
                "ResultForEndingが同じGameObjectにありません。"
            );

            return;
        }

        DisplayResult();
    }


    private void DisplayResult()
    {
        if (postPrefab == null)
        {
            Debug.LogWarning(
                "SNS投稿Prefabが設定されていません。"
            );

            return;
        }

        if (postParent == null)
        {
            Debug.LogWarning(
                "投稿表示場所が設定されていません。"
            );

            return;
        }


        // ResultForEndingから実際の結果を取得
        if (!resultForEnding.TryGetResult(
            out int level,
            out int tasteRank,
            out float leftoverRate))
        {
            return;
        }


        Debug.Log(
            $"SNS表示結果：Lv{level} / ★{tasteRank} / " +
            $"余り率{leftoverRate * 100f:F1}% / " +
            $"CO2 {GameState.totalCo2}"
        );


        // -------------------------
        // ① 成長段階
        // -------------------------

        CreatePost(
            GetStagePostData(
                growthPosts,
                level
            )
        );


        // -------------------------
        // ② 味ランク
        // -------------------------

        CreatePost(
            GetStagePostData(
                tastePosts,
                tasteRank
            )
        );


        // -------------------------
        // ③ 余り率
        // -------------------------

        CreatePost(
            GetLeftoverPostData(
                leftoverRate
            )
        );


        // -------------------------
        // ④ CO2排出量
        // -------------------------

        CreatePost(
            GetCo2PostData(
                GameState.totalCo2
            )
        );
    }


    // =========================================================
    // 5段階の結果を配列番号から取得
    // Lv1 → [0]
    // Lv2 → [1]
    // ...
    // Lv5 → [4]
    // =========================================================

    private SNSPostData GetStagePostData(
        SNSPostData[] posts,
        int stage)
    {
        if (posts == null || posts.Length == 0)
        {
            return null;
        }

        stage = Mathf.Clamp(stage, 1, posts.Length);

        SNSPostData data = posts[stage - 1];

        if (data == null)
        {
            Debug.LogWarning(
                $"SNS投稿データが設定されていません。段階：{stage}"
            );
        }

        return data;
    }


    // =========================================================
    // 数値によるthreshold判定
    // =========================================================

    private SNSPostData GetThresholdPostData(
        SNSPostData[] posts,
        float value)
    {
        if (posts == null || posts.Length == 0)
        {
            return null;
        }


        SNSPostData result = null;


        for (int i = 0; i < posts.Length; i++)
        {
            if (posts[i] == null)
            {
                continue;
            }

            if (value >= posts[i].threshold)
            {
                result = posts[i];
            }
        }


        if (result == null)
        {
            Debug.LogWarning(
                $"値 {value:F1} に対応するSNS投稿データがありません。"
            );
        }


        return result;
    }


    // =========================================================
    // 余り率によるSNS投稿
    // =========================================================

    private SNSPostData GetLeftoverPostData(
        float leftoverRate)
    {
        float percent = leftoverRate * 100f;

        if (Mathf.Approximately(percent, 0f))
            return leftoverPosts[0];

        if (percent < 10f)
            return leftoverPosts[1];

        if (percent < 30f)
            return leftoverPosts[2];

        if (percent < 50f)
            return leftoverPosts[3];

        return leftoverPosts[4];
    }


    // =========================================================
    // CO2排出量によるSNS投稿
    //
    // 0 ～ 9   → [0]
    // 10 ～ 24 → [1]
    // 25 ～ 44 → [2]
    // 45 ～ 69 → [3]
    // 70以上   → [4]
    // =========================================================

    private SNSPostData GetCo2PostData(
        float totalCo2)
    {
        if (co2Posts == null || co2Posts.Length < 5)
        {
            Debug.LogWarning(
                "CO2投稿データが5つ設定されていません。"
            );

            return null;
        }


        if (totalCo2 < 10f)
        {
            return co2Posts[0];
        }

        if (totalCo2 < 25f)
        {
            return co2Posts[1];
        }

        if (totalCo2 < 45f)
        {
            return co2Posts[2];
        }

        if (totalCo2 < 70f)
        {
            return co2Posts[3];
        }

        return co2Posts[4];
    }


    // =========================================================
    // SNS投稿Prefabを生成
    // =========================================================

    private void CreatePost(SNSPostData data)
    {
        if (data == null)
        {
            Debug.LogWarning(
                "表示するSNS投稿データがありません。"
            );

            return;
        }


        GameObject obj =
            Instantiate(
                postPrefab,
                postParent
            );


        SNSPostUI postUI =
            obj.GetComponent<SNSPostUI>();


        if (postUI == null)
        {
            Debug.LogWarning(
                "SNS投稿PrefabにSNSPostUIがありません。"
            );

            return;
        }


        postUI.SetData(
            data.image,
            data.text
        );
    }
}