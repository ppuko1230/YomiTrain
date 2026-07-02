using UnityEngine;
using DG.Tweening; // DOTweenを使用するための宣言

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
                // TODO: 背景スクロールの開始や、ポッポー音の再生などをここに書く
                break;

            case InGamePhase.ParentAnswering:
                ShowQuestionUI();
                break;

            case InGamePhase.ChildrenAnswering:
                ShowQuestionUI();
                break;

            case InGamePhase.ResultAnim:
                HideQuestionUI();
                // TODO: 当たり外れの判定データを受け取って、障害物を出す処理などをここに書く
                break;

            case InGamePhase.RoundEnd:
                // TODO: 次のラウンドへ向けて障害物を消すなどの処理を書く
                break;
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

        // TODO: 電車の位置を初期位置に戻す処理などを追加
    }
}