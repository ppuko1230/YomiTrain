using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Fusion; // PlayerRefを使うために追加

public class LobbyUIManager : MonoBehaviour
{
    [Header("UIパネルの参照")]
    [Tooltip("Lobby画面全体をまとめる親オブジェクト")]
    [SerializeField] private GameObject lobbyRoot;

    [Header("UIパーツの参照")]
    [Tooltip("ルームIDを表示するテキスト")]
    [SerializeField] private TextMeshProUGUI roomIdText;

    [Tooltip("プレイヤー一覧を表示するテキスト")]
    [SerializeField] private TextMeshProUGUI playerListText;

    [Tooltip("ゲームを開始するボタン（ホストのみ押せるようにする）")]
    [SerializeField] private Button startGameButton;

    [Tooltip("部屋から退出するボタン")]
    [SerializeField] private Button leaveButton;

    private void Start()
    {
        // AppManagerの状態変化イベントに登録
        AppManager.Instance.OnStateChanged += HandleStateChanged;

        // はじめるボタン、退出ボタンの処理を登録
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnClickStartGame);
        }

        if (leaveButton != null)
        {
            leaveButton.onClick.AddListener(OnClickLeaveRoom);
        }

        // 初期状態では非表示にしておく
        if (lobbyRoot != null)
        {
            lobbyRoot.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (AppManager.Instance != null)
        {
            AppManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        //イベントの登録解除
        if(NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnPlayerListUpdated -= UpdatePlayerListUI;
        }
    }

    // ==========================================
    // AppManagerから状態変化の通知を受け取った時の処理
    // ==========================================
    private void HandleStateChanged(GameState newState)
    {
        // 状態が Lobby の時だけ true になる
        bool isLobbyState = (newState == GameState.RoomLobby);

        if (lobbyRoot != null)
        {
            lobbyRoot.SetActive(isLobbyState);
        }

        // Lobby画面が表示された瞬間にやりたい処理
        if (isLobbyState)
        {
            SetupLobby();
        }
        else
        {
            //Lobby以外の画面に切り替わったら、イベントを解除
            if(NetworkManager.Instance != null)
            {
                NetworkManager.Instance.OnPlayerListUpdated -= UpdatePlayerListUI;
            }
        }
    }

    // ==========================================
    // ロビー画面が表示されたときの初期設定
    // ==========================================
    private void SetupLobby()
    {
        Debug.Log("Lobby画面を表示します。");

        // NetworkManagerから記憶しておいたRoomIDをもらって表示する！
        if (NetworkManager.Instance != null && roomIdText != null)
        {
            roomIdText.text = $"Room ID: {NetworkManager.Instance.CurrentRoomId}";

            //NetworkManagerから「人数が変わったよ！」という通知を受けるイベント
            NetworkManager.Instance.OnPlayerListUpdated += UpdatePlayerListUI;

            //Lobby画面を開いた瞬間のメンバーを画面に反映
            List<PlayerRef> currentPlayers = NetworkManager.Instance.GetCurrentPlayers();
            UpdatePlayerListUI(currentPlayers);
        }
    }

    // ==========================================
    // プレイヤー一覧UIを更新する処理
    // ==========================================
    private void UpdatePlayerListUI(List<PlayerRef> players)
    {
        if (playerListText == null) return;

        // 見出しを作ってテキストをリセット
        playerListText.text = "【参加メンバー】\n";

        // リストの中身を一つずつ取り出して、改行しながら追加していく
        foreach (var player in players)
        {
            // 今回はまだ自由に名前を付ける機能がないので、とりあえずFusionのIDを表示します
            // 例: 「・Player_1」
            playerListText.text += $"・Player_{player.PlayerId}\n";
        }
    }

    // ==========================================
    // ボタンの処理
    // ==========================================
    private void OnClickStartGame()
    {
        // NetworkManagerを使って、自分がホストかどうかをチェックする
        if (NetworkManager.Instance != null && NetworkManager.Instance.IsHost)
        {
            Debug.Log("ホストとしてゲーム開始ボタンが押されました。親決め画面（InGame）へ遷移します。");

            // ホストの画面を遷移させる
            AppManager.Instance.ChangeState(GameState.InGame);

            // 【補足】
            // もしクライアント（子）も一斉にInGameへ画面遷移させたい場合は、
            // NetworkBehaviorのフェーズを移行させ、それに応じて各端末のAppManagerを
            // 動かすような設計にすると、全員の画面が綺麗に同期して切り替わります。
        }
        else
        {
            // クライアント（子）が押した場合は何もしないか、警告を出す
            Debug.LogWarning("ゲームを開始できるのはホストのみです。");
        }
    }

    private async void OnClickLeaveRoom()
    {
        Debug.Log("退出ボタンが押されました。切断処理を開始します...");

        // 退出ボタンが連打されるのを防ぐためにボタンを無効化
        if (leaveButton != null) leaveButton.interactable = false;

        // NetworkManager に追加した切断処理を非同期で実行
        if (NetworkManager.Instance != null)
        {
            await NetworkManager.Instance.LeaveRoomAsync();
        }

        // 切断が完了したら、ボタンを再度押せるように戻す
        if (leaveButton != null) leaveButton.interactable = true;

        // 通信が切れたので、ルーム選択画面に戻る
        AppManager.Instance.ChangeState(GameState.RoomSelect);
    }
}