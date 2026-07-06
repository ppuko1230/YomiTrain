using Fusion;
using UnityEngine;

public class NetworkBehavior : NetworkBehaviour
{
    [Header("Viewとの連携")]
    [SerializeField]
    private InGameViewManager inGameViewManager;

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

            case InGamePhase.RoundEnd:
                if (PlayerManager.Instance.AreAllPlayersNextQuestionReady())
                {
                    CurrentPhase = InGamePhase.WaitQuestion;
                    Debug.Log("[NetworkBehavior] 全員次問題OK。WaitQuestionへ移行");
                }
                break;
        }
    }

    public void StartParentAnswering()
    {
        if (!Object.HasStateAuthority) return;
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
        {
            inGameViewManager = FindFirstObjectByType<InGameViewManager>();
        }

        if (inGameViewManager != null)
        {
            inGameViewManager.OnPhaseChanged(phase);
        }
    }
}