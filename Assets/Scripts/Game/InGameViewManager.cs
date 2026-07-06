using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTweenを使用するための宣言
using TMPro;


public class InGameViewManager : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField, Tooltip("問題文やタイマーをまとめた親オブジェクトのCanvasGroup")]
    private CanvasGroup questionUI;
    [Header("テキストの参照")]
    [SerializeField] private TextMeshProUGUI questionTextUI; // お題を表示するテキスト
    [SerializeField] private TextMeshProUGUI[] choiceTextUIs; // 選択肢を表示する4つのテキスト

    [Header("演出用UI")]
    [Tooltip("画面を覆う真っ黒なパネル（CanvasGroupが必要）")]
    [SerializeField] private CanvasGroup blackoutPanel;

    [Header("背景の切り替え")]
    [SerializeField] private GameObject singleLaneBackground;
    [SerializeField] private GameObject fourLanesBackground;

    [Header("電車とレーン設定")]
    [SerializeField, Tooltip("6人分の電車オブジェクトを順番にセット")]
    private Transform[] trains;
    [SerializeField, Tooltip("4つのレーン（①〜④）の基準となるオブジェクトをセット")]
    private Transform[] lanes;

    [Header("4つのレールの基準となるUI")]
    [Tooltip("fourLanesBackground1〜4を上から順に入れてください")]
    [SerializeField] private RectTransform[] laneRects = new RectTransform[4];

    // 自分が選んだ選択肢の番号（0〜3）を一時的に保存しておく変数
    private int myChoiceIndex = 0;

    // UIManagerから「これを選んだよ！」と教えてもらうための関数
    public void SetMyChoice(int choice)
    {
        myChoiceIndex = choice;
    }
    
    // クラスの最初（変数の宣言エリア）に追加
    private Vector3[] initialTrainPositions;
    // =========================================================
    //  八木君が使用する関数
    // =========================================================

    /// <summary>
    /// フェーズが切り替わった時に呼ばれる
    /// </summary>
    public void OnPhaseChanged(InGamePhase newPhase, QuestionData currentQuestion = null)
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
                ShowQuestionUI(currentQuestion);
                break;

            case InGamePhase.ChildrenAnswering:
                ShowQuestionUI(currentQuestion);
                break;

            case InGamePhase.ResultAnim:
                // 結果演出フェーズ：電車を表示して走らせる
                SetTrainsActive(true);
                PlayResultTransition();
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

    public void ShowQuestionUI(QuestionData currentQuestion)
    {
        if (questionUI == null || currentQuestion == null) return;

        if (singleLaneBackground != null) singleLaneBackground.SetActive(true);
        if (fourLanesBackground != null) fourLanesBackground.SetActive(false);

        // ① データをUIのテキストに代入する
        if (questionTextUI != null)
        {
            questionTextUI.text = currentQuestion.questionText;
        }

        // 4つの選択肢テキストに代入する
        for (int i = 0; i < choiceTextUIs.Length; i++)
        {
            if (i < currentQuestion.choices.Length && choiceTextUIs[i] != null)
            {
                choiceTextUIs[i].text = currentQuestion.choices[i];
            }
        }

        // ② アニメーションで表示する
        questionUI.DOKill(); // ★追加：残っているアニメーションを強制リセット！
        questionUI.gameObject.SetActive(true);
        questionUI.alpha = 0f;
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

    // ==========================================
    // ▼ 暗転から電車発車までの連続アニメーション ▼
    // ==========================================
    private void PlayResultTransition()
    {
        if (blackoutPanel == null) return;

        // まずBlackoutPanelを表示して、完全に透明（0）にしておく
        blackoutPanel.gameObject.SetActive(true);
        blackoutPanel.alpha = 0f;

        // DOTweenのSequenceを作って、やりたい事を順番に登録していく
        Sequence seq = DOTween.Sequence();

        // ① 0.5秒かけて画面を真っ黒にする（フェードイン）
        seq.Append(blackoutPanel.DOFade(1f, 0.5f));

        // ② 画面が真っ黒になった瞬間に、裏側でUIを消して電車を準備する
        seq.AppendCallback(() =>
        {
            if (questionUI != null) questionUI.gameObject.SetActive(false);

            // 背景を4車線に切り替える
            if (singleLaneBackground != null) singleLaneBackground.SetActive(false);
            if (fourLanesBackground != null) fourLanesBackground.SetActive(true);

            SetTrainsActive(true);

            // ▼▼▼ ワープ処理をこのように書き換えます ▼▼▼
            if (laneRects.Length > myChoiceIndex && laneRects[myChoiceIndex] != null)
            {
                // UI（RectTransform）の絶対座標(position.y)を取得する！
                // ※これにより、画面サイズが変わっても正確な高さを自動で取得できます
                float targetY = laneRects[myChoiceIndex].position.y;

                if (trains != null)
                {
                    foreach (Transform train in trains)
                    {
                        if (train != null)
                        {
                            // localPosition（親からの相対距離）ではなく、
                            // position（ワールド絶対座標）を使うのがズレないコツです！
                            Vector3 pos = train.position;
                            pos.y = targetY;
                            train.position = pos;
                        }
                    }
                }
            }
        });

        // ③ 0.5秒かけて黒い画面を透明に戻す（フェードアウト）
        seq.Append(blackoutPanel.DOFade(0f, 0.5f));

        // ④ 完全に画面が明るくなったら、電車を走らせる！
        seq.AppendCallback(() =>
        {
            blackoutPanel.gameObject.SetActive(false);

            // ここを変更！ trains配列に入っている全ての電車を走らせる
            if (trains != null)
            {
                foreach (Transform train in trains)
                {
                    if (train != null)
                    {
                        // 各電車にくっついているTrainMoverを取得してスタート！
                        TrainMover mover = train.GetComponent<TrainMover>();
                        if (mover != null)
                        {
                            mover.StartTrainAnimation();
                        }
                    }
                }
            }
        });
    }
}