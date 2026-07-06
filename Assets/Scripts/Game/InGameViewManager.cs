using UnityEngine;
using DG.Tweening; // DOTweenを使用するための宣言
using UnityEngine.UI;

public class InGameViewManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField, Tooltip("問題文やタイマーをまとめた親オブジェクトのCanvasGroup")]
    private CanvasGroup questionUI;

    [Header("電車とレーン設定")]
    [SerializeField, Tooltip("6人分の電車オブジェクトを順番にセット")]
    private Transform[] trains;
    [SerializeField, Tooltip("4つのレーン（①〜④）の基準となるオブジェクトをセット")]
    private Transform[] lanes;

    [SerializeField] private TrainMover trainMover;

    // クラスの最初（変数の宣言エリア）に追加
    private Vector3[] initialTrainPositions;
    // =========================================================
    //  八木君が使用する関数
    // =========================================================

    /// <summary>
    /// フェーズが切り替わった時に呼ばれる
    /// </summary>
    public void OnPhaseChanged(InGamePhase newPhase)
    {
        Debug.Log($"[View] フェーズ演出開始: {newPhase}");

        switch (newPhase)
        {
            case InGamePhase.Setup:
                ResetView();
                break;

            case InGamePhase.WaitQuestion:
                HideQuestionUI();
                break;

            case InGamePhase.ParentAnswering:
                ShowQuestionUI();
                break;

            case InGamePhase.ChildrenAnswering:
                ShowQuestionUI();
                break;

            case InGamePhase.ResultAnim:
                // ★結果演出フェーズ：電車を表示して走らせる
                SetTrainsActive(true);

                // ここでTrainMoverのStart処理を呼ぶ（各電車にアタッチされている場合）
                // 例: foreachで各電車のスクリプトを取得して走らせる
                break;

            case InGamePhase.RoundEnd:
                // ラウンド終了：電車を非表示にする
                SetTrainsActive(false);
                HideQuestionUI();
                // TODO: 次のラウンドへ向けて障害物を消すなどの処理を書く

                break;
        }
    }

    // =========================================================
    // 初期化処理
    // =========================================================
    private void Start()
    {
        // ゲーム開始時に、各電車の最初の座標を配列に記憶しておく
        if (trains != null)
        {
            initialTrainPositions = new Vector3[trains.Length];
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i] != null)
                {
                    initialTrainPositions[i] = trains[i].position;
                }
            }
        }
    }

    /// <summary>
    /// 誰かがレーンを選んだ時に呼ばれる
    /// </summary>
    public void MoveTrain(int playerIndex, int laneIndex)
    {
        // 念のため、エラーを防ぐチェック
        if (playerIndex < 0 || playerIndex >= trains.Length) return;
        if (laneIndex < 0 || laneIndex >= lanes.Length) return;

        Transform targetTrain = trains[playerIndex];
        float targetY = lanes[laneIndex].position.y; // 移動先のY座標を取得

        // DOTweenを使って、0.5秒かけて「スッ」とY軸を移動させる
        targetTrain.DOMoveY(targetY, 0.5f).SetEase(Ease.OutCubic);
    }


    // =========================================================
    // ▼ 内部で実行するアニメーション処理 ▼
    // =========================================================

    private void ShowQuestionUI()
    {
        if (questionUI == null) return;

        questionUI.gameObject.SetActive(true);
        questionUI.alpha = 0f;
        // 0.5秒かけて透明度を0から1へ（フェードイン）
        questionUI.DOFade(1f, 0.5f);
    }

    private void HideQuestionUI()
    {
        if (questionUI == null) return;

        // 0.5秒かけて透明度を1から0へ（フェードアウト）し、終わったら非表示にする
        questionUI.DOFade(0f, 0.5f).OnComplete(() =>
        {
            questionUI.gameObject.SetActive(false);
        });
    }

    private void ResetView()
    {
        // ラウンド開始前のリセット処理
        HideQuestionUI();

        // 電車の位置を初期位置に戻す処理
        if (trains != null && initialTrainPositions != null)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i] != null)
                {
                    // 記憶しておいた初期座標を代入して戻す
                    trains[i].position = initialTrainPositions[i];
                }
            }
        }
    }

    /// <summary>
    /// 配列内のすべての電車の表示/非表示を切り替える関数
    /// </summary>
    private void SetTrainsActive(bool isActive)
    {
        // 配列が空でないかチェック
        if (trains == null) return;

        foreach (Transform train in trains)
        {
            if (train != null)
            {
                // TransformからgameObjectにアクセスしてSetActiveを呼ぶ
                train.gameObject.SetActive(isActive);
            }
        }
    }
}