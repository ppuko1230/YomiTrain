using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [Header("UIパネルの参照")]
    [Tooltip("Lobby画面全体をまとめる親オブジェクト")]
    [SerializeField] private GameObject lobbyRoot;

    [Header("UIパーツの参照")]
    [Tooltip("ルームIDを表示するテキスト")]
    [SerializeField] private TextMeshProUGUI roomIdText;

    [Tooltip("ゲームを開始するボタン（ホストのみ押せるようにする予定）")]
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
        }

        // TODO: ここに「現在部屋にいるプレイヤー一覧」を取得して表示する処理を追加していく
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