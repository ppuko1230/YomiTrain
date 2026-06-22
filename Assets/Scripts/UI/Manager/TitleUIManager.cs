using UnityEngine;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject roomSelectionPanel;
    [SerializeField] private GameObject settingsPanel;

    // 開始ボタン
    public void ShowRoomSelection()
    {
        titlePanel.SetActive(false);
        roomSelectionPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // 設定ボタン
    public void ShowSettings()
    {
        titlePanel.SetActive(false);
        roomSelectionPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // 戻るボタン
    public void ReturnToTitle()
    {
        titlePanel.SetActive(true);
        roomSelectionPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    // 音量調整用バー
    public void OnVolumeChanged(float volume)
    {
        // 後で実装
        Debug.Log("音量：" + volume);
    }

}