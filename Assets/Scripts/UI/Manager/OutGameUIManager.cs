using UnityEngine;
using TMPro;

public class OutGameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject roomSelectionPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject joinRoomPanel;

    [SerializeField] private TMP_Text roomIDText;
    [SerializeField] private TMP_InputField roomIDInputField;

    // ルーム作成画面を表示
    public void ShowCreateRoom()
    {
        roomSelectionPanel.SetActive(false);
        createRoomPanel.SetActive(true);
        joinRoomPanel.SetActive(false);
    }

    // ルーム参加画面を表示
    public void ShowJoinRoom()
    {
        roomSelectionPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(true);
    }

    // ルーム選択画面へ戻る
    public void ReturnToRoomSelection()
    {
        roomSelectionPanel.SetActive(true);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
    }

    // ルームIDをコピー
    public void OnClickedCopy()
    {
        GUIUtility.systemCopyBuffer = roomIDText.text;
        Debug.Log("ルームIDをコピーしました");
    }

    // 受付開始ボタン
    public void StartMakeRoom()
    {
        Debug.Log("ルーム作成開始");

        // Photon Fusionでルーム作成処理を書く
    }

    // 入るボタン
    public void TryJoinRoom()
    {
        try
        {
            string roomID = roomIDInputField.text;

            Debug.Log($"ルーム {roomID} に参加");

            // Photon Fusionで参加処理を書く
        }
        catch (System.Exception e)
        {
            Debug.LogError($"参加失敗: {e.Message}");
        }
    }
}