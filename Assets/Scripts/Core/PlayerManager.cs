using Fusion;
using UnityEngine;

public enum PlayerRoleState
{
    Parent,
    Child
}

public enum PlayerState
{
    StartNotReady,
    StartReady,
    Createrd,
    NextQuestionReady
}

public struct PlayerStatus : INetworkStruct
{
    public PlayerRoleState RoleState;
    public PlayerState PlayerState;
}

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Networked]
    public PlayerRef ParentPlayer { get; private set; }

    [Networked, Capacity(6)]
    private NetworkDictionary<PlayerRef, PlayerStatus> PlayerStatuses => default;

    public override void Spawned()
    {
        Instance = this;

        if (Object.HasStateAuthority)
        {
            SetParentPlayer();
            SyncPlayerStatuses();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        SyncPlayerStatuses();
    }

    private void SetParentPlayer()
    {
        if (ParentPlayer != PlayerRef.None) return;

        ParentPlayer = Runner.LocalPlayer;

        Debug.Log($"[PlayerManager] ParentÇê›íËÇµÇ‹ÇµÇΩ: {ParentPlayer}");
    }

    private void SyncPlayerStatuses()
    {
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            if (PlayerStatuses.ContainsKey(player)) continue;

            PlayerStatus status = new PlayerStatus
            {
                RoleState = player == ParentPlayer
                    ? PlayerRoleState.Parent
                    : PlayerRoleState.Child,

                PlayerState = PlayerState.StartNotReady
            };

            PlayerStatuses.Set(player, status);

            Debug.Log($"[PlayerManager] {player} Ç {status.RoleState} / {status.PlayerState} Ç≈ìoò^ÇµÇ‹ÇµÇΩ");
        }
    }

    public void RequestStartReady()
    {
        RPC_ChangePlayerState(PlayerState.StartReady);
    }

    public void RequestAnswerCreated()
    {
        RPC_ChangePlayerState(PlayerState.Createrd);
    }

    public void RequestNextQuestionReady()
    {
        RPC_ChangePlayerState(PlayerState.NextQuestionReady);
    }

    public void RequestStartNotReady()
    {
        RPC_ChangePlayerState(PlayerState.StartNotReady);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_ChangePlayerState(PlayerState newState, RpcInfo info = default)
    {
        PlayerRef player = info.Source;

        if (!PlayerStatuses.TryGet(player, out PlayerStatus status))
        {
            status = new PlayerStatus
            {
                RoleState = player == ParentPlayer
                    ? PlayerRoleState.Parent
                    : PlayerRoleState.Child,

                PlayerState = PlayerState.StartNotReady
            };
        }

        status.PlayerState = newState;
        PlayerStatuses.Set(player, status);

        Debug.Log($"[PlayerManager] {player} ÇÃèÛë‘Ç {newState} Ç…ïœçXÇµÇ‹ÇµÇΩ");
    }

    public PlayerStatus GetPlayerStatus(PlayerRef player)
    {
        if (PlayerStatuses.TryGet(player, out PlayerStatus status))
        {
            return status;
        }

        return new PlayerStatus
        {
            RoleState = PlayerRoleState.Child,
            PlayerState = PlayerState.StartNotReady
        };
    }

    public bool IsParent(PlayerRef player)
    {
        return PlayerStatuses.TryGet(player, out PlayerStatus status)
               && status.RoleState == PlayerRoleState.Parent;
    }

    public bool IsChild(PlayerRef player)
    {
        return PlayerStatuses.TryGet(player, out PlayerStatus status)
               && status.RoleState == PlayerRoleState.Child;
    }

    public bool IsStartReady(PlayerRef player)
    {
        return PlayerStatuses.TryGet(player, out PlayerStatus status)
               && status.PlayerState == PlayerState.StartReady;
    }

    public bool IsAnswerCreated(PlayerRef player)
    {
        return PlayerStatuses.TryGet(player, out PlayerStatus status)
               && status.PlayerState == PlayerState.Createrd;
    }

    public bool IsNextQuestionReady(PlayerRef player)
    {
        return PlayerStatuses.TryGet(player, out PlayerStatus status)
               && status.PlayerState == PlayerState.NextQuestionReady;
    }

    public bool AreAllPlayersStartReady()
    {
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            if (!IsStartReady(player))
            {
                return false;
            }
        }

        return true;
    }

    public bool AreAllChildrenAnswerCreated()
    {
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            if (IsParent(player)) continue;

            if (!IsAnswerCreated(player))
            {
                return false;
            }
        }

        return true;
    }

    public bool AreAllPlayersNextQuestionReady()
    {
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            if (!IsNextQuestionReady(player))
            {
                return false;
            }
        }

        return true;
    }
}