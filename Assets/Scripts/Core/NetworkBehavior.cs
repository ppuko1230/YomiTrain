using Fusion;
using UnityEngine;

/// <summary>
/// 各プレイヤーの進行状態
/// </summary>
public enum PlayerAnswerStep
{
    None,               // 何もしていない
    Ready,              // ゲーム開始前の準備OK
    ParentAnswered,     // 親が回答完了
    ChildAnswered       // 子が回答完了
}

/// <summary>
/// ゲーム中のプレイヤー状態をPhoton Fusionで同期するクラス
/// </summary>
public class NetworkBehavior : NetworkBehaviour
{
    [Header("Viewとの連携")]
    [SerializeField]
    private InGameViewManager inGameViewManager;

    /// <summary>
    /// 現在のゲームフェーズ
    /// </summary>
    [Networked]
    public InGamePhase CurrentPhase { get; private set; }

    /// <summary>
    /// 今回の親プレイヤー
    /// </summary>
    [Networked]
    public PlayerRef ParentPlayer { get; private set; }

    /// <summary>
    /// プレイヤーごとの状態
    /// 最大6人想定
    /// </summary>
    [Networked, Capacity(6)]
    private NetworkDictionary<PlayerRef, PlayerAnswerStep> PlayerSteps => default;

    private InGamePhase _lastRenderedPhase;

    // =========================================================
    // Fusion初期化
    // =========================================================

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
            ParentPlayer = PlayerRef.None;

            InitializePlayerSteps();
        }

        ApplyPhaseToView(CurrentPhase);
    }

    public override void Render()
    {
        // Networked変数の変更を各クライアント側のViewへ反映する
        if (_lastRenderedPhase != CurrentPhase)
        {
            _lastRenderedPhase = CurrentPhase;
            ApplyPhaseToView(CurrentPhase);
        }
    }

    // =========================================================
    // 外部から呼ぶ関数
    // =========================================================

    /// <summary>
    /// 準備OKボタンを押した時に呼ぶ
    /// </summary>
    public void RequestSetReady(bool isReady)
    {
        RPC_SetReady(isReady);
    }

    /// <summary>
    /// 親が回答し終えた時に呼ぶ
    /// </summary>
    public void RequestParentAnswerCompleted()
    {
        RPC_ParentAnswerCompleted();
    }

    /// <summary>
    /// 親以外のプレイヤーが回答し終えた時に呼ぶ
    /// </summary>
    public void RequestChildAnswerCompleted()
    {
        RPC_ChildAnswerCompleted();
    }

    /// <summary>
    /// 親を設定して、親の回答フェーズへ進める
    /// 基本的にはHost側から呼ぶ
    /// </summary>
    public void StartParentAnswering(PlayerRef parentPlayer)
    {
        if (!Object.HasStateAuthority) return;

        ParentPlayer = parentPlayer;

        ResetRoundAnswerSteps();

        CurrentPhase = InGamePhase.ParentAnswering;
    }

    /// <summary>
    /// 集計が終わり、結果演出へ進めたい時にHost側から呼ぶ
    /// </summary>
    public void StartResultAnimation()
    {
        if (!Object.HasStateAuthority) return;

        CurrentPhase = InGamePhase.ResultAnim;
    }

    /// <summary>
    /// ラウンド終了へ進める
    /// </summary>
    public void EndRound()
    {
        if (!Object.HasStateAuthority) return;

        CurrentPhase = InGamePhase.RoundEnd;
    }

    // =========================================================
    // RPC
    // =========================================================

    /// <summary>
    /// 各プレイヤーが準備OKかどうかをHostへ送る
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SetReady(NetworkBool isReady, RpcInfo info = default)
    {
        PlayerRef player = info.Source;

        if (isReady)
        {
            PlayerSteps.Set(player, PlayerAnswerStep.Ready);
        }
        else
        {
            PlayerSteps.Set(player, PlayerAnswerStep.None);
        }

        Debug.Log($"[NetworkBehavior] {player} Ready = {isReady}");

        if (AreAllPlayersReady())
        {
            Debug.Log("[NetworkBehavior] 全員準備OK。ゲーム開始");

            // 全員準備OKになったのでゲーム開始
            // ここでは問題待ちフェーズへ進める
            CurrentPhase = InGamePhase.WaitQuestion;
        }
    }

    /// <summary>
    /// 親が回答を完了したことをHostへ送る
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_ParentAnswerCompleted(RpcInfo info = default)
    {
        PlayerRef player = info.Source;

        // 親以外から送られてきた場合は無視
        if (player != ParentPlayer)
        {
            Debug.LogWarning($"[NetworkBehavior] 親ではないプレイヤーから親回答完了RPCが呼ばれました: {player}");
            return;
        }

        PlayerSteps.Set(player, PlayerAnswerStep.ParentAnswered);

        Debug.Log($"[NetworkBehavior] 親 {player} の回答完了");

        // 親の回答が終わったので、子の回答フェーズへ進める
        CurrentPhase = InGamePhase.ChildrenAnswering;
    }

    /// <summary>
    /// 子プレイヤーが回答を完了したことをHostへ送る
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_ChildAnswerCompleted(RpcInfo info = default)
    {
        PlayerRef player = info.Source;

        // 親から送られてきた場合は無視
        if (player == ParentPlayer)
        {
            Debug.LogWarning($"[NetworkBehavior] 親プレイヤーから子回答完了RPCが呼ばれました: {player}");
            return;
        }

        PlayerSteps.Set(player, PlayerAnswerStep.ChildAnswered);

        Debug.Log($"[NetworkBehavior] 子 {player} の回答完了");

        if (AreAllChildrenAnswered())
        {
            Debug.Log("[NetworkBehavior] 子プレイヤー全員の回答完了。集計フェーズへ");

            // 回答が出揃ったので集計フェーズへ
            CurrentPhase = InGamePhase.Calculate;
        }
    }

    // =========================================================
    // 判定処理
    // =========================================================

    private void InitializePlayerSteps()
    {
        PlayerSteps.Clear();

        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            PlayerSteps.Set(player, PlayerAnswerStep.None);
        }
    }

    private void ResetRoundAnswerSteps()
    {
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            PlayerSteps.Set(player, PlayerAnswerStep.None);
        }
    }

    private bool AreAllPlayersReady()
    {
        int playerCount = 0;

        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            playerCount++;

            if (!PlayerSteps.TryGet(player, out PlayerAnswerStep step))
            {
                return false;
            }

            if (step != PlayerAnswerStep.Ready)
            {
                return false;
            }
        }

        return playerCount > 0;
    }

    private bool AreAllChildrenAnswered()
    {
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            // 親は子の回答判定に含めない
            if (player == ParentPlayer) continue;

            if (!PlayerSteps.TryGet(player, out PlayerAnswerStep step))
            {
                return false;
            }

            if (step != PlayerAnswerStep.ChildAnswered)
            {
                return false;
            }
        }

        return true;
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

    // =========================================================
    // デバッグ・確認用
    // =========================================================

    public bool IsPlayerReady(PlayerRef player)
    {
        return PlayerSteps.TryGet(player, out PlayerAnswerStep step)
               && step == PlayerAnswerStep.Ready;
    }

    public bool IsParentAnswered()
    {
        if (ParentPlayer == PlayerRef.None) return false;

        return PlayerSteps.TryGet(ParentPlayer, out PlayerAnswerStep step)
               && step == PlayerAnswerStep.ParentAnswered;
    }

    public bool IsChildAnswered(PlayerRef player)
    {
        if (player == ParentPlayer) return false;

        return PlayerSteps.TryGet(player, out PlayerAnswerStep step)
               && step == PlayerAnswerStep.ChildAnswered;
    }
}