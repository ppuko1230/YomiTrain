using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// RoomSelectステート。
/// ルーム作成、ルーム参加、参加失敗時のローカルエラー表示を担当する。
/// </summary>
public class OutGameUIManager : MonoBehaviour
{
    public static OutGameUIManager Instance { get; private set; }

    [Header("Root")]
    [SerializeField] private GameObject rootPanel;

    [Header("Panels")]
    [SerializeField] private GameObject roomSelectPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private GameObject errorDialogPanel;

    [Header("Input")]
    [SerializeField] private TMP_InputField roomIDInputField;

    [Header("Text")]
    [SerializeField] private Text statusText;
    [SerializeField] private Text errorText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void SetVisible(bool visible)
    {
        if (rootPanel != null) rootPanel.SetActive(visible);
        if (visible) RefreshUI();
    }

    public void RefreshUI()
    {
        ShowRoomSelect();
        HideError();
        ShowStatus("ルームを作成するか、IDを入力して参加してください。");
    }

    public void ShowRoomSelect()
    {
        SetPanel(roomSelectPanel, true);
        SetPanel(createRoomPanel, false);
        SetPanel(joinRoomPanel, false);
    }

    public void ShowCreateRoom()
    {
        SetPanel(roomSelectPanel, false);
        SetPanel(createRoomPanel, true);
        SetPanel(joinRoomPanel, false);
        ShowStatus("ルーム作成ボタンを押すと、セッションIDを発行してHostとして入室します。");
    }

    public void ShowJoinRoom()
    {
        SetPanel(roomSelectPanel, false);
        SetPanel(createRoomPanel, false);
        SetPanel(joinRoomPanel, true);
        ShowStatus("ルームIDを入力してください。");
    }

    public void ShowStatus(string message)
    {
        if (statusText != null) statusText.text = message;
        Debug.Log($"[OutGameUIManager] {message}");
    }

    public void ShowError(string message)
    {
        if (errorDialogPanel != null) errorDialogPanel.SetActive(true);
        if (errorText != null) errorText.text = message;
        ShowStatus(message);
    }

    public void HideError()
    {
        if (errorDialogPanel != null) errorDialogPanel.SetActive(false);
    }

    public void OnClickedShowCreateRoomButton() => ShowCreateRoom();
    public void OnClickedShowJoinRoomButton() => ShowJoinRoom();
    public void OnClickedBackToRoomSelectButton() => ShowRoomSelect();
    public void OnClickedCloseErrorDialogButton() => HideError();

    public void OnClickedBackToTitleButton()
    {
        AppManager.Instance.ChangeState(GameState.Title);
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }

    // 旧OnClick名との互換
    public void ShowCreateRoomPanel() => OnClickedShowCreateRoomButton();
    public void ShowJoinRoomPanel() => OnClickedShowJoinRoomButton();
    public void ReturnToRoomSelection() => OnClickedBackToRoomSelectButton();
}
