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
        // TODO: ホストが「はじめる」を押したときの処理（親決め画面へ遷移など）
        Debug.Log("ゲーム開始ボタンが押されました");
    }

    private void OnClickLeaveRoom()
    {
        // TODO: 部屋から退出して、通信を切断し、RoomSelect状態に戻る処理
        Debug.Log("退出ボタンが押されました");
        // 例: AppManager.Instance.ChangeState(GameState.RoomSelect);
    }
}