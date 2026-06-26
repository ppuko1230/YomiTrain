using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OutGameUIManager : MonoBehaviour
{
    [Header("UIパネルの参照")]
    [SerializeField] private GameObject outGameRoot;        // RoomSelect画面全体をまとめる親
    [SerializeField] private GameObject roomSelectionPanel; // ルーム選択画面
    [SerializeField] private GameObject createRoomPanel;    // ルーム作成画面
    [SerializeField] private GameObject joinRoomPanel;      // ルーム参加画面

    [Header("ボタンの参照")]
    [SerializeField] private Button roomCreateButton;       // 作成画面へ行くボタン
    [SerializeField] private Button roomJoinButton;         // 参加画面へ行くボタン
    [SerializeField] private Button createBackButton;       // 作成画面から戻るボタン
    [SerializeField] private Button joinBackButton;         // 参加画面から戻るボタン

    private void Start()
    {
        // AppManagerの状態変化イベントに登録
        AppManager.Instance.OnStateChanged += HandleStateChanged;

        // ボタンが押されたときの処理を登録
        roomCreateButton.onClick.AddListener(ShowCreateRoomPanel);
        roomJoinButton.onClick.AddListener(ShowJoinRoomPanel);

        // 戻るボタンがある場合
        if (createBackButton != null)
        {
            createBackButton.onClick.AddListener(ShowRoomSelectionPanel);
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

        roomSelectionPanel.SetActive(isRoomSelectState);

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
        Debug.Log($"RoomSelectionPanleを表示します。");
        roomSelectionPanel.SetActive(true);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
    }

    /// <summary>
    /// RoomCreateButtonを押したとき、CreateRoomPanelへ移行する
    /// </summary>
    public void ShowCreateRoomPanel()
    {
        roomSelectionPanel.SetActive(false);
        createRoomPanel.SetActive(true);
        joinRoomPanel.SetActive(false);
    }

    /// <summary>
    /// RoomJoinButtonを押したとき、JoinRoomPanelへ移行する
    /// </summary>
    public void ShowJoinRoomPanel()
    {
        roomSelectionPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(true);
    }
}