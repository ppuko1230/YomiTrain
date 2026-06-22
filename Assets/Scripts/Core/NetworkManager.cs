using UnityEngine;

// 通信だけを担当するマネージャー
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ==========================================
    // 1. 送信：自分が何かアクションを起こした時
    // ==========================================
    public void SendStateChange(GameState nextState)
    {
        Debug.Log($"【通信送信】全員に {nextState} へ移動するように命令を送ります！");

        // ※ここにPhotonなどの通信処理を書く
        // photonView.RPC("RPC_ReceiveStateChange", RpcTarget.All, nextState);
    }

    // ==========================================
    // 2. 受信：ネットワークからメッセージが届いた時
    // ==========================================
    // ※Photonなら [PunRPC] などの属性がつく
    public void ReceiveStateChange(GameState newState)
    {
        Debug.Log($"【通信受信】ホストから {newState} へ移動する指示が来ました！");

        // ここで初めて AppManager に状態を変更させる
        AppManager.Instance.ChangeState(newState);
    }
}