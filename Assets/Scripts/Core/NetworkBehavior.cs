using Fusion;
using UnityEngine;

public class NetworkBehavior : NetworkBehaviour
{
    [Header("Viewとの連携")]
    [SerializeField]
    private InGameViewManager inGameViewManager;

    [Header("お題のリスト（山札）")]
    [SerializeField, Tooltip("作ったQuestionDataをここに全部入れる")]
    private QuestionData[] allQuestions;

    /// <summary>
    /// 今選ばれているお題の番号（通信で全員に共有される！）
    /// </summary>
    [Networked]
    public int CurrentQuestionIndex { get; private set; }
    [Networked]
    public InGamePhase CurrentPhase { get; private set; }

    private InGamePhase _lastRenderedPhase;

    public override void Spawned()
    {
        if (inGameViewManager == null)
        {
            inGameViewManager = FindFirstObjectByType<InGameViewManager>();
        }

        _lastRenderedPhase = CurrentPhase;

        if (Object.HasStateAuthority)
        {
            CurrentPhase = InGamePhase.Setup;
        }

        ApplyPhaseToView(CurrentPhase);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (PlayerManager.Instance == null) return;

        CheckPhaseProgress();
    }

    public override void Render()
    {
        if (_lastRenderedPhase != CurrentPhase)
        {
            _lastRenderedPhase = CurrentPhase;
            ApplyPhaseToView(CurrentPhase);
        }
    }

    private void CheckPhaseProgress()
    {
        switch (CurrentPhase)
        {
            case InGamePhase.Setup:
                if (PlayerManager.Instance.AreAllPlayersStartReady())
                {
                    CurrentPhase = InGamePhase.WaitQuestion;
                    Debug.Log("[NetworkBehavior] 全員準備OK。WaitQuestionへ移行");
                }
                break;

            case InGamePhase.WaitQuestion:
                // 本来はここで「第1問！」などの演出を挟むが、今はすぐに親の回答フェーズへ進める
                CurrentPhase = InGamePhase.ParentAnswering;

                // ランダムにお題を1つ引く
                if (allQuestions != null && allQuestions.Length > 0)
                {
                    CurrentQuestionIndex = UnityEngine.Random.Range(0, allQuestions.Length);
                }
                Debug.Log($"[NetworkBehavior] お題を引きました！ ParentAnsweringへ移行します");
                break;

            case InGamePhase.ParentAnswering:
                if (PlayerManager.Instance.IsAnswerCreated(PlayerManager.Instance.ParentPlayer))
                {
                    CurrentPhase = InGamePhase.ChildrenAnswering;
                    Debug.Log("[NetworkBehavior] 親の回答完了。ChildrenAnsweringへ移行");
                }
                break;

            case InGamePhase.ChildrenAnswering:
                if (PlayerManager.Instance.AreAllChildrenAnswerCreated())
                {
                    CurrentPhase = InGamePhase.Calculate;
                    Debug.Log("[NetworkBehavior] 子全員の回答完了。Calculateへ移行");
                }
                break;

            case InGamePhase.Calculate:
                // 集計フェーズに入ったら、自動的に結果演出フェーズへ進める
                CurrentPhase = InGamePhase.ResultAnim;
                Debug.Log("[NetworkBehavior] 集計完了。ResultAnimへ自動移行します");
                break;

            case InGamePhase.RoundEnd:
                if (PlayerManager.Instance.AreAllPlayersNextQuestionReady())
                {
                    CurrentPhase = InGamePhase.WaitQuestion;
                    Debug.Log("[NetworkBehavior] 全員次問題OK。WaitQuestionへ移行");
                }
                break;
        }
    }

    // ホストが親の回答フェーズを開始する時に呼ばれる関数
    public void StartParentAnswering()
    {
        if (!Object.HasStateAuthority) return;

        // ここで山札の中からランダムに1つ番号を引く（03など）
        if (allQuestions != null && allQuestions.Length > 0)
        {
            CurrentQuestionIndex = UnityEngine.Random.Range(0, allQuestions.Length);
        }

        CurrentPhase = InGamePhase.ParentAnswering;
    }

    public void StartResultAnimation()
    {
        if (!Object.HasStateAuthority) return;
        CurrentPhase = InGamePhase.ResultAnim;
    }

    public void EndRound()
    {
        if (!Object.HasStateAuthority) return;
        CurrentPhase = InGamePhase.RoundEnd;
    }

    private void ApplyPhaseToView(InGamePhase phase)
    {
        if (inGameViewManager == null)
            inGameViewManager = FindFirstObjectByType<InGameViewManager>();

        if (inGameViewManager != null)
        {
            // 選ばれているお題を山札から取り出す
            QuestionData currentQuestion = null;
            if (allQuestions != null && CurrentQuestionIndex < allQuestions.Length)
            {
                currentQuestion = allQuestions[CurrentQuestionIndex];
            }

            // UIへ「フェーズ」と「お題データ」をセットで渡す！
            inGameViewManager.OnPhaseChanged(phase, currentQuestion);
        }
    }
    // テスト用に、強制的にお題表示フェーズへ行く関数を作っておきます
    public void TestStartQuestion()
    {
        // ランダムにお題を1つ選ぶ
        if (allQuestions != null && allQuestions.Length > 0)
        {
            CurrentQuestionIndex = UnityEngine.Random.Range(0, allQuestions.Length);
        }

        // お題表示フェーズ（親の回答フェーズ）へ！
        CurrentPhase = InGamePhase.ParentAnswering;
    }
}