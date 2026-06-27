using UnityEngine;
using UnityEngine.UI;

public class OutGameUIManager : MonoBehaviour
{
    [Header("UIパネルの参照")]
    [Tooltip("RoomSelect画面全体をまとめる親オブジェクト（あれば一括で消せて便利です）")]
    [SerializeField] private GameObject outGameRoot;
    [SerializeField] private GameObject roomSelectionPanel; // ルーム選択画面
    [SerializeField] private GameObject joinRoomPanel;      // ルーム参加画面
    // ※ createRoomPanel は削除されました

    [Header("ボタンの参照")]
    [SerializeField] private Button roomJoinButton;         // 参加画面へ行くボタン
    [SerializeField] private Button joinBackButton;         // 参加画面から戻るボタン
    // ※ roomCreateButton と createBackButton は削除されました

    private void Start()
    {
        // AppManagerの状態変化イベントに登録
        AppManager.Instance.OnStateChanged += HandleStateChanged;

        // ボタンが押されたときの処理を登録
        // （「部屋を作る」ボタンの処理はRoomSelectionPanelManagerが担当するのでここには不要です）
        if (roomJoinButton != null)
        {
            roomJoinButton.onClick.AddListener(ShowJoinRoomPanel);
        }

        if (joinBackButton != null)
        {
            joinBackButton.onClick.AddListener(ShowRoomSelectionPanel);
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
        bool isRoomSelectState = (newState == GameState.RoomSelect);

        // 状態がRoomSelectなら表示し、Lobbyなど他の状態なら非表示（消す）にする
        if (outGameRoot != null)
        {
            outGameRoot.SetActive(isRoomSelectState);
        }
        else
        {
            // 親オブジェクトが設定されていない場合は個別に非表示にする
            if (!isRoomSelectState)
            {
                roomSelectionPanel.SetActive(false);
                joinRoomPanel.SetActive(false);
            }
        }

        // RoomSelect状態に入った時は、確実に基本の選択画面を出す
        if (isRoomSelectState)
        {
            ShowRoomSelectionPanel();
        }
    }

    // ==========================================
    // パネル切り替え処理
    // ==========================================

    /// <summary>
    /// ルーム選択画面を表示する
    /// </summary>
    public void ShowRoomSelectionPanel()
    {
        Debug.Log("RoomSelectionPanelを表示します。");
        roomSelectionPanel.SetActive(true);
        joinRoomPanel.SetActive(false);
    }

    /// <summary>
    /// RoomJoinButtonを押したとき、JoinRoomPanelへ移行する
    /// </summary>
    public void ShowJoinRoomPanel()
    {
        roomSelectionPanel.SetActive(false);
        joinRoomPanel.SetActive(true);
    }
}